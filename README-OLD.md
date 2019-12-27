**HACKATHON GRAAC**
========================================================================

Parabens!!!

Sua equipe esta participando do Hackathon beneficiente para a Graac!


Foi disponibilizado um ambiente de desenvolvimento onde o acesso deverá ser 
realizado através da VPN.


**Seus dados de acesso são:**

    IP:         172.31.7.66
    USUÁRIO:    ubuntu
    SENHA:      N9sdhpi2Dhn2
    
    Para acesso externo à sua API desenvolvida, utilize o endereço: http://54.183.222.128



Você possui permissão de root (Administrador), para isso execute:

**$** sudo su -



Voce pode desenvolver utilizando os recursos abaixo:
========================================================================
- PHP 7.2.24
- Java 8 (OpenJDK 1.8.0)
- NodeJS v8.10.0
- Python 3.6.9
  > Obs.: Utilizar o comando "pip3"
- Python 2.7.15+
  > Utilizar o comando "pip"
- Docker / Docker Compose
- Maven

Banco de dados
========================================================================
Como acessar o client do Postgres:

    $ sudo -i -u postgres
    $ psql

Como acessar o client do MySQL:

    $ mysql -u root -p
    SENHA: N9sdhpi2Dhn2
    Acesso Web: http://<IP_AMBIENTE>/phpmyadmin/

Como acessar o client do mongodb:

    $ mongo --eval 'db.runCommand({ connectionStatus: 1 })'

Como acessar o Elasticsearch:

    $ curl -u elastic:changeme -XGET 'http://localhost:9200/'
    Usuário: elastic     Senha: changeme
