namespace Shared

type Produto =
    { Nome: string
      Valor: decimal
      Descricao: string
      Imagem: string option }

type Item = Doacao of decimal | Produto of (Produto * int)

type Cliente =
    { Nome: string
      Email: string
      CPF: string }

type Compra =
    { Cliente: Cliente
      Produtos: Item list }

module Route =
    let builder typeName methodName = sprintf "/store/api/%s/%s" typeName methodName

type ILojaApi =
    { CarregaProdutos: unit -> Async<Produto list>
      CriaCliente: string -> string -> string -> Async<Cliente>
      EnviaCompra: Compra -> Async<bool>}
