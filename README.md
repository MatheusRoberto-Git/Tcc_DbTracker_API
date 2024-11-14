**Projeto de TCC - API em .NET 8**

**Descrição**

> Este é um projeto de uma API desenvolvida em .NET 8, que tem como principal funcionalidade conectar-se a um banco de dados e retornar os schemas desse banco de dados. A API foi projetada para ser fácil de usar, com endpoints que fornecem informações sobre as tabelas, colunas e outros metadados do banco de dados.

**Estrutura do Projeto**

O projeto é composto pelas seguintes camadas:

- API: Contém os endpoints que permitem o acesso aos dados do banco.

- Controller: Responsável pela lógica de negócio e pela comunicação com o repositório.

- Repositório: Gerencia as consultas ao banco de dados.

**Tecnologias Utilizadas**

- .NET 8: Framework de desenvolvimento.

- Entity Framework Core: Para a interação com o banco de dados.

- SQL Server: Banco de dados utilizado.

- Docker: Para facilitar a conteinerização e o deploy.

**Instalação e Configuração**

1. Clone o repositório:
```bash
git clone https://github.com/usuario/projeto-tcc-api.git
cd projeto-tcc-api
```

2. Instale as dependências:
Certifique-se de ter o .NET 8 instalado. Para instalar as dependências do projeto, execute:

```bash
dotnet restore
```

3. Configuração do banco de dados:
Atualize o arquivo appsettings.json com a string de conexão adequada ao seu ambiente:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR;Database=SEU_BANCO;User Id=SEU_USUARIO;Password=SUA_SENHA;"
}
```

4. Execute as migrações (se necessário):

```bash
dotnet ef database update
```

5. Inicie a API:
```bash
dotnet run
```
**Endpoints Principais**

- GET /dbtracker/getAllTables: Retorna a lista de schemas do banco de dados.

- GET /dbtracker/getAllTables/structures: Retorna as strucs de um schema específico.

- GET /dbtracker/{nomeTabela}/structure: Retorna as strucs de uma tabela especifica