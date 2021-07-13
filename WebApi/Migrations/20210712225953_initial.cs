using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceCodes",
                columns: table => new
                {
                    UserCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DeviceCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceCodes", x => x.UserCode);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConsumedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Data = table.Column<string>(type: "nvarchar(max)", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersistedGrants", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "regra",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nome = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    normalizedNome = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    concurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regra", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuarioDados = table.Column<int>(type: "int", nullable: true),
                    idEmpresa = table.Column<int>(type: "int", nullable: false),
                    idGrupo = table.Column<int>(type: "int", nullable: false),
                    idUsuarioSituacao = table.Column<int>(type: "int", nullable: false),
                    idArea = table.Column<int>(type: "int", nullable: false),
                    idPais = table.Column<int>(type: "int", nullable: false),
                    idSexo = table.Column<int>(type: "int", nullable: false),
                    idIdioma = table.Column<int>(type: "int", nullable: false),
                    criacao = table.Column<int>(type: "int", nullable: false),
                    atualizacao = table.Column<int>(type: "int", nullable: false),
                    nome = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    senha = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dataExpiracaoSenha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    avatar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    termoUsoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    googleAuthenticatorSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    normalizedLogin = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    normalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    emailConfirmado = table.Column<bool>(type: "bit", nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    securityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    celular = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    celularConfirmado = table.Column<bool>(type: "bit", nullable: false),
                    twoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    lockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    lockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    accessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regraClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    regraId = table.Column<int>(type: "int", nullable: false),
                    claimTipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claimValor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_regraClaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_regraClaims_regra_regraId",
                        column: x => x.regraId,
                        principalTable: "regra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarioClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    claimTipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    claimValor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioClaims", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuarioClaims_usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarioLogin",
                columns: table => new
                {
                    loginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    providerKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    providerDisplayNome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    idUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioLogin", x => new { x.providerKey, x.loginProvider });
                    table.ForeignKey(
                        name: "FK_usuarioLogin_usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarioRegra",
                columns: table => new
                {
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idRegra = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioRegra", x => new { x.idUsuario, x.idRegra });
                    table.ForeignKey(
                        name: "FK_usuarioRegra_regra_idRegra",
                        column: x => x.idRegra,
                        principalTable: "regra",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuarioRegra_usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuarioToken",
                columns: table => new
                {
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    loginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    nome = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarioToken", x => new { x.idUsuario, x.loginProvider, x.nome });
                    table.ForeignKey(
                        name: "FK_usuarioToken_usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCodes_DeviceCode",
                table: "DeviceCodes",
                column: "DeviceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceCodes_Expiration",
                table: "DeviceCodes",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_Expiration",
                table: "PersistedGrants",
                column: "Expiration");

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_ClientId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "ClientId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_PersistedGrants_SubjectId_SessionId_Type",
                table: "PersistedGrants",
                columns: new[] { "SubjectId", "SessionId", "Type" });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "regra",
                column: "normalizedNome",
                unique: true,
                filter: "[normalizedNome] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_regraClaims_regraId",
                table: "regraClaims",
                column: "regraId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "usuario",
                column: "normalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "usuario",
                column: "normalizedLogin",
                unique: true,
                filter: "[normalizedLogin] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioClaims_idUsuario",
                table: "usuarioClaims",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioLogin_idUsuario",
                table: "usuarioLogin",
                column: "idUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_usuarioRegra_idRegra",
                table: "usuarioRegra",
                column: "idRegra");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCodes");

            migrationBuilder.DropTable(
                name: "PersistedGrants");

            migrationBuilder.DropTable(
                name: "regraClaims");

            migrationBuilder.DropTable(
                name: "usuarioClaims");

            migrationBuilder.DropTable(
                name: "usuarioLogin");

            migrationBuilder.DropTable(
                name: "usuarioRegra");

            migrationBuilder.DropTable(
                name: "usuarioToken");

            migrationBuilder.DropTable(
                name: "regra");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
