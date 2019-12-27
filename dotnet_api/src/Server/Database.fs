module Database

open FSharp.Data.Sql
open FSharp.Control.Tasks.ContextInsensitive
open System
open Shared

type SQL = SqlDataProvider<Common.DatabaseProviderTypes.MYSQL, "Server=localhost;Database=hackatonFinancialFundraising;Uid=root;Pwd=N9sdhpi2Dhn2", UseOptionTypes = true>

type UserCreation =
    | NewUser of {| Id: int; Token: string |}
    | ExistingUser

type DonorCreation =
    | NewDonor of int
    | ExistingDonor of int

let connection = SQL.GetDataContext()


let markPaid invoiceId =

    task{
        query {
            for i in connection.HackatonFinancialFundraising.Periodo do
                where (i.LinkPagamento = Some invoiceId)
                select i
        } |> Seq.iter (fun inv -> inv.Pago <- 1y)
        connection.SubmitUpdates()
    }

let renewToken userId token =
    task {
        let lojas = connection.HackatonFinancialFundraising.Loja

        let lojaById =
            query {
                for loja in lojas do
                    where (loja.IdLoja = userId)
                    select loja
            }
            |> Seq.tryHead
        return lojaById
               |> Option.map (fun loja ->
                   loja.Token <- token
                   connection.SubmitUpdates()
                   token)
    }

let findUser cnpj email =
    task {
        return query {
                   for user in connection.HackatonFinancialFundraising.Loja do
                       where (user.Cnpj = cnpj || user.Email = email)
               }
               |> Seq.tryHead
    }

let getUser userId =
    task {
        return query {
                   for user in connection.HackatonFinancialFundraising.Loja do
                       where (user.IdLoja = userId)
               }
               |> Seq.tryHead
               |> Option.map (fun e ->
                   {| UserId = e.IdLoja
                      Token = e.Token
                      CNPJ = e.Cnpj
                      Email = e.Email
                      Name = e.NomeLoja |})
    }

let loginUser email password =
    task {
        return query {
                   for user in connection.HackatonFinancialFundraising.Loja do
                       where (user.Senha = password || user.Email = email)
               }
               |> Seq.tryHead
               |> Option.map (fun e ->
                   {| UserId = e.IdLoja
                      Email = e.Email
                      Name = e.NomeLoja
                      CNPJ = e.Cnpj
                      Token = e.Token |})
    }

let loginDonor email password =
    task {
        return query {
                   for user in connection.HackatonFinancialFundraising.Doador do
                       where (user.Senha = password || user.Email = email)
               }
               |> Seq.tryHead
               |> Option.map (fun e ->
                   {| UserId = e.IdDoador
                      Email = e.Email
                      Name = e.NomeDoador
                      CPF = e.Cpf |})
    }

let createPeriodo() =
    task {
        let periodo = connection.HackatonFinancialFundraising.Periodo.Create(Abertura = DateTime.Now, Pago = 0y)
        connection.SubmitUpdates()
        return periodo
    }

let createUser cpnj name email pass =

    task {
        match! findUser cpnj email with
        | Some _ -> return ExistingUser
        | None ->
            let! periodo = createPeriodo()
            let user =
                connection.HackatonFinancialFundraising.Loja.Create
                    (Cnpj = cpnj, Email = email, IdPeriodo = Some periodo.IdPeriodo, NomeLoja = name, Senha = pass,
                     Token = Guid.NewGuid().ToString("N"))
            connection.SubmitUpdates()
            periodo.IdLoja <- Some user.IdLoja
            connection.SubmitUpdates()
            return NewUser
                       {| Id = user.IdLoja
                          Token = user.Token |}
    }

let getOpen idUser =
    task {
        return query {
                   for user in connection.HackatonFinancialFundraising.Loja do
                       join p in connection.HackatonFinancialFundraising.Periodo on (user.IdPeriodo = Some p.IdPeriodo)
                       join d in connection.HackatonFinancialFundraising.Doacoes on (p.IdPeriodo = d.IdPeriodo)
                       where (user.IdLoja = idUser)
                       select
                           ({ PurchaseID = d.NumPedido
                              Date =  d.DataDoacao.Value
                              Value = d.Valor.Value })
               }
               |> Seq.toList
    }

let closed idUser =
    task {
        return query {
                    for p in connection.HackatonFinancialFundraising.Periodo do
                    where (p.Fechamento.IsSome && p.LinkPagamento.IsSome && p.IdLoja = Some idUser )
                    select ({DateClosed = p.Fechamento.Value
                             DateOpen = p.Abertura
                             Paid = p.Pago <> 0y

                             PayReference = p.LinkPagamento.Value
                             LinkToPay = p.LinkPagamento.Value
                             })} |> Seq.toList

}

let closePeriod idUser link =
    task {
        let user =
            query {
                for x in connection.HackatonFinancialFundraising.Loja do
                    where (x.IdLoja = idUser)
                    select x
            }
            |> Seq.tryHead
        match user with
        | Some user ->
            let per =
                query {
                    for p in connection.HackatonFinancialFundraising.Periodo do
                        where (Some p.IdPeriodo = user.IdPeriodo)
                        select p
                }
                |> Seq.tryHead
            match per with
            | Some periodo ->
                periodo.Fechamento <- Some DateTime.Now
                periodo.LinkPagamento <- link
                let! newPeriodo = createPeriodo()
                newPeriodo.IdLoja <- Some user.IdLoja
                connection.SubmitUpdates()
                user.IdPeriodo <- Some newPeriodo.IdPeriodo
                connection.SubmitUpdates()
                return! closed idUser
            | None -> return failwith "Dados inválidos"
        | None -> return failwith "Dados inválidos"
    }

let addDonation value idUser idDonor userIdentifier =
    task {
        let period =
            query {
                for user in connection.HackatonFinancialFundraising.Loja do
                    where (user.IdLoja = idUser)
                    select user.IdPeriodo
            }
            |> Seq.tryHead
            |> Option.flatten
        match period with
        | Some p ->
            let donation =
                connection.HackatonFinancialFundraising.Doacoes.Create
                    (Valor = value, IdDoador = idDonor, IdLoja = idUser, IdPeriodo = p, NumPedido = userIdentifier,
                     DataDoacao = Some DateTime.Now)
            connection.SubmitUpdates()
            return Some donation.IdDoacao
        | None -> return None
    }

let getOrCreateDonor nome cpf email =
    task {
        let q =
            query {
                for user in connection.HackatonFinancialFundraising.Doador do
                    where (user.Email.Value = email || user.Cpf.Value = cpf)
                    select user
            }
            |> Seq.tryHead
        match q with
        | Some d -> return ExistingDonor d.IdDoador
        | None ->
            let user = connection.HackatonFinancialFundraising.Doador.Create(NomeDoador=nome, Email = Some email, Cpf = Some cpf)
            connection.SubmitUpdates()
            return NewDonor user.IdDoador
    }
