using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIG_PSPEP.Migrations
{
    /// <inheritdoc />
    public partial class _Inicial_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescricaoArea = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bancos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeBanco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bancos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bancos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EstadoEfectividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEstadoEfect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescricaoEstadoEfect = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoEfectividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstadoEfectividades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FuncaoCargos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeFuncaoCargo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncaoCargos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuncaoCargos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrdemServicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumOrdemServico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnoOrdemServico = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdemServicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdemServicos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrgaoUnidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeOrgaoUnidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sigla = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgaoUnidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrgaoUnidades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Patentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Posto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Classe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patentes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SituacaoEfectivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoSituacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SituacaoEfectivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SituacaoEfectivos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Efectivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SituacaoEfectivoId = table.Column<int>(type: "int", nullable: false),
                    OrgaoUnidadeId = table.Column<int>(type: "int", nullable: false),
                    FuncaoCargoId = table.Column<int>(type: "int", nullable: false),
                    PatenteId = table.Column<int>(type: "int", nullable: false),
                    Num_Processo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NIP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    N_Agente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apelido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Genero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DataNasc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoCivil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GSanguineo = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    NumBI = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BIValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BIEmitido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumCartaConducao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CartaValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CartaEmitido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumPassaporte = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PassapValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PassapEmitido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nacionalidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Naturalidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MunicipioRes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Destrito_BairroRes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Rua = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CasaNum = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Habilitacao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CursoHabilitado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InstitAcademica = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefone1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Telefone2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DataIngresso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoVinculo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Carreira = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UnidadeOrigem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OutrasInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Efectivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Efectivos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Efectivos_FuncaoCargos_FuncaoCargoId",
                        column: x => x.FuncaoCargoId,
                        principalTable: "FuncaoCargos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Efectivos_OrgaoUnidades_OrgaoUnidadeId",
                        column: x => x.OrgaoUnidadeId,
                        principalTable: "OrgaoUnidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Efectivos_Patentes_PatenteId",
                        column: x => x.PatenteId,
                        principalTable: "Patentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Efectivos_SituacaoEfectivos_SituacaoEfectivoId",
                        column: x => x.SituacaoEfectivoId,
                        principalTable: "SituacaoEfectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgregadoFams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EfectivoId = table.Column<int>(type: "int", nullable: false),
                    NomeAgregado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrauParentesco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNasc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgregadoFams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgregadoFams_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgregadoFams_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfectivoContaBancarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EfectivoId = table.Column<int>(type: "int", nullable: false),
                    BancoId = table.Column<int>(type: "int", nullable: false),
                    NumeroConta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfectivoContaBancarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EfectivoContaBancarias_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EfectivoContaBancarias_Bancos_BancoId",
                        column: x => x.BancoId,
                        principalTable: "Bancos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EfectivoContaBancarias_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfectivoEstadoEfectividades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EfectivoId = table.Column<int>(type: "int", nullable: false),
                    EstadoEfectividadeId = table.Column<int>(type: "int", nullable: false),
                    DescricaoEstadoEfectividade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfectivoEstadoEfectividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EfectivoEstadoEfectividades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EfectivoEstadoEfectividades_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EfectivoEstadoEfectividades_EstadoEfectividades_EstadoEfectividadeId",
                        column: x => x.EstadoEfectividadeId,
                        principalTable: "EstadoEfectividades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfectivoOrdemServicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EfectivoId = table.Column<int>(type: "int", nullable: false),
                    OrdemServicoId = table.Column<int>(type: "int", nullable: false),
                    PatenteId = table.Column<int>(type: "int", nullable: false),
                    NumDespacho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnoPromocao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoEfectividadeId = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfectivoOrdemServicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EfectivoOrdemServicos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EfectivoOrdemServicos_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EfectivoOrdemServicos_EstadoEfectividades_EstadoEfectividadeId",
                        column: x => x.EstadoEfectividadeId,
                        principalTable: "EstadoEfectividades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EfectivoOrdemServicos_Patentes_PatenteId",
                        column: x => x.PatenteId,
                        principalTable: "Patentes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Filiacaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EfectivoId = table.Column<int>(type: "int", nullable: false),
                    Pai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApelidoPai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NacionalidadePai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaturalidadePai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mae = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApelidoMae = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NacionalidadeMae = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NaturalidadeMae = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filiacaos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Filiacaos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Filiacaos_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FotoEfectivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EfectivoId = table.Column<int>(type: "int", nullable: false),
                    Foto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUltimaAlterecao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoEfectivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotoEfectivos_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FotoEfectivos_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioAutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    EfectivoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioAutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioAutes_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioAutes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UsuarioAutes_Efectivos_EfectivoId",
                        column: x => x.EfectivoId,
                        principalTable: "Efectivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgregadoFams_EfectivoId",
                table: "AgregadoFams",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgregadoFams_UserId",
                table: "AgregadoFams",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Areas_UserId",
                table: "Areas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bancos_UserId",
                table: "Bancos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoContaBancarias_BancoId",
                table: "EfectivoContaBancarias",
                column: "BancoId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoContaBancarias_EfectivoId",
                table: "EfectivoContaBancarias",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoContaBancarias_UserId",
                table: "EfectivoContaBancarias",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoEstadoEfectividades_EfectivoId",
                table: "EfectivoEstadoEfectividades",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoEstadoEfectividades_EstadoEfectividadeId",
                table: "EfectivoEstadoEfectividades",
                column: "EstadoEfectividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoEstadoEfectividades_UserId",
                table: "EfectivoEstadoEfectividades",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoOrdemServicos_EfectivoId",
                table: "EfectivoOrdemServicos",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoOrdemServicos_EstadoEfectividadeId",
                table: "EfectivoOrdemServicos",
                column: "EstadoEfectividadeId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoOrdemServicos_PatenteId",
                table: "EfectivoOrdemServicos",
                column: "PatenteId");

            migrationBuilder.CreateIndex(
                name: "IX_EfectivoOrdemServicos_UserId",
                table: "EfectivoOrdemServicos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Efectivos_FuncaoCargoId",
                table: "Efectivos",
                column: "FuncaoCargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Efectivos_OrgaoUnidadeId",
                table: "Efectivos",
                column: "OrgaoUnidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Efectivos_PatenteId",
                table: "Efectivos",
                column: "PatenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Efectivos_SituacaoEfectivoId",
                table: "Efectivos",
                column: "SituacaoEfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Efectivos_UserId",
                table: "Efectivos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EstadoEfectividades_UserId",
                table: "EstadoEfectividades",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Filiacaos_EfectivoId",
                table: "Filiacaos",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Filiacaos_UserId",
                table: "Filiacaos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FotoEfectivos_EfectivoId",
                table: "FotoEfectivos",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_FotoEfectivos_UserId",
                table: "FotoEfectivos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FuncaoCargos_UserId",
                table: "FuncaoCargos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdemServicos_UserId",
                table: "OrdemServicos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgaoUnidades_UserId",
                table: "OrgaoUnidades",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Patentes_UserId",
                table: "Patentes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SituacaoEfectivos_UserId",
                table: "SituacaoEfectivos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAutes_AreaId",
                table: "UsuarioAutes",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAutes_EfectivoId",
                table: "UsuarioAutes",
                column: "EfectivoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioAutes_UserId",
                table: "UsuarioAutes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgregadoFams");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "EfectivoContaBancarias");

            migrationBuilder.DropTable(
                name: "EfectivoEstadoEfectividades");

            migrationBuilder.DropTable(
                name: "EfectivoOrdemServicos");

            migrationBuilder.DropTable(
                name: "Filiacaos");

            migrationBuilder.DropTable(
                name: "FotoEfectivos");

            migrationBuilder.DropTable(
                name: "OrdemServicos");

            migrationBuilder.DropTable(
                name: "UsuarioAutes");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Bancos");

            migrationBuilder.DropTable(
                name: "EstadoEfectividades");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Efectivos");

            migrationBuilder.DropTable(
                name: "FuncaoCargos");

            migrationBuilder.DropTable(
                name: "OrgaoUnidades");

            migrationBuilder.DropTable(
                name: "Patentes");

            migrationBuilder.DropTable(
                name: "SituacaoEfectivos");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
