module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fetch.Types
open Thoth.Fetch
open Fulma
open Thoth.Json

open Shared
open Fable.Core.JsInterop

module Login =

    type Model =
        { User: string
          Pass: string
          Error: string option }

    type Msg =
        | UpdatePass of string
        | UpdateUser of string
        | Login
        | Create
        | SetLastError of exn
        | LoggedIn of UserData

    let init() =
        { User = ""
          Pass = ""
          Error = None }

    let update msg model =
        match msg with
        | UpdatePass p -> { model with Pass = p }, Cmd.none
        | UpdateUser u -> { model with User = u }, Cmd.none
        | SetLastError e -> { model with Error = Some(e.Message) }, Cmd.none
        | Login ->
            let cmd b =
                Fetch.fetchAs<UserData>
                    ("/admin/api/login",
                     [ Body !^b
                       Method HttpMethod.POST ])
            model,
            Cmd.OfPromise.either cmd
                (Encode.Auto.toString
                    (0,
                     { Email = model.User
                       Password = model.Pass })) LoggedIn SetLastError
        | Create
        | LoggedIn(_) -> failwith "Captured on parent"

    let view model dispatch =
        Columns.columns [ Columns.Option.Props [ Style [ JustifyContent "center" ] ] ]
            [ Column.column [ Column.Width(Screen.All, Column.Is3) ]
                  [ Panel.panel []
                        [ Panel.heading [] [ str "Login" ]
                          Panel.block []
                              [ Control.div [ Control.IsExpanded ]
                                    [ Field.label [] [ str "Email" ]
                                      Control.p [ Control.IsExpanded ]
                                          [ Input.email [ Input.OnChange(fun e -> dispatch (UpdateUser e.Value)) ] ] ] ]
                          Panel.block []
                              [ Control.div [ Control.IsExpanded ]
                                    [ Field.label [] [ str "Senha" ]
                                      Control.p [ Control.IsExpanded ]
                                          [ Input.password [ Input.OnChange(fun e -> dispatch (UpdatePass e.Value)) ] ] ] ]
                          Panel.block []
                              [ Button.a
                                  [ Button.OnClick(fun _ -> dispatch Create)
                                    Button.IsFullWidth ] [ str "Criar conta" ]
                                Button.a
                                    [ Button.OnClick(fun _ -> dispatch Login)
                                      Button.IsFullWidth ] [ str "Login" ] ] ] ] ]

module Create =
    type Model =
        { User: string
          Pass: string
          Name: string
          CNPJ: string
          Error: string option }

    type Msg =
        | UpdatePass of string
        | UpdateName of string
        | UpdateCNPJ of string
        | UpdateUser of string
        | Create
        | SetLastError of exn
        | SignedIn of UserData

    let init() =
        { User = ""
          Pass = ""
          Name = ""
          CNPJ = ""
          Error = None }

    let update msg model =
        match msg with
        | UpdatePass p -> { model with Pass = p }, Cmd.none
        | UpdateName n -> { model with Name = n }, Cmd.none
        | UpdateCNPJ c -> { model with CNPJ = c }, Cmd.none
        | UpdateUser u -> { model with User = u }, Cmd.none
        | SetLastError e -> { model with Error = Some(e.Message) }, Cmd.none
        | Create ->
            let cmd b =
                Fetch.fetchAs<UserData>
                    ("/admin/api/signup",
                     [ Body !^b
                       Method HttpMethod.POST ])
            model,
            Cmd.OfPromise.either cmd
                (Encode.Auto.toString
                    (0,
                     { RegisterData.Email = model.User
                       Name = model.Name
                       CNPJ = model.CNPJ
                       Password = model.Pass })) SignedIn SetLastError
        | SignedIn(_) -> failwith "Captured on parent"

    let view model dispatch =
        Columns.columns [ Columns.Option.Props [ Style [ JustifyContent "center" ] ] ]
            [ Column.column [ Column.Width(Screen.All, Column.Is4) ]
                  [ Panel.panel []
                        [ Panel.heading [] [ str "Login" ]
                          Panel.block []
                              [ Control.div [ Control.IsExpanded ]
                                    [ Field.label [] [ str "Email" ]
                                      Control.p []
                                          [ Input.email [ Input.OnChange(fun e -> dispatch (UpdateUser e.Value)) ] ] ] ]
                          Panel.block []
                              [ Control.div [ Control.IsExpanded ]
                                    [ Field.label [] [ str "Senha" ]
                                      Control.p []
                                          [ Input.password [ Input.OnChange(fun e -> dispatch (UpdatePass e.Value)) ] ] ] ]
                          Panel.block []
                              [ Control.div [ Control.IsExpanded ]
                                    [ Field.label [] [ str "Nome" ]
                                      Control.p []
                                          [ Input.input [ Input.OnChange(fun e -> dispatch (UpdateName e.Value)) ] ] ] ]
                          Panel.block []
                              [ Control.div [ Control.IsExpanded ]
                                    [ Field.label [] [ str "CNPJ" ]
                                      Control.p []
                                          [ Input.input [ Input.OnChange(fun e -> dispatch (UpdateCNPJ e.Value)) ] ] ] ]
                          Panel.block []
                              [ Button.a
                                  [ Button.OnClick(fun _ -> dispatch Create)
                                    Button.IsFullWidth ] [ str "Criar conta" ] ] ] ] ]

