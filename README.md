# 🏦 BankMore || O Banco Digital da Ana

### ⚠️ Aviso

O respectivo projeto foi desenvolvido **exclusivamente para fins de avaliação técnica**.  
❌ Não há intenção de uso ou qualquer fim comercial.

Resumidamente, o projeto consiste em um conjunto de API's para um sistema de gerenciamento bancário simplificado, construídas em **.NET 8**, com autenticação **JWT**, consultas em **Oracle PL/SQL** (executadas via **Dapper/ com mapeamentos pelo EF Core**) e arquitetura em camadas (**DDD + CQRS**).

---

### **Requisitos**
  - Precisa-se que o sistema inicial tenha algumas funcionalidades principais:
    - 💹 Cadastro e autenticação de usuários
    - 💹 Realização de movimentações na conta corrente (depósitos e saques)
    - 💹 Transferências entre contas da mesma instituição
    - 💹 Consulta de saldo
   
### **Especificações**

 - **Arquitetura:**
   - 💹 Todos os microsserviços adotem os padrões de **DDD** (*Domain-Driven Design*)
   - 💹 A arquitetura de cada serviço adote o Pattern **CQRS** (*Command Query Responsibility Segregation*)
  
 - **Segurança:**
   - 💹 Todas as APIs devem ser protegidas com autenticação via token (**JWT**). Nenhum endpoint pode ser acessado sem um token válido;
   - 💹 Dados sensíveis como CPF ou número da conta não podem transitar entre os microsserviços ou ser armazenados fora do microsserviço de Usuário.

 - **Qualidade:**
   - 💹 Todas as APIs devem conter um projeto de testes automatizados. (No total, a solução hoje conta com 62 cenários de teste)

 - **Infraestrutura:**
   - O sistema deve ser executado em ambiente de nuvem, com orquestração via Kubernetes.
   - Cada microsserviço precisa rodar com pelo menos 2 réplicas (instâncias)

 - **Crédito:**
   - É importante que a API seja resiliente a falhas, pois o aplicativo que utiliza a API pode perder a conexão com a API antes de receber a resposta e então nestes casos o comportamento é repetir a mesma requisição até que o aplicativo receba um retorno. Por isso os serviços devem ser idempotentes.
   - (OPCIONAL) Seria interessante se fosse possível implementar a API de Tarifas no sistema inicial.

---

## 📋 Funcionalidades

- **Conta Corrente**
  - Cadastro de conta corrente (*com CPF, Nome e Senha*) - *o número da conta é gerado pela aplicação*.
  - Login em conta corrente (*com CPF ou Número da Conta e Senha*) - *gera um token JWT para autenticação nos demais endpoints protegidos*.
  - Consulta de saldo (*créditos - débitos*).

- **Movimentações**
  - Efetuar Débito e crédito em conta corrente.

- **Transferências**
  - Entre contas da mesma instituição.
  - Validação de saldo, contas ativas e valores positivos.
  - Estorno automático em caso de falhas.

- **Autenticação**
  - Geração de **Token JWT**.
  - Uso do token em todas as chamadas protegidas.

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8 / ASP.NET Core Web API**
- **MediatR** → implementação de **CQRS**  
- **Dapper** → consultas no **Oracle PL/SQL**  
- **Entity Framework Core** (para mapeamentos / contexts)  
- **JWT (Json Web Token)** → autenticação  
- **Docker** → conteinerização
- **MSTest / Moq / Sqlite InMemory** → Para testes *automatizados/integração*

---

## 🗂️ Estrutura do Projeto
```
BankMore/
├── BankMore.Account/
│   ├── BankMore.Account.Api/ Camada de entrada (Controllers)
│   ├── BankMore.Account.Application/  Regras de negócio (Handlers, Commands/Queries)
│   ├── BankMore.Account.Domain/  Entidades e Interfaces
│   ├── BankMore.Account.Infrastructure/  Persistência (Repositories, Migrations, Contexts)
│   └── BankMore.Account.Tests/  Testes automatizados (unitários e de integração)
│
├── BankMore.Tariff/
│   ├── BankMore.Tariff.Application/  Regras de negócio (Handlers, Commands/Queries)
│   ├── BankMore.Tariff.Domain/  Entidades e Interfaces
│   ├── BankMore.Tariff.Infrastructure/  Persistência (Repositories, Migrations, Contexts)
│   └── BankMore.Tariff.Worker/  Camada de entrada (Consumers)
│
├── BankMore.Transfer/
│   ├── BankMore.Transfer.Api/  Camada de entrada (Controllers)
│   ├── BankMore.Transfer.Application/  Regras de negócio (Handlers, Commands/Queries)
│   ├── BankMore.Transfer.Domain/  Entidades e Interfaces
│   ├── BankMore.Transfer.Infrastructure/  Persistência (Repositories, Migrations, Contexts)
│   └── BankMore.Transfer.Tests/  Testes automatizados (unitários e de integração)
│
└── README.md  
```

