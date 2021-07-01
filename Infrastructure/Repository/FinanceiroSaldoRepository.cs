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
    class FinanceiroSaldoRepository : IFinanceiroSaldoRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public FinanceiroSaldoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(FinanceiroSaldoModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO financeiro_saldo ( ");
            sql.Append("    id_financeiro_conta,");
            sql.Append("    id_anuncio,");
            sql.Append("    valor,");
            sql.Append("    data,");
            sql.Append("    token");
            sql.Append(" VALUES (");
            sql.Append("    @idFinanceiroConta,");
            sql.Append("    @idAnuncio,");
            sql.Append("    @valor,");
            sql.Append("    @data,");
            sql.Append("    @token");
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
            var sql = "DELETE FROM financeiro_saldo WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<FinanceiroSaldoModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    valor,");
            sql.Append("    token");
            sql.Append(" FROM");
            sql.Append("    financeiro_saldo");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroSaldoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<FinanceiroSaldoModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    valor,");
            sql.Append("    token");
            sql.Append(" FROM");
            sql.Append("    financeiro_saldo");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FinanceiroSaldoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FinanceiroSaldoModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    financeiro_saldo");
            sql.Append(" SET");
            sql.Append("     id_financeiro_conta=@idFinanceiroConta,");
            sql.Append("     id_anuncio=@idAnuncio,");
            sql.Append("     valor=@valor,");
            sql.Append("     token=@token");
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

        public async Task<FinanceiroSaldoModel> GetSaldoByIdAnuncioAsync(int idFinanceiroConta, int? idAnuncio)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    valor,");
            sql.Append("    token");
            sql.Append(" FROM");
            sql.Append("    financeiro_saldo");
            sql.Append(" WHERE");
            sql.Append("    id_financeiro_conta = @idFinanceiroConta and");
            if (idAnuncio == null)
            {
                sql.Append("    id_anuncio is null");
            }
            else
            {
                sql.Append("    id_anuncio = @idAnuncio");
            }

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FinanceiroSaldoModel>(Helpers.QBuild(sql), new { idFinanceiroConta = idFinanceiroConta, idAnuncio = idAnuncio });
                return result;
            }
        }

        public async Task<IReadOnlyList<FinanceiroSaldoModel>> GetSaldoByIdContaAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    valor,");
            sql.Append("    token");
            sql.Append(" FROM");
            sql.Append("    financeiro_saldo");
            sql.Append(" WHERE");
            sql.Append("    id_financeiro_conta = @id");
            
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroSaldoModel>(Helpers.QBuild(sql), new { id = id });
                return result.ToList();
            }
        }

        #endregion
    }
}