module PanelSub =
    type Pages =
        | Dev
        | Open of Shared.OpenDonationInfo list
        | Closed of Shared.ClosedDonationInfo list
        | Doc

module UserPanel =
    type Model =
        { Error: string option
          UserData: UserData
          Page: PanelSub.Pages }

    type Msg =
        | ChangeToken
        | CloseOpen
        | TokenChanged of string
        | SetLastError of exn
        | SetPage of PanelSub.Pages
        | DownloadOpen
        | DownloadClosed
        | UpdatePay of string
        | GotOpen of Shared.OpenDonationInfo list
        | GotClosed of Shared.ClosedDonationInfo list

    let update msg model =
        match msg with
        | ChangeToken ->
            let cmd() =
                Fetch.fetchAs<string>
                    ("/admin/api/secure/renewToken",
                     [ Method HttpMethod.GET
                       Fetch.requestHeaders ([ Authorization(sprintf "Bearer %s" model.UserData.Token) ]) ])
            model, Cmd.OfPromise.either cmd () TokenChanged SetLastError

        | GotOpen l -> { model with Page = PanelSub.Open l }, Cmd.none
        | GotClosed l -> { model with Page = PanelSub.Closed l }, Cmd.none
        | UpdatePay s ->
            let cmd s =
                Fetch.fetchAs<ClosedDonationInfo list>
                    ("/admin/api/secure/updatePay",
                     [ Method HttpMethod.POST
                       Body !^(sprintf "{\"Id\":\"%s\"}" s)
                       Fetch.requestHeaders ([ Authorization(sprintf "Bearer %s" model.UserData.Token) ]) ])
            model, Cmd.OfPromise.either cmd s GotClosed SetLastError
        | CloseOpen ->
            let cmd() =
                Fetch.fetchAs<ClosedDonationInfo list>
                    ("/admin/api/secure/closePeriod",
                     [ Method HttpMethod.POST
                       Body !^"true"
                       Fetch.requestHeaders ([ Authorization(sprintf "Bearer %s" model.UserData.Token) ]) ])
            model, Cmd.OfPromise.either cmd () GotClosed SetLastError
        | DownloadOpen ->
            let cmd() =
                Fetch.fetchAs<OpenDonationInfo list>
                    ("/admin/api/secure/getOpenInfo",
                     [ Method HttpMethod.GET
                       Fetch.requestHeaders ([ Authorization(sprintf "Bearer %s" model.UserData.Token) ]) ])
            model, Cmd.OfPromise.either cmd () GotOpen SetLastError
        | DownloadClosed ->
            let cmd() =
                Fetch.fetchAs<ClosedDonationInfo list>
                    ("/admin/api/secure/getClosedInfo",
                     [ Method HttpMethod.GET
                       Fetch.requestHeaders ([ Authorization(sprintf "Bearer %s" model.UserData.Token) ]) ])
            model, Cmd.OfPromise.either cmd () GotClosed SetLastError
        | SetLastError ex -> { model with Error = Some(ex.Message) }, Cmd.none
        | TokenChanged(_) -> failwith "Captured on parent"
        | SetPage p -> { model with Page = p }, Cmd.none

    let view model dispatch =
        Panel.panel []
            [ Panel.heading [] [ str (sprintf "Olá, %s" model.UserData.Name) ]
              Panel.tabs []
                  [ Panel.tab
                      [ Panel.Tab.IsActive(model.Page = PanelSub.Dev)

                        Panel.Tab.Props [ OnClick(fun _ -> dispatch <| SetPage PanelSub.Dev) ] ]
                        [ str "Opções de desenvolvedor" ]
                    Panel.tab
                        [ Panel.Tab.IsActive
                            (match model.Page with
                             | PanelSub.Open _ -> true
                             | _ -> false)

                          Panel.Tab.Props
                              [ OnClick(fun _ ->
                                  dispatch <| DownloadOpen
                                  dispatch <| SetPage(PanelSub.Open [])) ] ] [ str "Lançamentos em aberto" ]
                    Panel.tab
                        [ Panel.Tab.IsActive
                            (match model.Page with
                             | PanelSub.Closed _ -> true
                             | _ -> false)

                          Panel.Tab.Props
                              [ OnClick(fun _ ->
                                  dispatch <| DownloadClosed
                                  dispatch <| SetPage(PanelSub.Closed [])) ] ] [ str "Repasses passados" ]
                    Panel.tab
                        [ Panel.Tab.IsActive(model.Page = PanelSub.Doc)

                          Panel.Tab.Props [ OnClick(fun _ -> dispatch <| SetPage PanelSub.Doc) ] ]
                        [ str "Documentação" ] ]

              match model.Page with
              | PanelSub.Doc ->
                  Panel.block [ Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]
                      [ Heading.h3 [] [ str "APIs públicas" ] ]
                  Panel.block [] [ Control.p [] [ str "As seguintes APIs não requerem autenticação:" ] ]
                  let postDoc api desc entrada saida =
                    Panel.block[][
                      Container.container [ Container.IsFluid ]
                          [ Heading.h5 [] [ str (sprintf "POST - %s" api) ]
                            Heading.h6 [ Heading.IsSubtitle ] [ str desc ]
                            Field.div []
                                [ Field.label [] [ str "Entrada" ]
                                  Control.p []
                                      [ code
                                          [ Style
                                              [ Display DisplayOptions.Block
                                                WhiteSpace WhiteSpaceOptions.PreWrap ] ] [ str entrada ] ] ]
                            Field.div []
                                [ Field.label [] [ str "Saida" ]
                                  Control.p []
                                      [ code
                                          [ Style
                                              [ Display DisplayOptions.Block
                                                WhiteSpace WhiteSpaceOptions.PreWrap ] ] [ str saida ] ] ] ] ]
                  let getDoc api desc saida =
                    Panel.block[][
                      Container.container [ Container.IsFluid ]
                          [ Heading.h5 [] [ str (sprintf "GET - %s" api) ]
                            Heading.h6 [ Heading.IsSubtitle ] [ str desc ]
                            Field.div []
                                [ Field.label [] [ str "Saida" ]
                                  Control.p []
                                      [ code
                                          [ Style
                                              [ Display DisplayOptions.Block
                                                WhiteSpace WhiteSpaceOptions.PreWrap ] ] [ str saida ] ] ] ] ]
                  postDoc "/api/login"
                                       "Recebe os dados de usuário e senha e fornece os dados do usuário logado" """    { Email: <email de login>
      Password: <senha do usuário> }""" """    { Email: <email de login>
      Name: <nome do usuário>
      CNPJ: <CNPJ do usuário>
      Token: <Token Bearer para as APIs autenticadas> }"""
                  postDoc "/api/signup"  "Recebe os dados de cadastro e fornece os dados do novo usuário logado" """
    { Email: <email de login>
      Password: <senha do usuário>
      Name: <nome do usuário>
      CNPJ: <CNPJ do usuário> }""" """
    { Email: <email de login>
      Name: <nome do usuário>
      CNPJ: <CNPJ do usuário>
      Token: <Token Bearer para as APIs autenticadas> }"""

                  Panel.block [ Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]
                      [ Heading.h3 [] [ str "APIs autenticadas" ] ]
                  Panel.block [] [ Control.p [] [ str "As seguintes APIs requerem o token no cabeçalho 'Authorization' com o prefixo 'Bearer':" ] ]
                  postDoc "/api/secure/addDonation" "Adiciona os dados da doação feita pelo cliente" """    { Email: <email do doador>
      Name: <Nome do doador>
      CPF: <CPF do doador>
      Value: <Valor da doação:numérico>
      Identifier: <Identificador da compra origem> }""" "<id da inserção>"
                  postDoc "/api/secure/closePeriod" "Fecha o periodo de contabilização e gera fatura para os lançamentos" "<Confirma a intenção de fechar o período:booleano>" """   [{ DateOpen: <Data de abertura:Date>
      DateClosed: <Data de fechamento:Date>
      Paid: <Fatura já paga:booleano>
      PayReference: <Referência para o pagamento>
      LinkToPay: <Link para pagamento> }]"""
                  getDoc "/api/secure/getClosedInfo" "Recupera todos os lançamentos de faturas" """   [{ DateOpen: <Data de abertura:Date>
      DateClosed: <Data de fechamento:Date>
      Paid: <Fatura já paga:booleano>
      PayReference: <Referência para o pagamento>
      LinkToPay: <Link para pagamento> }]"""
                  getDoc "/api/secure/getOpenInfo" "Recupera os lançamentos em aberto para pagamento" "   [{ Date: <Data da doação:Date
      PurchaseID: <Identificador da compra origem>
      Value: <Valor da doação:numérico> }]"
                  postDoc "/api/secure/updatePay" "Verifica a existência de pagamento para a fatura selecionada" "{ Id: <Id da mensagem> }" "<Verdadeiro caso o pagamento tenha sido realizado>"
                  getDoc "/api/secure/renewToken" "Cria um novo token e revoga o anterior" "<novo token>"
              | PanelSub.Dev ->
                  Panel.block []
                      [ Control.div [ Control.IsExpanded ]
                            [ Field.div [ Field.IsExpanded   ]
                                  [ Control.div [ Control.IsExpanded ]
                                        [ Control.div [ Control.IsExpanded ]
                                              [ str "Este é seu token para fazer chamadas na API, guarde-o com cuidado" ] ]
                                    Control.p [ Control.IsExpanded ]
                                        [ Textarea.textarea
                                            [ Textarea.Props [ Style [ Width "100%" ] ]
                                              Textarea.IsFullWidth
                                              Textarea.IsReadOnly true
                                              Textarea.Value model.UserData.Token ] [] ]
                                    Control.p [ Control.IsExpanded ]
                                        [ Button.a [ Button.OnClick(fun _ -> dispatch ChangeToken) ]
                                              [ str "Alterar token" ] ] ] ] ]
              | PanelSub.Open op ->
                  Panel.block []
                      [ match op with
                        | [] -> str "Nada a mostrar"
                        | n ->
                            Table.table [ Table.IsFullWidth ]
                                [ thead []
                                      [ tr []
                                            [ th [] [ str "Data" ]
                                              th [] [ str "Pedido" ]
                                              th [] [ str "Valor" ] ] ]
                                  tbody []
                                      [ for el in n do
                                          tr []
                                              [ td [] [ str (el.Date.ToShortDateString()) ]
                                                td [] [ str el.PurchaseID ]
                                                td [] [ str <| sprintf "R$ %0.2f" el.Value ] ] ]
                                  tfoot []
                                      [ tr []
                                            [ td [ ColSpan 3 ]
                                                  [ let value = n |> List.sumBy (fun e -> e.Value)
                                                    Button.a
                                                        [ Button.IsFullWidth
                                                          Button.OnClick(fun e -> dispatch CloseOpen) ]
                                                        [ str
                                                          <| sprintf "Encerrar periodo e pagar (R$ %0.2f) com paypal"
                                                                 value ] ] ] ] ] ]
              | PanelSub.Closed k ->
                  Panel.block []
                      [ match k with
                        | [] -> str "Nada a mostrar"
                        | n ->
                            Table.table [ Table.IsFullWidth ]
                                [ thead []
                                      [ tr []
                                            [ th [] [ str "Data de abertura" ]
                                              th [] [ str "Data de encerramento" ]
                                              th [] [ str "Pago" ]
                                              th [] [ str "Pagar" ] ] ]
                                  tbody []
                                      [ for el in n do
                                          tr []
                                              [ td [] [ str (el.DateOpen.ToShortDateString()) ]
                                                td [] [ str (el.DateClosed.ToShortDateString()) ]
                                                td []
                                                    [ if el.Paid then str "Sim"
                                                      else
                                                          Button.a
                                                              [ Button.OnClick
                                                                  (fun e -> dispatch (UpdatePay el.PayReference)) ]
                                                              [ str "Não - Atualizar" ] ]
                                                td []
                                                    [ Button.a
                                                        [ Button.Props
                                                            [ Href el.LinkToPay
                                                              Target "_blank" ] ]
                                                          [ str
                                                              (if el.Paid then "Detalhes"
                                                               else "Pagar aqui") ] ] ] ] ] ] ]

