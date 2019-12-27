open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared
open Microsoft.IdentityModel.Tokens
open System
open Microsoft.AspNetCore.Http
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt


let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x
let secret = "HMAC_SECRET" |> tryGetEnv |> Option.defaultValue "spadR2dre#u-ruBrE@TepA&*Uf@U"
let issuer = "JWT_ISSUER" |> tryGetEnv |> Option.defaultValue "Validador"
let generate_token sub token  =
    let nameClaim = [Some (Claim("uid", sub)); token |> Option.map (fun t -> Claim("dnKey",t))] |> List.choose id

    Auth.generateJWT
        (secret, SecurityAlgorithms.HmacSha256)
        issuer
        (DateTime.Now.AddMonths 1)
        nameClaim

let publicPath = Path.GetFullPath "../Client/public"

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8085us



let userFromCtx (ctx : HttpContext) = task {
    let uid =
        ctx.User.FindFirst("uid")
        |> Option.ofObj |> Option.bind (fun e -> e.Value |> Option.ofObj)
    match uid with
    | Some e -> return! Database.getUser (int e)
    | None -> return None

}

let donation expectedToken =
    router {
        pipe_through (Auth.requireClaim JWT "dnKey" expectedToken)
        pipe_through (fun next ctx -> task {
                let! don = ctx.BindJsonAsync<DonationData>()
                match! userFromCtx ctx with
                |Some e ->
                    let! (Database.NewDonor i | Database.ExistingDonor i) as idDonor =
                        Database.getOrCreateDonor don.Name don.CPF don.Email
                    let! x = Database.addDonation (Some don.Value) e.UserId i don.Identifier
                    match x with
                    |Some i -> return! Successful.CREATED i next ctx
                    |None -> return! RequestErrors.UNPROCESSABLE_ENTITY "Erro ao persistir" next ctx
                |None ->
                    return! RequestErrors.FORBIDDEN "User not found" next ctx
        })
    }

let authorized =
    router {
        pipe_through (Auth.requireAuthentication JWT)
        post "/closePeriod" (fun next ctx -> task {
            match! userFromCtx ctx with
            |Some i ->
                let! doit = ctx.BindJsonAsync<bool>()
                if doit then
                        let! openDonations = Database.getOpen i.UserId
                        let value = openDonations |> List.sumBy (fun e -> e.Value)
                        let! invoiceLink = PayPal.createInvoice (decimal value) {CNPJ = i.CNPJ;Token=i.Token;Email=i.Email;Name=i.Name}
                        let! n = Database.closePeriod i.UserId (Some invoiceLink)
                        let k = n |> List.map ( fun e -> {e with LinkToPay = sprintf "https://www.sandbox.paypal.com/invoice/payerView/details/%s" e.LinkToPay})
                        return! json k next ctx

                else
                    return! Successful.NO_CONTENT next ctx
            | None -> return! RequestErrors.NOT_FOUND "Not found" next ctx
        })
        get "/getOpenInfo" (fun next ctx -> task {
            match! userFromCtx ctx with
            |Some i ->
                    let! openDonations = Database.getOpen i.UserId
                    return! json openDonations next ctx
            | None -> return! RequestErrors.NOT_FOUND "Not found" next ctx
        })
        get "/getClosedInfo" (fun next ctx -> task {
            match! userFromCtx ctx with
            |Some i ->
                    let! n = Database.closed i.UserId
                    let closedDonations = n |> List.map ( fun e -> {e with LinkToPay = sprintf "https://www.sandbox.paypal.com/invoice/payerView/details/%s" e.LinkToPay})

                    return! json closedDonations next ctx
            | None -> return! RequestErrors.NOT_FOUND "Not found" next ctx
        })
        get "/renewToken"
            (fun next ctx ->
                task {
                       match! userFromCtx ctx with
                       | Some e ->
                            let t = Guid.NewGuid().ToString("N")
                            let! x = Database.renewToken e.UserId t
                            return! json (generate_token (string e.UserId) x) next ctx
                       | None -> return! RequestErrors.FORBIDDEN "No claims" next ctx  })
        post "/addDonation" (fun next ctx -> task{
                match! userFromCtx ctx with
                |None -> return! RequestErrors.FORBIDDEN "Not found" next ctx
                |Some i ->  return! donation i.Token next ctx
        })

        post "/updatePay" (fun next ctx -> task{
                match! userFromCtx ctx with
                |None -> return! RequestErrors.FORBIDDEN "Not found" next ctx
                |Some i ->
                    let! str = ctx.BindJsonAsync<{|Id:string|}>()
                    let! status = PayPal.updateInvoice str.Id
                    if status then
                        do! Database.markPaid str.Id
                        return! json true next ctx
                    else return! json false next ctx
        })}


let webApp = router {
    post "/api/signup" (fun next ctx ->
        task {
            let! sign = ctx.BindJsonAsync<RegisterData>()
            let! id = Database.createUser sign.CNPJ sign.Name sign.Email sign.Password
            match id with
            | Database.NewUser e ->
                let t = generate_token (string e.Id) (Some e.Token)
                return! json {UserData.Token = t; Name = sign.Name; Email = sign.Email; CNPJ = sign.CNPJ} next ctx
            | Database.ExistingUser ->
                return! RequestErrors.CONFLICT "User already exists" next ctx
        })
    post "/api/login" (fun next ctx ->
        task{
            let! user = ctx.BindJsonAsync<LoginData>()
            let! id = Database.loginUser user.Email user.Password
            match id with
            |Some e ->
                return! json
                    { UserData.Token = generate_token (string e.UserId) (Some e.Token)
                      Name = e.Name
                      Email = e.Email
                      CNPJ = e.CNPJ} next ctx
            |None -> return! RequestErrors.NOT_FOUND "Usuário ou senha inválidos" next ctx
        })
    forward "/api/secure" authorized
}

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_json_serializer(Thoth.Json.Giraffe.ThothSerializer())
    use_gzip
    use_jwt_authentication secret issuer
}

run app
