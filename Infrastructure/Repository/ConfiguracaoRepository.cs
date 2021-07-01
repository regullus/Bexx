#region using

using Interfaces;
using Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
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
            sql.Append("    id_empresa,");
            sql.Append("    chave,");
            sql.Append("    valor,");
            sql.Append("    descricao,");
            sql.Append("    valor_ing,");
            sql.Append("    descricao_ing,");
            sql.Append("    valor_esp,");
            sql.Append("    descricao_esp,");
            sql.Append("    atualizacao)");
            sql.Append(" VALUES (");
            sql.Append("    @id_empresa,");
            sql.Append("    @chave,");
            sql.Append("    @valor,");
            sql.Append("    @descricao,");
            sql.Append("    @valor_ing,");
            sql.Append("    @descricao_ing,");
            sql.Append("    @valor_esp,");
            sql.Append("    @descricao_esp,");
            sql.Append("    @atualizacao");
            sql.Append("    )");
            sql.Append(" RETURNING id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM configuracao WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
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
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    chave,");
            sql.Append("    valor,");
            sql.Append("    descricao,");
            sql.Append("    valor_ing as valorIng,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    valor_esp as valorEsp,");
            sql.Append("    descricao_esp as descricaoEsp,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    configuracao");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
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
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    chave,");
            sql.Append("    valor,");
            sql.Append("    descricao,");
            sql.Append("    valor_ing as valorIng,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    valor_esp as valorEsp,");
            sql.Append("    descricao_esp as descricaoEsp,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    configuracao");
            sql.Append(" WHERE");
            sql.Append("    id = @id");
         
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
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
            sql.Append("    id_empresa=@idEmpresa,");
            sql.Append("    chave=@chave,");
            sql.Append("    valor=@valor,");
            sql.Append("    descricao=@descricao,");
            sql.Append("    valor_ing=@valorIng,");
            sql.Append("    descricao_ing=@descricaoIng,");
            sql.Append("    valor_esp=@valorEsp,");
            sql.Append("    descricao_esp=@descricaoEsp,");
            sql.Append("    atualizacao=@atualizacao");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
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
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    configuracao_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<ConfiguracaoTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