---

## ⚙️ Endpoints Principais

Toda a documentação dos Endpoints também está disponível pelo **Swagger** (*.../swagger*)

---

### 🏦 Conta Corrente

- `POST /api/accounts` → Criação de uma nova conta corrente.
  - Em caso de **sucesso**, retorna um **OK** com o número da conta corrente.
  - `204 No Content` → Sucesso.
  - `400 Bad Request` → Dados inválidos (detalhes no body).
   
- `POST /api/accounts/login` → Efetua o login na conta corrente.
  - Retorna um **token JWT**.
  - `200 OK` → Sucesso, contém o token no response body.
  - `401 Unauthorized` → Informações de Login inválidas.

- `GET /api/accounts/balance` → Consulta o saldo da conta corrente.  
  - Valida token.  
  - Retorna erro em caso de conta inválida/inativa.
- Respostas:
  - `204 No Content` → Sucesso.
  - `400 Bad Request` → Dados inválidos (detalhes no body).
  - `401 Unauthorized` → Informações de Login inválidas.
  - `403 Forbidden` → Token inválido/expirado.

- `POST /api/accounts/movement` → Efetuar uma movimentação na conta corrente.
- Respostas:
  - `204 No Content` → Sucesso.
  - `400 Bad Request` → Dados inválidos (detalhes no body).
  - `401 Unauthorized` → Informações de Login inválidas.
  - `403 Forbidden` → Token inválido/expirado.

---

### 💸 Transferência
`POST /api/transfer` → Efetuar uma transferência entre contas. 
- Regras:
  - Valida token.
  - Apenas contas ativas podem transferir.
  - Apenas valores positivos são aceitos.
  - Débito da conta origem → Crédito na conta destino.
  - Estorno em caso de falha.  
- Respostas:
  - `204 No Content` → Sucesso.
  - `400 Bad Request` → Dados inválidos (detalhes no body).
  - `403 Forbidden` → Token inválido/expirado.

---

## 📊 Banco de Dados

### Tabelas

- **ContaCorrente**
  - `IdContaCorrente` (PK)
  - `Cpf`
  - `Nome`
  - `Senha`
  - `Salt`
  - `Numero`
  - `Ativo`

- **Movimento**
  - `IdMovimento` (PK)
  - `IdContaCorrente` (FK)
  - `Tipo` (`C` = Crédito, `D` = Débito)
  - `Valor`
  - `Data`

- **Transferencia**
  - `IdTransferencia` (PK)
  - `IdContaOrigem`
  - `IdContaDestino`
  - `Valor`
  - `Data`

  - **Tarifa**
  - `IdTarifa` (PK)
  - `IdContaCorrente`
  - `Valor`
  - `DataTarifacao`

---

## 🚀 Executando o Projeto

### 🔧 Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (para rodar os contâiners das aplicações)

### 📥 Clone o repositório
```bash
git clone https://github.com/LipeFW/BankMore.git
cd BankMore
```

## 🐳 Subindo pelo docker-compose

Na pasta raíz do projeto Bankmore rode o comando
docker run -d --name oracle-xe \
```bash
docker compose -f docker-compose.yml up
```
  -p 1521:1521 \
  -e ORACLE_PWD=bankmore123 \
  gvenzl/oracle-xe

O docker-compose deve subir as 2 APIs separadamente, o Worker do serviço de tarifação, e as dependências como Oracle, Kafka e o Zookeper.
**Talvez seja necessário a criação do usuário/scheme BankMore no banco, ou que altere o nome/senha na connection string.

A API Account estará disponível em:

http://localhost:5001

A API Transfer estará disponível em:

http://localhost:5002

## 🧪 Testes

O projeto conta com testes utilizando MSTest.
Para rodar os testes:

dotnet test

## 📌 Exemplos de Uso

Criação de Conta Corrente
POST /api/accounts/login
{
  "cpf": "12345678900", // O **CPF** precisa ser válido (*pode ser inserido com ou sem mascara*).
  "senha": "senha123"
}

Resposta:

{
  "numeroConta": "12345",
}

---

Gerar Token
POST /api/accounts/login
{
  "cpfOrAccountNumber": "12345678900", // pode usar o **CPF** (*com ou sem mascara*) ou o **número da conta**.
  "senha": "senha123"
}

Resposta:

{
  "token": "eyJhbGciOiJIUzI1NiIsInR..."
}

---

Transferência
POST /api/transfer
Header: Authorization: Bearer {token}
{
  "idRequisicao": "uuid",
  "numeroContaDestino": "456789",
  "valor": 150.75
}

Resposta exemplo em caso de erro:

{
  "tipo": "INVALID_ACCOUNT",
  "mensagem": "Conta destino não encontrada."
}

## 👨‍💻 Autor

Desenvolvido por Felipe Weber
🔗 [GitHub](https://github.com/LipeFW/)


