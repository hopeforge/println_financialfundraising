
SELECT * FROM hackatonFinancialFundraising.LOJA;






ALTER table LOJATOKENTOKEN
ADD foreign key (ID_PERIODO)
references PERIODO(ID_PERIODO);

INSERT INTO LOJA(ID_LOJA,NOME_LOJA,CNPJ,EMAIL,SENHA,TOKEN,ID_PERIODO) VALUES (2,'Stonks Pizza','000000000','stonks@pizza.dino','stonks','pi',2);


INSERT INTO LOJA (ID_LOJA,NOME_LOJA,CNPJ,EMAIL,SENHA,TOKEN,ID_PERIODO)
VALUES(5,'Stonks Papercraft','111111111','stonks@paper.dino','paper','pp',2);


INSERT INTO LOJA (ID_LOJA,NOME_LOJA,CNPJ,EMAIL,SENHA,TOKEN,ID_PERIODO)
VALUES(3,'Stonks Burguer','222222222','stonks@burguer.dino','hamburguer','hb',4);


INSERT INTO LOJA (ID_LOJA,NOME_LOJA,CNPJ,EMAIL,SENHA,TOKEN,ID_PERIODO)
VALUES(4,'Stonks Phones','333333333333','stonks@phones.dino','celphone','cp',5);
