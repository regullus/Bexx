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
    class FinanceiroLancamentoRepository : IFinanceiroLancamentoRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public FinanceiroLancamentoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(FinanceiroLancamentoModel entity)
        {
            entity.criacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO financeiro_lancamento(");
            sql.Append("    id_financeiro_conta,");
            sql.Append("    id_financeiro_lancamento_tipo,");
            sql.Append("    id_anuncio,");
            sql.Append("    id_referencia,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    data,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    criacao)");
            sql.Append("    id_usuario)");
            sql.Append("    )");
            sql.Append(" VALUES (");
            sql.Append("    @idFinanceiroConta,");
            sql.Append("    @idFinanceiroLancamentoTipo,");
            sql.Append("    @idAnuncio,");
            sql.Append("    @idReferencia,");
            sql.Append("    @descricao,");
            sql.Append("    @valor,");
            sql.Append("    @data,");
            sql.Append("    @token,");
            sql.Append("    @observacao,");
            sql.Append("    @criacao,");
            sql.Append("    @idUsuario");
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
            var sql = "DELETE FROM financeiro_lancamento WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<FinanceiroLancamentoModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_financeiro_lancamento_tipo as idFinanceiroLancamentoTipo,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_referencia as idReferencia,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    data,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    criacao,");
            sql.Append("    id_usuario as idUsuario");
            sql.Append(" FROM");
            sql.Append("    financeiro_lancamento");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroLancamentoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<FinanceiroLancamentoModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_financeiro_lancamento_tipo as idFinanceiroLancamentoTipo,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_referencia as idReferencia,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    data,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    criacao,");
            sql.Append("    id_usuario as idUsuario");
            sql.Append(" FROM");
            sql.Append("    financeiro_lancamento");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FinanceiroLancamentoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FinanceiroLancamentoModel entity)
        {
            entity.criacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    financeiro_lancamento");
            sql.Append(" SET");
            sql.Append("     id_financeiro_conta=@idFinanceiroConta,");
            sql.Append("     id_financeiro_lancamento_tipo=@idFinanceiroLancamentoTipo,");
            sql.Append("     id_anuncio=@idAnuncio,");
            sql.Append("     id_referencia=@idReferencia,");
            sql.Append("     descricao=@descricao,");
            sql.Append("     valor=@valor,");
            sql.Append("     data=@data,");
            sql.Append("     token=@token,");
            sql.Append("     observacao=@observacao,");
            sql.Append("     criacao=@criacao,");
            sql.Append("     id_usuario=@idUsuario");
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
        public async Task<int> AddWithSaldoAsync(FinanceiroLancamentoModel entity)
        {
            entity.criacao = Helpers.DataAtualInt;
            //Insert na tabela financeiro_lancamento
            StringBuilder sql1 = new StringBuilder();
            sql1.Append(" INSERT INTO financeiro_lancamento(");
            sql1.Append("    id_financeiro_conta,");
            sql1.Append("    id_financeiro_lancamento_tipo,");
            sql1.Append("    id_anuncio,");
            sql1.Append("    id_referencia,");
            sql1.Append("    descricao,");
            sql1.Append("    valor,");
            sql1.Append("    data,");
            sql1.Append("    token,");
            sql1.Append("    observacao,");
            sql1.Append("    criacao,");
            sql1.Append("    id_usuario");
            sql1.Append("    )");
            sql1.Append(" VALUES (");
            sql1.Append("    @idFinanceiroConta,");
            sql1.Append("    @idFinanceiroLancamentoTipo,");
            sql1.Append("    @idAnuncio,");
            sql1.Append("    @idReferencia,");
            sql1.Append("    @descricao,");
            sql1.Append("    @valor,");
            sql1.Append("    @data,");
            sql1.Append("    @token,");
            sql1.Append("    @observacao,");
            sql1.Append("    @criacao,");
            sql1.Append("    @idUsuario");
            sql1.Append("    )");
            sql1.Append(" RETURNING id;");
     
            //Update na tabela financeiro_saldo
            StringBuilder sql2 = new StringBuilder();
            sql2.Append(" UPDATE financeiro_saldo");
            sql2.Append(" SET");
            sql2.Append("    valor = valor + @valor,");
            sql2.Append("    data  = @data");
            sql2.Append(" WHERE");
            sql2.Append("    id_financeiro_conta = @idFinanceiroConta and");
            if (entity.idAnuncio != null)
                sql2.Append("    id_anuncio = @idAnuncio;");
            else
                sql2.Append("    id_anuncio is null;");

            //Insert na tabela financeiro_saldo 
            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" INSERT INTO financeiro_saldo (");
            sql3.Append("    id_financeiro_conta,");
            sql3.Append("    id_anuncio,");
            sql3.Append("    valor,");
            sql3.Append("    data,");
            sql3.Append("    token)");
            sql3.Append(" VALUES (");
            sql3.Append("    @idFinanceiroConta,");
            sql3.Append("    @idAnuncio,");
            sql3.Append("    @valor,");
            sql3.Append("    @data,");
            sql3.Append("    ''");
            sql3.Append("    )");
            sql3.Append(" RETURNING id;");

            //Verifica se existe registro em financeiro_saldo
            StringBuilder sql4 = new StringBuilder();
            sql4.Append(" SELECT");
            sql4.Append("    COUNT(1) as retorno");
            sql4.Append(" FROM financeiro_saldo");
            sql4.Append("    financeiro_saldo");
            sql4.Append(" WHERE");
            sql4.Append("    id_financeiro_conta = @idFinanceiroConta and");
            if (entity.idAnuncio != null)
                sql4.Append("    id_anuncio = @idAnuncio;");
            else
                sql4.Append("    id_anuncio is null;");
  
            int resultTabFinanceiroLancamento = 0;
            int resultTabFinanceiroSaldo = 0;

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //   Verifica se já existe o registro de saldo
                var registroSaldo = await connection.QueryFirstAsync<int>(Helpers.QBuild(sql4), new { idFinanceiroConta = entity.idFinanceiroConta, idAnuncio = entity.idAnuncio });

                using (var trans = connection.BeginTransaction())
                {
                    try
                    {                       
                        //Insert na tabela financeiro_lancamento
                        resultTabFinanceiroLancamento = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql1), entity);
                        //Insert na tabela financeiro_saldo
                        entity.data = DateTime.Now;
                        if (registroSaldo > 0)
                        {
                            //Update sql3
                            resultTabFinanceiroSaldo = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql2), entity);
                        }
                        else
                        {
                            //Insert sql4
                            resultTabFinanceiroSaldo = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql3), entity);
                        }                       

                        //Dando tudo certo, comita
                        await trans.CommitAsync();
                    }
                    catch (Exception)
                    {
                        //Erro! ToDo LogErro
                        await trans.RollbackAsync();
                        throw;
                    }
                }
            }
            return resultTabFinanceiroLancamento;
        }

        public async Task<IReadOnlyList<FinanceiroLancamentoModel>> GetLancamentoByIdContaAsync(int id, int? qtdeDias)
        {
            DateTime data = DateTime.Now;
            if (qtdeDias != null && qtdeDias > 0)
                data = DateTime.Now.AddDays( - (int)qtdeDias);

            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_financeiro_conta as idFinanceiroConta,");
            sql.Append("    id_financeiro_lancamento_tipo as idFinanceiroLancamentoTipo,");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_referencia as idReferencia,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    data,");
            sql.Append("    token,");
            sql.Append("    observacao,");
            sql.Append("    criacao,");
            sql.Append("    id_usuario as idUsuario");
            sql.Append(" FROM");
            sql.Append("    financeiro_lancamento");
            sql.Append(" WHERE");
            sql.Append("    id_financeiro_conta = @id");
            if (qtdeDias != null && qtdeDias > 0)
                sql.Append("    and data >= @data");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroLancamentoModel>(Helpers.QBuild(sql), new { id = id, data = data });
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<FinanceiroLancamentoTipoModel>> GetFinanceiroLancamentoTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    financeiro_lancamento_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FinanceiroLancamentoTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion
    }
}
