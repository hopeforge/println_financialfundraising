module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fulma
open Thoth.Json

open Shared
open Fulma.Extensions.Wikiki
open Fable.Core.JsInterop
open Fable.FontAwesome

importSideEffects "bulma-pageloader"
importSideEffects "bulma-quickview/dist/css/bulma-quickview.min.css"
importSideEffects "bulma-quickview/dist/js/bulma-quickview.min.js"


type Model =
    { Produtos: Produto list option
      Carrinho: (Produto * int) list * bool
      Doacao: decimal option
      Cliente: Cliente option
      TelaLogin: bool
      CompraRealizada: bool }

type Msg =
    | CarregaProdutos of Produto list
    | AdicionaProduto of (Produto * int * bool)
    | RemoveProduto of Produto
    | AdicionaCliente of Cliente
    | DeslogaCliente of bool
    | AdicionaDoacao of decimal
    | RemoveDoacao
    | AbreLogin
    | EncerraCompra
    | ResetaCompra of bool
    | EstadoCarrinho



module Server =

    open Shared
    open Fable.Remoting.Client

    /// A proxy you can use to talk to server directly
    let api: ILojaApi =
        Remoting.createApi()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<ILojaApi>

let carregaProdutos = Server.api.CarregaProdutos

// defines the initial state and initial command (= side-effect) of the application
let init(): Model * Cmd<Msg> =
    let modelInicial =
        { Produtos = None
          Carrinho = [], false
          Doacao = None
          TelaLogin = false
          Cliente = None
          CompraRealizada = false }

    let carregaProdutosCmd = Cmd.OfAsync.perform carregaProdutos () CarregaProdutos
    modelInicial, carregaProdutosCmd

let addProduto (p, n, s) (carr, b) =
    let rec inner l added =
        function
        | [] ->
            (if added then l
             else (p, n) :: l)
            |> List.rev
        | (px, nx) :: r when p = px ->
            inner
                ((px,
                  (if s then nx + n
                   else max 1 n))
                 :: l) true r
        | px :: s -> inner (px :: l) added s
    (inner [] false carr, b)

let getImgSrc { Imagem = s } =
    s
    |> Option.defaultValue "https://dummyimage.com/128x128/7a7a7a/fff"
    |> Src

let update (msg: Msg) (currentModel: Model): Model * Cmd<Msg> =
    match currentModel.Produtos, msg with
    | Some produtos, AdicionaProduto p ->
        let nextModel =
            { currentModel with
                  Doacao = None
                  Carrinho = currentModel.Carrinho |> addProduto p }
        nextModel, Cmd.none
    | Some _, RemoveProduto p ->
        let { Carrinho = (nextCarrinho, e) } = currentModel
        { currentModel with Carrinho = nextCarrinho |> List.filter (fun (e, _) -> e <> p), e }, Cmd.none
    | Some produtos, EstadoCarrinho ->
        let { Carrinho = (nextCarrinho, e) } = currentModel
        { currentModel with Carrinho = nextCarrinho, not e }, Cmd.none
    | Some produtos, AdicionaDoacao d -> { currentModel with Doacao = Some d }, Cmd.none
    | _, CarregaProdutos produtos ->
        let nextModel = { currentModel with Produtos = Some produtos }
        nextModel, Cmd.none
    | _, DeslogaCliente true -> { currentModel with TelaLogin = false }, Cmd.none
    | _, DeslogaCliente false ->
        { currentModel with
              TelaLogin = false
              Cliente = None }, Cmd.none
    | _, AbreLogin ->
        { currentModel with
              TelaLogin = not currentModel.TelaLogin
              Cliente =
                  Some
                      { Nome = ""
                        CPF = ""
                        Email = "" } }, Cmd.none
    | _, AdicionaCliente cl -> { currentModel with Cliente = Some cl }, Cmd.none
    | _, EncerraCompra ->
        match currentModel.Cliente with
        | Some cl ->
            let carrinho =
                fst currentModel.Carrinho
                |> List.fold (fun a x -> Produto x :: a)
                       [ if currentModel.Doacao.IsSome then Doacao currentModel.Doacao.Value ]

            let cmd =
                Cmd.OfAsync.perform Server.api.EnviaCompra
                    { Cliente = cl
                      Produtos = carrinho } ResetaCompra

            currentModel, cmd
        | None -> currentModel, Cmd.ofMsg AbreLogin
    | _, ResetaCompra true ->
        let m, c = init()
        { m with Cliente = currentModel.Cliente }, c
    | _, ResetaCompra false -> currentModel, Cmd.ofMsg EstadoCarrinho
    | _ -> currentModel, Cmd.none

