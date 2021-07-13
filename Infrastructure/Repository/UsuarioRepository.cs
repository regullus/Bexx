#region using

using Interfaces;
using Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using Utils;

#endregion region

namespace Infrastructure.Repository
{
    class UsuarioRepository : IUsuarioRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public UsuarioRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud
        //Não há criação de usuario aqui, quem cria é o identity em WebApi
        //As tabelas não se chamam usuario e sim user por padrao do identity
        //aqui usa em porqugres 'usuario' para manter o padrao no sistema

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM usuario WHERE id = @id";
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<UsuarioModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    idUsuarioDados,");
            sql.Append("    idEmpresa,");
            sql.Append("    idGrupo,");
            sql.Append("    idUsuarioSituacao,");
            sql.Append("    idArea,");
            sql.Append("    idPais,");
            sql.Append("    idSexo,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    email,");
            sql.Append("    login,");
            sql.Append("    senha,");
            sql.Append("    twoFactorEnabled,");            
            sql.Append("    dataExpiracaoSenha,");
            sql.Append("    idIdioma,");
            sql.Append("    avatar,");
            sql.Append("    celular,");
            sql.Append("    celularConfirmado");
            sql.Append(" FROM");
            sql.Append("    usuario");
            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<UsuarioModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    idUsuarioDados,");
            sql.Append("    idEmpresa,");
            sql.Append("    idGrupo,");
            sql.Append("    idUsuarioSituacao,");
            sql.Append("    idArea,");
            sql.Append("    idPais,");
            sql.Append("    idSexo,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    email,");
            sql.Append("    login,");
            sql.Append("    senha,");
            sql.Append("    twoFactorEnabled,");
            sql.Append("    dataExpiracaoSenha,");
            sql.Append("    idIdioma,");
            sql.Append("    avatar,");
            sql.Append("    celular,");
            sql.Append("    celularConfirmado");
            sql.Append(" FROM");
            sql.Append("    usuario");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(UsuarioModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    usuario");
            sql.Append(" SET");
            sql.Append("    idUsuarioDados=@idUsuarioDados,");
            sql.Append("    idEmpresa=@idEmpresa,");
            sql.Append("    idGrupo=@idGrupo,");
            sql.Append("    idUsuarioSituacao=@idUsuarioSituacao,");
            sql.Append("    idArea=@idArea,");
            sql.Append("    idPais=@idPais,");
            sql.Append("    idSexo=@idSexo,");
            sql.Append("    atualizacao=@atualizacao,");
            sql.Append("    nome=@nome,");
            sql.Append("    email=@email,");
            sql.Append("    senha=@senha,");
            sql.Append("    twoFactorEnabled=@doisFatoresHabilitados,");
            sql.Append("    dataExpiracaoSenha=@dataExpiracaoSenha,");
            sql.Append("    idIdioma=@idIdioma,");
            sql.Append("    avatar=@avatar,");
            sql.Append("    celular=@celular,");
            sql.Append("    celularConfirmado=@celularConfirmado");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {

                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteLogicAsync(int id)
        {
            UsuarioModel entity = await GetByIdAsync(id);

            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    usuario");
            sql.Append(" SET");
            sql.Append("    idAnuncioSituacao = 2,"); //2 é Desativado
            sql.Append("    atualizacao=@atualizacao");
            sql.Append(" WHERE");
            sql.Append("    id=@id;");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> UpdateDoisFatoresAsync(DoisFatoresModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    usuario");
            sql.Append(" SET");
            sql.Append("    twoFactorEnabled=@doisFatoresHabilitado,");
            sql.Append("    googleAuthenticatorSecretKey=@autenticadorGoogleChaveSecreta");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<IReadOnlyList<UsuarioListaCrudModel>> GetListaUsuarioCrudAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    u.id,");       
            sql.Append("    u.nome,");         
            sql.Append("    u.login");           
            sql.Append(" FROM");
            sql.Append("    usuario      as u,");
            sql.Append("    usuarioRegra as r");
            sql.Append(" WHERE");
            sql.Append("    u.id = r.idUsuario and");
            sql.Append("    r.idRegra != " + (int)Utils.Perfil.Master );

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioListaCrudModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<UsuarioListaCrudModel>> GetListaUsuarioCrudByPerfilAsync(string perfil)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    u.id,");
            sql.Append("    u.nome,");
            sql.Append("    u.login");
            sql.Append(" FROM");
            sql.Append("    usuario      as u,");
            sql.Append("    usuarioRegra as r");
            sql.Append(" WHERE");
            sql.Append("    u.id = r.idUsuario and");
            sql.Append("    r.idRegra in (" + perfil + ");");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioListaCrudModel>(Helpers.QBuild(sql), new { perfil = perfil });
                return result.ToList();
            }
        }

        #endregion

        #region Custom

        #region EmailConfim

        public async Task<int> EmailConfimAsync(int id)
        {
            //ToDo Verificar se já não esta confirmado, se não confima
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    count(id)");
            sql.Append(" FROM");
            sql.Append("    usuario");
            sql.Append(" WHERE ");
            sql.Append("    id = @id and");
            sql.Append("    emailConfirmado = true");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                var result1 = await connection.QueryFirstAsync<int>(Helpers.QBuild(sql), new { id = id });
                if (result1 < 1)
                {
                    sql.Clear();
                    sql.Append(" UPDATE");
                    sql.Append("    usuario");
                    sql.Append(" SET");
                    sql.Append("    atualizacao = @atualizacao,");
                    sql.Append("    idUsuarioSituacao = 1,");
                    sql.Append("    emailConfirmado = true");
                    sql.Append(" WHERE");
                    sql.Append("    id = @id");

                    var result2 = await connection.ExecuteAsync(Helpers.QBuild(sql), new { id = id, atualizacao = Helpers.DataAtualInt });
                    return result2;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region UsuarioSexo

        public async Task<IReadOnlyList<UsuarioSexoModel>> GetUsuarioSexoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp");
            sql.Append(" FROM");
            sql.Append("    sexo");
            sql.Append(" ORDER BY id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioSexoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

        #region UsuarioPais

        public async Task<UsuarioPaisModel> GetUsuarioPaisByIdUsuarioAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    pais.id,");
            sql.Append("    pais.nome,");
            sql.Append("    pais.nomeIng,");
            sql.Append("    pais.nomeEsp,");
            sql.Append("    pais.nomenternacional,");
            sql.Append("    pais.culture");
            sql.Append(" FROM");
            sql.Append("    usuario usu,");
            sql.Append("    pais pais");
            sql.Append(" WHERE");
            sql.Append("    usu.id = @id and");
            sql.Append("    usu.idPais = pais.id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioPaisModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        #endregion

        #region UsuarioSituacao

        public async Task<IReadOnlyList<UsuarioSituacaoModel>> GetUsuarioSituacaoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp");
            sql.Append(" FROM");
            sql.Append("    usuarioSituacao");
            sql.Append(" ORDER BY id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioSituacaoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

        #region UsuarioPerfil

        public async Task<int> GetUsuarioPerfilAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    idRegra");      
            sql.Append(" FROM");
            sql.Append("    usuarioRegra");
            sql.Append(" WHERE");
            sql.Append("    idUsuario=@id;");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<int>(Helpers.QBuild(sql), new { id = id });

                return result.FirstOrDefault();
            }
        }

        #endregion

        #region UsuarioDados

        public async Task<int> AddUsuarioDadosAsync(UsuarioDadosModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO usuarioDados (");
            sql.Append("    atualizacao,");
            sql.Append("    dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas,");
            sql.Append("    twitch,");
            sql.Append("    facebook,");
            sql.Append("    twitter,");
            sql.Append("    youtube,");
            sql.Append("    instagram,");
            sql.Append("    whatsapp");
            sql.Append("    )");
            sql.Append("VALUES (");
            sql.Append("    @atualizacao,");
            sql.Append("    @dataUltimoAcesso,");
            sql.Append("    @exibeBoasVindas,");
            sql.Append("    @twitch,");
            sql.Append("    @facebook,");
            sql.Append("    @twitter,");
            sql.Append("    @youtube,");
            sql.Append("    @instagram,");
            sql.Append("    @whatsapp");
            sql.Append("    )");
            sql.Append("SELECT SCOPE_IDENTITY()");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> UpdateUsuarioDadosAsync(UsuarioDadosModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    usuarioDados");
            sql.Append(" SET");
            sql.Append("    atualizacao=@atualizacao,");
            sql.Append("    dataUltimoAcesso=@dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas=@exibeBoasVindas,");
            sql.Append("    twitch=@twitch,");
            sql.Append("    facebook=@facebook,");
            sql.Append("    twitter=@twitter,");
            sql.Append("    youtube=@youtube,");
            sql.Append("    instagram=@instagram,");
            sql.Append("    whatsapp=@whatsapp");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<UsuarioDadosModel> GetUsuarioDadosAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas,");
            sql.Append("    twitch,");
            sql.Append("    facebook,");
            sql.Append("    twitter,");
            sql.Append("    youtube,");
            sql.Append("    instagram,");
            sql.Append("    whatsapp");
            sql.Append(" FROM");
            sql.Append("    usuarioDados");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioDadosModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        #endregion

        #endregion

    }
}
