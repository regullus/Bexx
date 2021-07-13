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

        public async Task<IReadOnlyList<PaisModel>> GetPaisAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    nomeIng,");
            sql.Append("    nomeEsp");
            sql.Append(" FROM");
            sql.Append("    pais");
            sql.Append(" ORDER BY nome");

            using (var connection = new  SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PaisModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
