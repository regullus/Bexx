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
    class FilialRepository : IFilialRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public FilialRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(FilialModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO filial(");
            sql.Append("    id_empresa,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    endereco,");
            sql.Append("    telefone,");
            sql.Append("    mapa,");
            sql.Append("    nome_ing,");
            sql.Append("    nome_esp");
            sql.Append(" VALUES (");
            sql.Append("    @id_empresa,");
            sql.Append("    @atualizacao,");
            sql.Append("    @nome,");
            sql.Append("    @endereco,");
            sql.Append("    @telefone,");
            sql.Append("    @mapa,");
            sql.Append("    @nome_ing,");
            sql.Append("    @)nome_esp");
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
            var sql = "DELETE FROM filial WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<FilialModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    endereco,");
            sql.Append("    telefone,");
            sql.Append("    mapa,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("	FROM");
            sql.Append("    filial");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<FilialModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<FilialModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_empresa as idEmpresa,");
            sql.Append("    atualizacao,");
            sql.Append("    nome,");
            sql.Append("    endereco,");
            sql.Append("    telefone,");
            sql.Append("    mapa,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("	FROM");
            sql.Append("    filial");
            sql.Append(" WHERE");
            sql.Append("    id = @id");
         
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<FilialModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(FilialModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("     filial");
            sql.Append(" SET");
            sql.Append("     id_empresa=@idEmpresa,");
            sql.Append("     atualizacao=@atualizacao,");
            sql.Append("     nome=@nome,");
            sql.Append("     endereco=@endereco,");
            sql.Append("     telefone=@telefone,");
            sql.Append("     mapa=@mapa,");
            sql.Append("     nome_ing=@nomeIng,");
            sql.Append("     nome_esp=@nomeEsp");
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
