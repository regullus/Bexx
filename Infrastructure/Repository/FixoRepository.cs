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
    class FixoRepository : IFixoRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public FixoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Custom

        public async Task<IdiomaModel> GetIdiomaByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp,");
            sql.Append("    sigla");
            sql.Append(" FROM");
            sql.Append("    idioma");
            sql.Append(" WHERE");
            sql.Append("    id=@id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<IdiomaModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<IdiomaModel>> GetIdiomaAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp,");
            sql.Append("    sigla");
            sql.Append(" FROM");
            sql.Append("    idioma");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<IdiomaModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<AtivoModel> GetAtivoByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp");
            sql.Append(" FROM");
            sql.Append("    fixo_ativo");
            sql.Append(" WHERE");
            sql.Append("    id=@id");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<AtivoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<AtivoModel>> GetAtivoAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp");
            sql.Append(" FROM");
            sql.Append("    fixo_ativo");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AtivoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