type Pages =
    | Login of Login.Model
    | Create of Create.Model
    | Panel of UserPanel.Model


type Model =
    { Page: Pages }

type Msg =
    | LoginMsg of Login.Msg
    | CreateMsg of Create.Msg
    | PanelMsg of UserPanel.Msg



let init(): Model * Cmd<Msg> = { Page = Login(Login.init()) }, Cmd.none

let update (msg: Msg) (currentModel: Model): Model * Cmd<Msg> =
    match currentModel, msg with
    | { Page = Login _ }, LoginMsg Login.Create -> { currentModel with Page = Create(Create.init()) }, Cmd.none
    | { Page = Panel p }, PanelMsg(UserPanel.TokenChanged x) ->
        { currentModel with Page = Panel { p with UserData = { p.UserData with Token = x } } }, Cmd.none
    | { Page = Create _ }, CreateMsg(Create.SignedIn ud)
    | { Page = Login _ }, LoginMsg(Login.LoggedIn ud) ->
        { Page =
              Panel
                  { Error = None
                    UserData = ud
                    Page = PanelSub.Dev } }, Cmd.none
    | { Page = Create c }, CreateMsg msg ->
        let nextModel, cmd = Create.update msg c
        { currentModel with Page = Create nextModel }, Cmd.map CreateMsg cmd
    | { Page = Login l }, LoginMsg msg ->
        let nextModel, cmd = Login.update msg l
        { currentModel with Page = Login nextModel }, Cmd.map LoginMsg cmd
    | { Page = Panel p }, PanelMsg msg ->
        let nextModel, cmd = UserPanel.update msg p
        { currentModel with Page = Panel nextModel }, Cmd.map PanelMsg cmd


let view (model: Model) (dispatch: Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ] [ Navbar.Item.div [] [ Heading.h2 [] [ str "Compras do bem" ] ] ]

          Container.container [ Container.IsFluid ]
              [ match model.Page with
                | Login p -> Login.view p (LoginMsg >> dispatch)
                | Create c -> Create.view c (CreateMsg >> dispatch)
                | Panel p -> UserPanel.view p (PanelMsg >> dispatch) ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withReactBatched "elmish-app"
|> Program.run
