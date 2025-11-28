# 🛡️ NIST-CSF
Aplicação desenvolvida para auxiliar organizações na **gestão e acompanhamento da maturidade em Segurança da Informação**, com base no **NIST Cybersecurity Framework (CSF)**.

## 📘 Sumário
- [Visão Geral](#🧩-visão-geral)
- [Arquitetura do Projeto](#🏗️-arquitetura-do-projeto)
- [Principais Funcionalidades](#⚙️-principais-funcionalidades)
- [Estrutura de Pastas](#📁-estrutura-de-pastas)
- [Requisitos e Instalação](#💻-requisitos-e-instalação)
- [Configuração e Execução](#🚀-configuração-e-execução)
- [Docker e Banco de Dados Oracle](#🐳-docker-e-banco-de-dados-oracle)
- [Swagger e Documentação da API](#📚-swagger-e-documentação-da-api)
- [Endpoints e API](#🔌-endpoints-e-api)
- [Boas Práticas e Segurança](#🔒-boas-práticas-e-segurança)
- [Como Contribuir](#🤝-como-contribuir)
- [Roadmap Futuro](#🧭-roadmap-futuro)
- [Licença](#📄-licença)
- [Referências](#🧠-referências)

## 🧩 Visão Geral
O **NIST-CSF** é uma aplicação em **.NET 8 (C#)** que digitaliza o processo de avaliação e maturidade em segurança da informação, seguindo o **NIST Cybersecurity Framework**.

### 🎯 Objetivos principais:
- Digitalizar a avaliação de aderência ao NIST CSF.  
- Acompanhar a evolução da maturidade organizacional.  
- Gerar relatatórios e dashboards.  
- Oferecer API documentada em Swagger.  

## 🏗️ Arquitetura do Projeto
Arquitetura **MVC (Model–View–Controller)** com:
- Controllers
- Services
- Models
- Views
- wwwroot
- Docker
- Swagger

## ⚙️ Principais Funcionalidades
- Gestão dos controles NIST  
- Avaliação de maturidade  
- Relatórios e dashboards  
- API REST com Swagger  
- Banco Oracle XE via Docker  

## 📁 Estrutura de Pastas
```
NIST-CSF/
 ├── Controllers/
 ├── Models/
 ├── Services/
 ├── Views/
 ├── wwwroot/
 ├── Docker/
 │   ├── docker-compose.yml
 │   └── init-scripts/
 │       └── init.sql
 ├── appsettings.json
 ├── Program.cs
 ├── NistXGH.csproj
 └── NistXGH.sln
```

## 💻 Requisitos e Instalação
- .NET 8+
- Docker Desktop
- VS Code ou Visual Studio

```bash
git clone https://github.com/antoniobr844/NIST-CSF.git
cd NIST-CSF
dotnet restore
```

## 🚀 Configuração e Execução
Edite `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "SgsiDbContext": "User Id=system;Password=oracle;Data Source=localhost:1521/XEPDB1"
  }
}
```

Execute:
```bash
dotnet run
```

Acesse:
```
http://localhost:5000
```

## 🐳 Docker e Banco de Dados Oracle
```yaml
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
```

Subir o container:
```bash
docker-compose up -d
```

## 📚 Swagger e Documentação da API
Acesse:
```
http://localhost:5000/swagger
```

## 🔌 Endpoints e API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET    | /api/controles           | Lista controles |
| GET    | /api/avaliacoes/{id}     | Retorna avaliação |
| POST   | /api/avaliacoes          | Cria avaliação |
| PUT    | /api/avaliacoes/{id}     | Atualiza avaliação |
| DELETE | /api/avaliacoes/{id}     | Remove avaliação |

## 🔒 Boas Práticas e Segurança
- HTTPS  
- Autenticação por papéis  
- Logs de auditoria  
- Backup do Oracle  
- LGPD  

## 🤝 Como Contribuir
```bash
git checkout -b feature/nova-funcionalidade
git commit -m "Implementa funcionalidade X"
git push origin feature/nova-funcionalidade
```

## 🧭 Roadmap Futuro
- JWT  
- Dashboards dinâmicos  
- ISO 27001  
- Testes (xUnit)  
- CI/CD  
- Docker completo  

## 📄 Licença
Licença **MIT**.

## 🧠 Referências
- NIST Cybersecurity Framework  
- Oracle XE  
- .NET 8 Docs  
- Swagger UI  
