# API Raizes do Nordeste

Este projeto é uma API para apoio ao gerenciamento de uma lanchonete com temática nordestina. Um sistema desenvolvido para organizar clientes, unidades, produtos e pedidos em um sistema simples, usando tecnologias comuns no desenvolvimento backend com .NET.

## Funcionalidades

- Cadastro e login de clientes com JWT
- Cadastro e consulta de unidades
- Cadastro e consulta de produtos
- Criacao de pedidos
- Consulta de pedidos
- Atualizacao de status do pedido
- Programa de fidelidade basico
- Relatorios gerenciais de vendas, produtos, clientes e estoque
- Validacao dos dados enviados para a API
- Documentacao dos endpoints com Swagger

## Tecnologias usadas

- .NET 10
- ASP.NET Core
- Entity Framework Core
- SQL Server
- FluentValidation
- Swagger
- JWT Bearer
- xUnit
- Moq

## Estrutura do projeto

```text
Controllers/   Recebem as requisicoes da API
Services/      Contem as regras principais do sistema
Repositories/  Fazem o acesso aos dados
Data/          Configuracao do banco de dados
Models/        Entidades do sistema
DTOs/          Dados de entrada e saida da API
Validators/    Regras de validacao
Tests/         Testes automatizados
```

## Como executar

Restaure as dependencias:

```bash
dotnet restore
```

Execute o projeto:

```bash
dotnet run
```

Depois de iniciar a API, acesse o Swagger pelo navegador:

```text
http://localhost:5000
```

Dependendo da configuracao local, a porta pode ser diferente. Nesse caso, confira a URL exibida no terminal ao executar o projeto.

## Autenticacao

O cadastro e o login retornam um token JWT. Nos endpoints protegidos, envie:

```text
Authorization: Bearer SEU_TOKEN
```

No Swagger, use o botao **Authorize** e informe o token. A chave configurada em
`Jwt:Key` serve apenas para desenvolvimento e deve ser substituida em producao,
preferencialmente pela variavel de ambiente `Jwt__Key`.

## Banco de dados

A string de conexao esta no arquivo `appsettings.json`.

Antes de rodar em outro ambiente, e necessario ajustar os dados do SQL Server conforme a maquina utilizada.

## Testes

Para executar os testes:

```bash
dotnet test Tests/ApiRaizesNordeste.Tests.csproj
```

Os testes cobrem principalmente validacoes e algumas regras de servico.

## Relatorios

Os endpoints aceitam filtros opcionais de periodo (`dataInicio` e `dataFim`) e
unidade (`unidadeId`). Quando o periodo nao e informado, sao considerados os
ultimos 30 dias. O faturamento considera somente pedidos entregues.

```text
GET /api/v1/relatorios/vendas/resumo
GET /api/v1/relatorios/produtos/mais-vendidos
GET /api/v1/relatorios/clientes/maiores-compradores
GET /api/v1/relatorios/estoque/baixo
```

## Observacoes

Algumas partes foram mantidas de forma simples de proposito. Recursos como
pagamento online, perfis avancados de permissao, renovacao de token e cache
ficaram fora do escopo.

Essas melhorias podem ser adicionadas em uma evolucao futura do projeto.
