🛡️ NIST-CSF
Aplicação desenvolvida para auxiliar organizações na gestão e acompanhamento da maturidade em Segurança da Informação, com base no NIST Cybersecurity Framework (CSF).

📘 Sumário
Visão Geral

Arquitetura do Projeto

Principais Funcionalidades

Estrutura de Pastas

Requisitos e Instalação

Configuração e Execução

Testes

Docker e Banco de Dados Oracle

Swagger e Documentação da API

Endpoints e API

Boas Práticas e Segurança

Como Contribuir

Roadmap Futuro

Licença

Referências

🧩 Visão Geral
O NIST-CSF é uma aplicação em .NET 8 (C#) que digitaliza o processo de avaliação e maturidade em segurança da informação, seguindo o NIST Cybersecurity Framework.

🎯 Objetivos principais:
Digitalizar a avaliação de aderência ao NIST CSF.

Acompanhar a evolução da maturidade organizacional.

Gerar relatatórios e dashboards.

Oferecer API documentada em Swagger.

🏗️ Arquitetura do Projeto
Arquitetura MVC (Model–View–Controller) com:

Controllers

Services

Models

Views

wwwroot

Docker

Swagger

⚙️ Principais Funcionalidades
Gestão dos controles NIST

Avaliação de maturidade

Relatórios e dashboards

API REST com Swagger

Banco Oracle XE via Docker

Suíte de testes automatizados

📁 Estrutura de Pastas
text
NIST-CSF/
 ├── Controllers/
 ├── Models/
 ├── Services/
 ├── Views/
 ├── wwwroot/
 ├── Tests/
 │   ├── UnitTests/
 │   ├── IntegrationTests/
 │   └── TestData/
 ├── Docker/
 │   ├── docker-compose.yml
 │   └── init-scripts/
 │       └── init.sql
 ├── appsettings.json
 ├── Program.cs
 ├── NistXGH.csproj
 └── NistXGH.sln
💻 Requisitos e Instalação
.NET 8+

Docker Desktop

VS Code ou Visual Studio

bash
git clone https://github.com/antoniobr844/NIST-CSF.git
cd NIST-CSF
dotnet restore
🚀 Configuração e Execução
Edite appsettings.Development.json:

json
{
  "ConnectionStrings": {
    "SgsiDbContext": "User Id=system;Password=oracle;Data Source=localhost:1521/XEPDB1"
  }
}
Execute:

bash
dotnet run
Acesse:

text
http://localhost:5000
🧪 Testes
Estrutura de Testes
O projeto inclui uma suíte abrangente de testes para garantir a qualidade do código:

UnitTests: Testes unitários para serviços e lógica de negócio

IntegrationTests: Testes de integração com banco de dados e APIs

TestData: Dados mockados para testes

Executando os Testes
Executar Todos os Testes
bash
# Executa todos os testes do projeto
dotnet test

# Executa com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
Executar Testes Específicos
bash
# Executar apenas testes unitários
dotnet test --filter Category=Unit

# Executar apenas testes de integração
dotnet test --filter Category=Integration

# Executar testes por nome
dotnet test --filter "FullyQualifiedName~ControleServiceTests"
Executar com Relatório de Cobertura
bash
# Instalar ferramenta de cobertura (se necessário)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test --settings coverlet.runsettings --logger "trx;LogFileName=test-results.trx"

# Gerar relatório HTML
reportgenerator -reports:./**/coverage.cobertura.xml -targetdir:./coverage-report -reporttypes:Html
Testes em Ambiente Docker
bash
# Executar testes em container isolado
docker-compose -f docker-compose.test.yml up --build --abort-on-container-exit
Tipos de Testes Implementados
Testes Unitários
csharp
// Exemplo: Teste do serviço de controles
[Fact]
public void Deve_Retornar_Controles_Por_Categoria()
{
    // Arrange
    var mockRepo = new Mock<IControleRepository>();
    var service = new ControleService(mockRepo.Object);
    
    // Act
    var result = service.ObterControlesPorCategoria("Identify");
    
    // Assert
    Assert.NotNull(result);
    Assert.All(result, c => Assert.Equal("Identify", c.Categoria));
}
Testes de Integração
csharp
[Collection("DatabaseCollection")]
public class AvaliacaoIntegrationTests
{
    [Fact]
    public async Task Deve_Criar_Avaliacao_No_Banco()
    {
        // Arrange & Act
        var avaliacao = await CriarAvaliacaoTeste();
        
        // Assert
        Assert.True(avaliacao.Id > 0);
        Assert.Equal("Avaliação Teste", avaliacao.Nome);
    }
}
Configuração de Testes
Arquivo appsettings.Testing.json
json
{
  "ConnectionStrings": {
    "SgsiDbContext": "User Id=test_user;Password=test123;Data Source=localhost:1521/XEPDB1"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  }
}
Docker Compose para Testes
yaml
version: '3.9'
services:
  oracle-test:
    image: gvenzl/oracle-xe:21-slim
    environment:
      - ORACLE_PASSWORD=test123
      - APP_USER=test_user
      - APP_USER_PASSWORD=test123
    ports:
      - "1522:1521"
    volumes:
      - ./TestData/init-test.sql:/container-entrypoint-initdb.d/init-test.sql
Boas Práticas de Teste
Nomenclatura Clara:

[Método]_[Cenário]_[ResultadoEsperado]

Exemplo: SalvarAvaliacao_ComDadosValidos_DeveRetornarSucesso

Arrange-Act-Assert:

csharp
// Arrange
var input = new AvaliacaoInput { Nome = "Teste" };

// Act
var result = await service.SalvarAvaliacao(input);

// Assert
Assert.True(result.Sucesso);
Testes Independentes: Cada teste deve ser independente e não depender de estado anterior

Mock de Dependências: Use mocks para serviços externos e repositórios

Relatórios e Métricas
Cobertura de Código: Meta mínima de 80%

Testes Passando: Todos os testes devem passar no build

Relatório HTML: Gerado automaticamente no CI/CD

🐳 Docker e Banco de Dados Oracle
yaml
version: '3.9'
services:
  oracle-db:
    image: gvenzl/oracle-xe:21-slim
    container_name: oracle-nist
    environment:
      - ORACLE_PASSWORD=oracle
      - APP_USER=system
      - APP_USER_PASSWORD=oracle
    ports:
      - "1521:1521"
    volumes:
      - ./init-scripts:/container-entrypoint-initdb.d
Subir o container:

bash
docker-compose up -d
📚 Swagger e Documentação da API
Acesse:

text
http://localhost:5000/swagger
🔌 Endpoints e API
Método	Endpoint	Descrição
GET	/api/controles	Lista controles
GET	/api/avaliacoes/{id}	Retorna avaliação
POST	/api/avaliacoes	Cria avaliação
PUT	/api/avaliacoes/{id}	Atualiza avaliação
DELETE	/api/avaliacoes/{id}	Remove avaliação
🔒 Boas Práticas e Segurança
HTTPS

Autenticação por papéis

Logs de auditoria

Backup do Oracle

LGPD

🤝 Como Contribuir
bash
git checkout -b feature/nova-funcionalidade
git commit -m "Implementa funcionalidade X"
git push origin feature/nova-funcionalidade
Importante: Certifique-se de que todos os testes passem antes do push:

bash
dotnet test
🧭 Roadmap Futuro
JWT

Dashboards dinâmicos

ISO 27001

Testes (xUnit)

CI/CD

Docker completo

📄 Licença
Licença MIT.

🧠 Referências
NIST Cybersecurity Framework

Oracle XE

.NET 8 Docs

Swagger UI

xUnit Testing Framework

