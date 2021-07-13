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
    class TokenRefreshRepository : ITokenRefreshRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public TokenRefreshRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(TokenRefreshModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO user_refresh_token (");
            sql.Append("    id_ativo,");
            sql.Append("    id_usuario,");
            sql.Append("    id_jwt,");
            sql.Append("    token,");
            sql.Append("    revogado,");
            sql.Append("    data_adicao,");
            sql.Append("    data_expiracao)");
            sql.Append(" VALUES (");
            sql.Append("    @idAtivo,");
            sql.Append("    @idUsuario,");
            sql.Append("    @idJWT,");
            sql.Append("    @token,");
            sql.Append("    @revogado,");
            sql.Append("    @dataAdicao,");
            sql.Append("    @dataExpiracao");
            sql.Append("    )");
            sql.Append("SELECT SCOPE_IDENTITY()");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM user_refresh_token WHERE id = @id";
            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<TokenRefreshModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_jwt as idJWT,");
            sql.Append("    token,");
            sql.Append("    revogado,");
            sql.Append("    data_adicao as dataAdicao,");
            sql.Append("    data_expiracao as dataExpiracao");
            sql.Append(" FROM");
            sql.Append("    user_refresh_token");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TokenRefreshModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<TokenRefreshModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_jwt as idJWT,");
            sql.Append("    token,");
            sql.Append("    revogado,");
            sql.Append("    data_adicao as dataAdicao,");
            sql.Append("    data_expiracao as dataExpiracao");
            sql.Append(" FROM");
            sql.Append("    user_refresh_token");
            sql.Append(" WHERE");
            sql.Append("    id = @id and");
            sql.Append("    id_ativo = 1"); //1-ativo

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TokenRefreshModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(TokenRefreshModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    user_refresh_token");
            sql.Append(" SET");
            sql.Append("     id_ativo=@idAtivo,");
            sql.Append("     id_usuario=@idUsuario,");
            sql.Append("     id_jwt=@idJWT,");
            sql.Append("     token=@token,");
            sql.Append("     revogado=@revogado,");
            sql.Append("     data_adicao=@dataAdicao,");
            sql.Append("     data_expiracao=@dataExpiracao");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        #endregion

        #region Custom

        public async Task<TokenRefreshModel> GetByIdUsusarioAsync(int idUsuario)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_jwt as idJWT,");
            sql.Append("    token,");
            sql.Append("    revogado,");
            sql.Append("    data_adicao as dataAdicao,");
            sql.Append("    data_expiracao as dataExpiracao");
            sql.Append(" FROM");
            sql.Append("    user_refresh_token");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario AND");
            sql.Append("    id_ativo = 1"); //1-ativo
            sql.Append(" ORDER BY id DESC");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryFirstAsync<TokenRefreshModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario });
                return result;
            }
        }

        #endregion

    }
}
