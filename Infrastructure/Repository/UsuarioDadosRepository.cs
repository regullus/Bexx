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

namespace Infrastructure.Repository
{
    public class UsuarioDadosRepository : IUsuarioDadosRepository
    {
        #region Crud

        private readonly IConfiguration configuration;
        public UsuarioDadosRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<int> AddAsync(UsuarioDadosModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO ");
            sql.Append("    usuarioDados (");
            sql.Append("    atualizacao,");
            sql.Append("    dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas,");
            sql.Append("    twitch,");
            sql.Append("    facebook,");
            sql.Append("    twitter,");
            sql.Append("    youtube,");
            sql.Append("    instagram");
            sql.Append("    )");
            sql.Append("VALUES (");
            sql.Append("    @atualizacao,");
            sql.Append("    @dataUltimoAcesso,");
            sql.Append("    @exibeBoasVindas,");
            sql.Append("    @twitch,");
            sql.Append("    @facebook,");
            sql.Append("    @twitter,");
            sql.Append("    @youtube,");
            sql.Append("    @instagram");
            sql.Append("    )");
            sql.Append("SELECT SCOPE_IDENTITY()");
            
            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM usuarioDados WHERE id = @Id";
            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<UsuarioDadosModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    atualizacao,");
            sql.Append("    dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas,");
            sql.Append("    twitch,");
            sql.Append("    facebook,");
            sql.Append("    twitter,");
            sql.Append("    youtube,");
            sql.Append("    instagram");
            sql.Append(" FROM");
            sql.Append("    usuarioDados");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioDadosModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<UsuarioDadosModel> GetByIdAsync(int id)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    atualizacao,");
            sql.Append("    dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas,");
            sql.Append("    twitch,");
            sql.Append("    facebook,");
            sql.Append("    twitter,");
            sql.Append("    youtube,");
            sql.Append("    instagram");
            sql.Append(" FROM");
            sql.Append("    usuarioDados");
            sql.Append(" WHERE id = @Id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioDadosModel>(Helpers.QBuild(sql), new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(UsuarioDadosModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    usuarioDados");
            sql.Append(" SET");
            sql.Append("    atualizacao = @atualizacao,");
            sql.Append("    dataUltimoAcesso = @dataUltimoAcesso,");
            sql.Append("    exibeBoasVindas = @exibeBoasVindas,");
            sql.Append("    twitch = @twitch,");
            sql.Append("    facebook = @facebook,");
            sql.Append("    twitter = @twitter,");
            sql.Append("    youtube = @youtube,");
            sql.Append("    instagram = @instagram");
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

        #endregion
    }
}
