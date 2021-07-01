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
    class BaseRepository : IBaseRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public BaseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Custom

        public async Task<IReadOnlyList<InstituicaoFinanceiraModel>> GetInstituicaoFinanceiraAsync(int ativo)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    codigo,");
            sql.Append("    descricao");
            sql.Append(" FROM");
            sql.Append("    base_instituicao_financeira");
            sql.Append(" WHERE");
            sql.Append("    id_ativo = " + ativo);
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<InstituicaoFinanceiraModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<PaisModel>> GetPaisAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    base_pais");
            sql.Append(" ORDER BY nome");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PaisModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