let modal active carrinho v doacao dispatch =
    Modal.modal [ Modal.IsActive(active && (carrinho <> [])) ]
        [ Modal.background [ Props [ OnClick ignore ] ] []
          Modal.Card.card []
              [ Modal.Card.head []
                    [ Modal.Card.title [] [ str "Carrinho" ]
                      Delete.delete [ Delete.OnClick(fun _ -> dispatch EstadoCarrinho) ] [] ]
                Modal.Card.body []
                    [ Modal.Card.body []
                          [ Panel.panel []
                                [ for (p, n) in carrinho do
                                    Panel.block
                                        [ Panel.Block.Modifiers [ Modifier.Display(Screen.All, Display.Flex) ]
                                          Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]
                                        [ Level.level [ Level.Level.Props [ Style [ Width "100%" ] ] ]
                                              [ Level.left []
                                                    [ Image.image [ Image.Is32x32 ] [ img [ getImgSrc p ] ] ]
                                                Level.item [ Level.Item.HasTextCentered ]
                                                    [ Field.div [ Field.Props [ Style [ Width "100%" ] ] ]
                                                          [ Panel.block
                                                              [ Panel.Block.Modifiers
                                                                  [ Modifier.Display(Screen.All, Display.Flex) ]
                                                                Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]
                                                                [ str p.Nome ]
                                                            Panel.block
                                                                [ Panel.Block.Modifiers
                                                                    [ Modifier.Display(Screen.All, Display.Flex) ]
                                                                  Panel.Block.Props
                                                                      [ Style [ JustifyContent "center" ] ] ]
                                                                [ Field.div [ Field.IsGroupedCentered ]
                                                                      [ Icon.icon
                                                                          [ Icon.Props
                                                                              [ OnClick
                                                                                  (fun e ->
                                                                                  dispatch
                                                                                  <| if n <= 1 then RemoveProduto p
                                                                                     else (AdicionaProduto(p, -1, true))) ]
                                                                            Icon.IsLeft ] [ Fa.i [ Fa.Solid.Minus ] [] ]
                                                                        Control.p []
                                                                            [ Input.number
                                                                                [ Input.OnChange
                                                                                    (fun e ->
                                                                                    dispatch
                                                                                        (AdicionaProduto
                                                                                            (p, int e.Value, false)))
                                                                                  Input.Modifiers
                                                                                      [ Modifier.TextSize
                                                                                          (Screen.All, TextSize.Is7) ]
                                                                                  Input.ValueOrDefault(string n) ] ]
                                                                        Icon.icon
                                                                            [ Icon.Props
                                                                                [ OnClick
                                                                                    (fun e ->
                                                                                    dispatch
                                                                                        (AdicionaProduto(p, 1, true))) ]
                                                                              Icon.IsRight ]
                                                                            [ Fa.i [ Fa.Solid.Plus ] [] ] ] ] ] ]
                                                Level.right [] [ str (sprintf "R$ %0.2f" (decimal n * p.Valor)) ] ] ] ] ]
                      Modal.Card.body []
                          [ match carrinho with
                            | [] -> str "Adicione produtos ao carrinho"
                            | _ ->

                                Panel.panel []
                                    [ Panel.block [ Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]
                                          [ str "Doação GRAACC" ]
                                      Panel.checkbox [ Panel.Block.IsActive true ]
                                          [ match doacao with
                                            | Some d ->
                                                Container.container []
                                                    [ sprintf "Obrigado pela sua doação de R$ %0.2f!" d |> str ]
                                            | None ->
                                                let x = floor (v + 1m)
                                                let y = x - v
                                                Container.container
                                                    [ Container.Props
                                                        [ OnClick(fun _ -> AdicionaDoacao y |> dispatch) ] ]
                                                    [ str
                                                      <| sprintf
                                                          "Clique aqui para doar R$ %0.2f e arredondar sua compra para R$ %0.2f!"
                                                             y x ] ] ] ] ]
                Modal.Card.foot []
                    [ Button.a
                        [ Button.IsFullWidth
                          Button.IsStatic true ] [ str (sprintf "Total da compra: R$ %0.2f" v) ]
                      Button.a
                          [ Button.OnClick(fun _ -> dispatch EncerraCompra)
                            Button.IsFullWidth ] [ str "Finalizar compra" ] ] ] ]

