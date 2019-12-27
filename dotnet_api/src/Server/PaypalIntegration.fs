module PayPal

open PayPal.Core
open Shared
open FSharp.Control.Tasks.ContextInsensitive
open System.Configuration
open PayPal.v1.Invoices

let tryGetEnv =
    System.Environment.GetEnvironmentVariable
    >> function
    | null
    | "" -> None
    | x -> Some x

let environment = SandboxEnvironment("AaOH5xy59eDW1iFUDm_fz7vo2iXNad49c4r9YgZ15UIJa8zdpZi7mrDL68aZge_BxYQ5BSlepRsSWKAw","EC0sE7duQyc2-HKKfvS-JRi85fZO464SHAKkCVgkwwcwFKeu50DSj0qbckczdFycl7pr1-iTQK9Agx7n")
let client = PayPalHttpClient(environment)

let createInvoice value (user: UserData) =
    task {
        let list = ResizeArray<InvoiceItem>()
        list.Add(InvoiceItem(Name="Repasse de doação", Description="Doação GRAAC", Quantity=System.Nullable.op_Implicit 1., UnitPrice=Currency(CurrencyCode="BRL", Value=string value)))
        let list2 = ResizeArray<BillingInfo>()
        list2.Add(BillingInfo(Email=user.Email))
        let inv = Invoice
                     (AllowPartialPayment = System.Nullable.op_Implicit false, Items=list,
                      InvoiceDate = System.DateTime.UtcNow.ToString("yyyy-MM-dd UTC"), BillingInfo = list2, MerchantInfo = MerchantInformation(BusinessName=user.Name)
                     )
        use req = (new InvoiceCreateRequest()).RequestBody(inv)
        let! cli = client.Execute(req)
        let inv = cli.Result<Invoice>()
        let link = inv.Links |> Seq.pick (fun e -> Some e.Href)
        let reqs = new InvoiceSendRequest(inv.Id)
        let! cli = client.Execute(reqs)
        return inv.Id
    }

let updateInvoice invId =
    task {
        use req = new InvoiceGetRequest(invId)
        let! cli = client.Execute(req)
        let inv = cli.Result<Invoice>()
        return inv.Status = "PAID"
    }

