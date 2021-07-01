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
    class TicketsRepository : ITicketsRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public TicketsRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(TicketsModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO tickets (");
            sql.Append("    id_usuario,");
            sql.Append("    origem,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    ticket,");
            sql.Append("    criacao,");
            sql.Append("    atualizacao,");
            sql.Append("    status");
            sql.Append("    )");
            sql.Append(" VALUES (");
            sql.Append("    @idUsuario,");
            sql.Append("    @origem,");
            sql.Append("    @descricao,");
            sql.Append("    @valor,");
            sql.Append("    @ticket,");
            sql.Append("    @criacao,");
            sql.Append("    @atualizacao,");
            sql.Append("    @status");
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
            var sql = "DELETE FROM tickets WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<TicketsModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    origem,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    ticket,");
            sql.Append("    criacao,");
            sql.Append("    atualizacao,");
            sql.Append("    status");
            sql.Append(" FROM");
            sql.Append("     tickets");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<TicketsModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<TicketsModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    origem,");
            sql.Append("    descricao,");
            sql.Append("    valor,");
            sql.Append("    ticket,");
            sql.Append("    criacao,");
            sql.Append("    atualizacao,");
            sql.Append("    status");
            sql.Append(" FROM");
            sql.Append("     tickets");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<TicketsModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(TicketsModel entity)
        {
            entity.atualizacao = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("     tickets");
            sql.Append(" SET");
            sql.Append("     id_usuario=@idUsuario,");
            sql.Append("     origem=@origem,");
            sql.Append("     descricao=@descricao,");
            sql.Append("     valor=@valor,");
            sql.Append("     ticket=@ticket,");
            sql.Append("     criacao=@criacao,");
            sql.Append("     atualizacao=@atualizacao,");
            sql.Append("     status=@status");
            sql.Append(" WHERE");
            sql.Append("     id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
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