let modalLogin v cli d f =
    Modal.modal [ Modal.IsActive v ]
        [ Modal.background [] [ Modal.close [ Modal.Close.OnClick(fun _ -> f false) ] [] ]
          Modal.content []
              [ Modal.Card.body []
                    [ Panel.panel []
                          [ Panel.heading [] [ str "Dados pessoais" ]
                            Panel.block []
                                [ Input.input
                                    [ Input.Placeholder "Nome"
                                      Input.ValueOrDefault cli.Nome
                                      Input.OnChange(fun e -> d { cli with Cliente.Nome = e.Value }) ] ]
                            Panel.block []
                                [ Input.input
                                    [ Input.Placeholder "CPF"
                                      Input.ValueOrDefault cli.CPF
                                      Input.OnChange(fun e -> d { cli with Cliente.CPF = e.Value }) ] ]
                            Panel.block []
                                [ Input.input
                                    [ Input.Placeholder "Email"
                                      Input.ValueOrDefault cli.Email
                                      Input.OnChange(fun e -> d { cli with Cliente.Email = e.Value }) ] ]
                            Panel.block [] [ Button.a [ Button.OnClick(fun _ -> f true) ] [ str "Salvar" ] ] ] ] ] ]

let view (model: Model) (dispatch: Msg -> unit) =
    div []
        [ PageLoader.pageLoader [ PageLoader.IsActive model.Produtos.IsNone ]
              [ str "Aguarde, estamos carregando nossos produtos" ]
          Navbar.navbar [ Navbar.Color IsPrimary ] [ Navbar.Item.div [] [ Heading.h2 [] [ str "Lojinha do bem" ] ] ]
          let v =
              model.Carrinho
              |> fst
              |> List.fold (fun v (p, n) -> v + (p.Valor * decimal n)) (model.Doacao |> Option.defaultValue 0m)

          modal (snd model.Carrinho) (fst model.Carrinho) v model.Doacao dispatch
          match model.Cliente with
          | Some cliente ->
              modalLogin model.TelaLogin cliente (AdicionaCliente >> dispatch) (DeslogaCliente >> dispatch)
          | None -> ()
          Container.container []
              [ Columns.columns [ Columns.IsMultiline ]
                    [ match model.Produtos with
                      | Some ps ->
                          for p in ps do
                              yield Column.column [ Column.Width(Screen.All, Column.Is4) ]
                                        [ Panel.panel
                                            [ Panel.Modifiers
                                                [ Modifier.TextAlignment(Screen.All, TextAlignment.Centered) ] ]
                                              [ Panel.heading []
                                                    [ Heading.h4 [] [ str p.Nome ]
                                                      Heading.h6 [ Heading.IsSubtitle ]
                                                          [ str (sprintf "R$ %0.2f" p.Valor) ] ]
                                                Panel.block
                                                    [ Panel.Block.Modifiers
                                                        [ Modifier.Display(Screen.All, Display.Flex) ]
                                                      Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]

                                                    [ Image.image [ Image.Is128x128 ] [ img [ getImgSrc p ] ] ]
                                                Panel.block [ Panel.Block.Props [ Style [ JustifyContent "center" ] ] ]
                                                    [ str p.Descricao ]
                                                Panel.block []
                                                    [ Button.a
                                                        [ Button.IsFullWidth
                                                          Button.OnClick
                                                              (fun _ -> AdicionaProduto(p, 1, true) |> dispatch) ]
                                                          [ let n =
                                                              fst model.Carrinho
                                                              |> List.tryFind (fst >> ((=) p))
                                                              |> Option.map snd
                                                            match n with
                                                            | None -> str "Comprar"
                                                            | Some n -> str (sprintf "Comprar x%i" n) ] ] ] ]
                      | None -> () ]
                Button.a
                    [ Button.IsFullWidth
                      Button.IsStatic true ] [ str (sprintf "Total da compra: R$ %0.2f" v) ]
                if v > 0m then
                    Button.a
                        [ Button.IsFullWidth
                          Button.Color IsSuccess
                          Button.OnClick(fun _ -> dispatch EstadoCarrinho) ] [ str "Finalizar compra" ] ]


          Footer.footer []
              [ Content.content [ Content.Modifiers [ Modifier.TextAlignment(Screen.All, TextAlignment.Centered) ] ]
                    [ str "Obrigado por comprar com a gente!" ] ] ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.withConsoleTrace
|> Program.withReactBatched "elmish-app"
|> Program.run
