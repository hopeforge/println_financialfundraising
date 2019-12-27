<h1>FINANCIAL FUNDRAISING-API

<h2> HACKATHON GRAAC-PRINTLN 

*Objetivo*
========================================================================
Este projeto tem como objetivo desenvolver uma API para arrecadação de fundos para GRAAC.
A API funcionará em conjunto com sites de e-commerce, onde, durante o checkout do pedido do cliente, será oferecido ao cliente arredondar o preço da compra ligeramente para cima e doar esta quantia para a GRAAC.

A proposta é que após um periodo determinado pela empresa de e-commerce, ela repasse a soma dessas pequenas doações para a GRAAC via uma integração com o Paypal.


*Implementação*
========================================================================
Após a modelagem dos campos necessários foi criada tabelas no banco de dados MySQL que serviram de base para que geradores de código pudessem construir interfaces entre a linguagem e a camada de persistência. O projeto usa uma proteção a alguns endpoints autenticados por meio de um token JWT que pode ser invalidado quando desejado pelo desenvolvedor parceiro. Tanto o front-end e back-end foram programados usando a linguagem F# dentro do ecossistema dotnet core, usando o transpilador de F# para Javascript Fable, juntamente com bibliotecas que são suportadas por React.

 










========================================================================

## APIs públicas
### As seguintes APIs não requerem autenticação:

#### POST - /api/login
- Recebe os dados de usuário e senha e fornece os dados do usuário logado
> Entrada
```fs
    { Email: <email de login>
      Password: <senha do usuário> }
``` 
>Saida
```fs
    { Email: <email de login>
      Name: <nome do usuário>
      CNPJ: <CNPJ do usuário>
      Token: <Token Bearer para as APIs autenticadas> }
```
#### POST - /api/signup
- Recebe os dados de cadastro e fornece os dados do novo usuário logado
> Entrada
```fs
    { Email: <email de login>
      Password: <senha do usuário>
      Name: <nome do usuário>
      CNPJ: <CNPJ do usuário> }
```
> Saida
```fs 
    { Email: <email de login>
      Name: <nome do usuário>
      CNPJ: <CNPJ do usuário>
      Token: <Token Bearer para as APIs autenticadas> }
```
## APIs autenticadas
### As seguintes APIs requerem o token no cabeçalho 'Authorization' com o prefixo 'Bearer':

#### POST - /api/secure/addDonation
- Adiciona os dados da doação feita pelo cliente
> Entrada
```fs
    { Email: <email do doador>
      Name: <Nome do doador>
      CPF: <CPF do doador>
      Value: <Valor da doação:numérico>
      Identifier: <Identificador da compra origem> }
```
> Saida
```
<id da inserção>
```

#### POST - /api/secure/closePeriod
- Fecha o periodo de contabilização e gera fatura para os lançamentos
> Entrada
```
<Confirma a intenção de fechar o período:booleano>
```
> Saida
```
   [{ DateOpen: <Data de abertura:Date>
      DateClosed: <Data de fechamento:Date>
      Paid: <Fatura já paga:booleano>
      PayReference: <Referência para o pagamento>
      LinkToPay: <Link para pagamento> }]
```
#### GET - /api/secure/getClosedInfo
- Recupera todos os lançamentos de faturas
> Saida
```
   [{ DateOpen: <Data de abertura:Date>
      DateClosed: <Data de fechamento:Date>
      Paid: <Fatura já paga:booleano>
      PayReference: <Referência para o pagamento>
      LinkToPay: <Link para pagamento> }]
```
#### GET - /api/secure/getOpenInfo
- Recupera os lançamentos em aberto para pagamento
> Saida
```
   [{ Date: <Data da doação:Date
      PurchaseID: <Identificador da compra origem>
      Value: <Valor da doação:numérico> }]
```
#### POST - /api/secure/updatePay
- Verifica a existência de pagamento para a fatura selecionada
> Entrada
```
{ Id: <Id da mensagem> }
```

> Saida
```
<Verdadeiro caso o pagamento tenha sido realizado>
```

#### GET - /api/secure/renewToken
- Cria um novo token e revoga o anterior
> Saida
```
<novo token>
```
