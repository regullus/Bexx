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
    class FinanceiroContaRepository : IFinanceiroContaRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public FinanceiroContaRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(FinanceiroContaModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO financeiro_conta( ");
            sql.Append("    id_usuario,");
            sql.Append("    id_financeiro_situacao,");
            sql.Append("    id_moeda,");
            sql.Append("    id_financeiro_conta_tipo,");
            sql.Append("    nome,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    atualizacao)");
            sql.Append(" VALUES (");
            sql.Append("    @idUsuario,");
            sql.Append("    @idFinanceiroSituacao,");
            sql.Append("    @idMoeda,");
            sql.Append("    @idFinanceiroContaTipo,");
            sql.Append("    @nome,");
            sql.Append("    @token,");
            sql.Append("    @observacao,");
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
            var sql = "DELETE FROM financeiro_conta WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<FinanceiroContaModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_financeiro_situacao as idFinanceiroSituacao,");
            sql.Append("    id_moeda as idMoeda,");
            sql.Append("    id_financeiro_conta_tipo as idFinanceiroContaTipo,");
            sql.Append("    nome,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    financeiro_conta");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroContaModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<FinanceiroContaModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_financeiro_situacao as idFinanceiroSituacao,");
            sql.Append("    id_moeda as idMoeda,");
            sql.Append("    id_financeiro_conta_tipo as idFinanceiroContaTipo,");
            sql.Append("    nome,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    financeiro_conta");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FinanceiroContaModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FinanceiroContaModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    financeiro_conta");
            sql.Append(" SET");
            sql.Append("     id_usuario=@idUsuario,");
            sql.Append("     id_financeiro_situacao=@idFinanceiroSituacao,");
            sql.Append("     id_moeda=@idMoeda,");
            sql.Append("     id_financeiro_conta_tipo=@idFinanceiroContaTipo,");
            sql.Append("     nome=@nome,");
            sql.Append("     token=@token,");
            sql.Append("     observacao=@observacao,");
            sql.Append("     atualizacao=@atualizacao");
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

        public async Task<IReadOnlyList<FinanceiroContaTipoModel>> GetFinanceiroContaTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    financeiro_conta_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroContaTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<FinanceiroSituacaoModel>> GetFinanceiroSituacaoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    financeiro_situacao");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroSituacaoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<FinanceiroMoedaModel>> GetFinanceiroMoedaAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    sigla,");
            sql.Append("    simbolo,");
            sql.Append("    nome,");
            sql.Append("    mascara,");
            sql.Append("    mascara_out as mascaraOut,");
            sql.Append("    mascara_input as mascaraInput");
            sql.Append(" FROM");
            sql.Append("    financeiro_moeda");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroMoedaModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }
        public async Task<FinanceiroMoedaModel> GetFinanceiroMoedaByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    sigla,");
            sql.Append("    simbolo,");
            sql.Append("    nome,");
            sql.Append("    mascara,");
            sql.Append("    mascara_out as mascaraOut,");
            sql.Append("    mascara_input as mascaraInput");
            sql.Append(" FROM");
            sql.Append("    financeiro_moeda");
            sql.Append(" WHERE");
            sql.Append("    id=@id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FinanceiroMoedaModel>(Helpers.QBuild(sql), new { id = id });           
                return result;
            }
        }

        public async Task<FinanceiroContaModel> GetByIdUsuarioAsync(int idUsuario)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_financeiro_situacao as idFinanceiroSituacao,");
            sql.Append("    id_moeda as idMoeda,");
            sql.Append("    id_financeiro_conta_tipo as idFinanceiroContaTipo,");
            sql.Append("    nome,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    financeiro_conta");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FinanceiroContaModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario });
                return result;
            }
        }

        #endregion
    }
}
