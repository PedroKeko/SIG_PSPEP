# SIG_PSPEP

Sistema de Gestão Integrada desenvolvido em **ASP.NET Core**, com arquitetura modular e backend em **C#**, Razor Pages no frontend, EF Core e Identity para autenticação.

---

## 🚀 Objetivo do Projeto

O SIG_PSPEP tem como objetivo **gerir processos e informações administrativas e operacionais** de forma centralizada e segura, com foco em:

- Modularidade via Areas (Admin, DI, Dpq, Dss, Dt, Dtti)
- Separação de responsabilidades (Controllers, Services, Entidades)
- Backend limpo e escalável
- Segurança com políticas e Identity
- Persistência com EF Core e Migrations

---

## 🛠 Tecnologias utilizadas

- **Backend:** C# (.NET 8 / ASP.NET Core)  
- **Frontend:** Razor Pages + JavaScript  
- **Banco de dados:** SQL Server + EF Core Migrations  
- **Segurança:** Policies, Identity Authentication  
- **Controle de versão:** Git + GitHub  
- **Dependências:** SixLabors.ImageSharp, etc.  

---

## 📂 Estrutura do Projeto

/Areas
/Admin
/DI
/Dpq
/Dss
/Dt
/Dtti
/Context
/Controllers
/Dtti-server
/Entidades
/Enums
/Migrations
/Models
/Policies
/Services
/Views
Program.cs
appsettings.json

**Descrição das pastas principais:**

- **Areas:** módulos da aplicação separados por responsabilidades  
- **Context:** DbContext do EF Core  
- **Controllers:** endpoints / páginas Razor  
- **Services:** regras de negócio, lógica separada do controller  
- **Entidades:** classes de domínio  
- **Policies:** regras de autorização  
- **Migrations:** histórico de banco  

---

## ⚡ Como rodar o projeto

1. Clone o repositório:
```bash
git clone https://github.com/PedroKeko/SIG_PSPEP.git

