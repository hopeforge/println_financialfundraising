namespace Shared

open System

type RegisterData =
    { Email: string
      Password: string
      Name: string
      CNPJ: string }

type LoginData =
    { Email: string
      Password: string }

type UserData =
    { Email: string
      Name: string
      CNPJ: string
      Token: string }

type DonationData =
    { Email: string
      Name: string
      CPF: string
      Value: float
      Identifier: string }

type OpenDonationInfo =
    { Date: DateTime
      PurchaseID: string
      Value: float }

type ClosedDonationInfo =
    { DateOpen: DateTime
      DateClosed: DateTime
      Paid: bool
      PayReference: string
      LinkToPay: string }
