open System.IO
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2
open Giraffe
open Saturn
open Shared

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Microsoft.AspNetCore.Http
open System.Net.Http
open Giraffe.Serialization

let tryGetEnv = System.Environment.GetEnvironmentVariable >> function null | "" -> None | x -> Some x

let publicPath = Path.GetFullPath "../Client/public"

let produtos = [
    {
        Nome = "Meia"
        Valor = 3.2m
        Descricao = "Esquenta pés"
        Imagem = Some "socks-128.png"
    }
    {
        Nome = "Mochila"
        Valor = 52.15m
        Descricao = "Carrega meias"
        Imagem = Some "military-backpack-radio-128.ico"
    }
    {
        Nome = "Tenis"
        Valor = 220.12m
        Descricao = "Bom com meias"
        Imagem = Some "boot-icon.png"
    }
]

let port =
    "SERVER_PORT"
    |> tryGetEnv |> Option.map uint16 |> Option.defaultValue 8095us

let token = tryGetEnv "SERVER_AUTH_TOKEN"
let url = tryGetEnv "SERVER_AUTH_URL"

let counterApi (ctx:HttpContext) = {
    CarregaProdutos = fun () -> async { return produtos }
    CriaCliente = fun nome email cpf -> async { return {Nome = nome; Email = email; CPF = cpf}}
    EnviaCompra = fun compra -> async {
             let v = compra.Produtos |> List.tryPick (function (Doacao e) -> Some e | _ -> None ) |> Option.defaultValue 0m
             if v > 0m then
              match token, url with
              | Some token, Some url ->
                let cli = ctx.GetService<IHttpClientFactory>()
                let client = cli.CreateClient()
                use req = new HttpRequestMessage(HttpMethod.Post, url)
                req.Headers.Add("Authorization", sprintf "Bearer %s" token)
                req.Headers.Add("Accept", "application/json")
                let body =
                    Utf8Json.JsonSerializer.ToJsonString(
                        {|
                          Email = compra.Cliente.Email
                          Name= compra.Cliente.Nome
                          CPF=compra.Cliente.CPF
                          Value = v
                          Identifier= string <| System.Guid.NewGuid() |})
                req.Content <- new StringContent(body)
                let! resp = client.SendAsync(req) |> Async.AwaitTask
                return resp.IsSuccessStatusCode

              | _ -> return false;
             else return false;
            }
}

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext counterApi
    |> Remoting.buildHttpHandler

let app = application {
    url ("http://0.0.0.0:" + port.ToString() + "/")
    use_router webApp
    memory_cache
    use_static publicPath
    use_gzip
    service_config (fun s -> s.AddHttpClient())

}

run app
