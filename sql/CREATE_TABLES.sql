create schema hackatonFinancialFundraising;
use hackatonFinancialFundraising;

DROP schema hackatonFinancialFundraising;

create table LOJA(
 ID_LOJA INT not null AUTO_INCREMENT,
 NOME_LOJA varchar(255) not null,
 CNPJ varchar(14) not null,
 EMAIL varchar(100) not null,
 SENHA varchar(100) not null,
 TOKEN varchar(255) not null,
 ID_PERIODO int ,
 PRIMARY KEY (ID_LOJA)

);

create table DOADOR(
 ID_DOADOR INT NOT NULL AUTO_INCREMENT,
 NOME_DOADOR varchar(255)not null,
 CPF varchar(11),
 EMAIL varchar(255),
 SENHA VARCHAR(100),
 PRIMARY KEY (ID_DOADOR)
 );
 
create table DOACOES(
 ID_DOACAO INT NOT NULL AUTO_INCREMENT,
 NUM_PEDIDO varchar(255)not null,
 DATA_DOACAO datetime,
 VALOR DOUBLE(10,2),
 ID_DOADOR INT NOT NULL,
 ID_LOJA INT NOT NULL,
 ID_PERIODO INT NOT NULL,
 PRIMARY KEY (ID_DOACAO)
);

 
create table PERIODO(
 ID_PERIODO INT NOT NULL AUTO_INCREMENT,
 ABERTURA DATETIME NOT NULL,
 FECHAMENTO DATETIME,
 PAGO BOOLEAN NOT NULL,
 LINK_PAGAMENTO VARCHAR(255),
 ID_LOJA INT ,
 PRIMARY KEY (ID_PERIODO)
 );
ALTER TABLE DOACOES
 ADD foreign key (ID_DOADOR)
  references DOADOR(ID_DOADOR);

ALTER TABLE DOACOES
 ADD foreign key (ID_LOJA)
  references LOJA(ID_LOJA);

ALTER TABLE DOACOES
ADD foreign key (ID_PERIODO)
references PERIODO(ID_PERIODO);

ALTER TABLE LOJA
ADD foreign key (ID_PERIODO)
references PERIODO(ID_PERIODO);

ALTER TABLE PERIODO
ADD foreign key (ID_LOJA)
references LOJA (ID_LOJA);
