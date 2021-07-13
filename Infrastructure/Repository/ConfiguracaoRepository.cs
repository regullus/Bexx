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
    class ConfiguracaoRepository : IConfiguracaoRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public ConfiguracaoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(ConfiguracaoModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO configuracao(");
            sql.Append("    idEmpresa,");
            sql.Append("    chave,");
            sql.Append("    valor,");
            sql.Append("    descricao,");
            sql.Append("    valorIng,");
            sql.Append("    descricao_ing,");
            sql.Append("    valorEsp,");
            sql.Append("    descricao_esp,");
            sql.Append("    atualizacao)");
            sql.Append(" VALUES (");
            sql.Append("    @idEmpresa,");
            sql.Append("    @chave,");
            sql.Append("    @valor,");
            sql.Append("    @descricao,");
            sql.Append("    @valorIng,");
            sql.Append("    @descricao_ing,");
            sql.Append("    @valorEsp,");
            sql.Append("    @descricao_esp,");
            sql.Append("    @atualizacao");
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
            var sql = "DELETE FROM configuracao WHERE id = @id";
            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<ConfiguracaoModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    idEmpresa,");
            sql.Append("    chave,");
            sql.Append("    valor,");
            sql.Append("    descricao,");
            sql.Append("    valorIng as valorIng,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    valorEsp as valorEsp,");
            sql.Append("    descricao_esp as descricaoEsp,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    configuracao");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ConfiguracaoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<ConfiguracaoModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    idEmpresa,");
            sql.Append("    chave,");
            sql.Append("    valor,");
            sql.Append("    descricao,");
            sql.Append("    valorIng as valorIng,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    valorEsp as valorEsp,");
            sql.Append("    descricao_esp as descricaoEsp,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    configuracao");
            sql.Append(" WHERE");
            sql.Append("    id = @id");
         
            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<ConfiguracaoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(ConfiguracaoModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    configuracao");
            sql.Append(" SET");
            sql.Append("    idEmpresa=@idEmpresa,");
            sql.Append("    chave=@chave,");
            sql.Append("    valor=@valor,");
            sql.Append("    descricao=@descricao,");
            sql.Append("    valorIng=@valorIng,");
            sql.Append("    descricao_ing=@descricaoIng,");
            sql.Append("    valorEsp=@valorEsp,");
            sql.Append("    descricao_esp=@descricaoEsp,");
            sql.Append("    atualizacao=@atualizacao");
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
        public async Task<IReadOnlyList<ConfiguracaoTipoModel>> GetConfiguracaoTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp");
            sql.Append(" FROM");
            sql.Append("    configuracao_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ConfiguracaoTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
