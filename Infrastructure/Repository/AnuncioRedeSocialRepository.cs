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
    class AnuncioRedeSocialRepository : IAnuncioRedeSocialRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public AnuncioRedeSocialRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(AnuncioRedeSocialModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO anuncio_rede_social (");
            sql.Append("    id_anuncio,");
            sql.Append("    id_rede_social,");
            sql.Append("    id_ativo,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao)");
            sql.Append(" VALUES (");
            sql.Append("    @idAnuncio,");
            sql.Append("    @idRedeSocial,");
            sql.Append("    @idAtivo,");
            sql.Append("    @atualizacao,");
            sql.Append("    @criacao");
            sql.Append("    )");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "Delete From anuncio_rede_social Where id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<AnuncioRedeSocialModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_rede_social as idRedeSocial,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao");
            sql.Append(" FROM");
            sql.Append("    anuncio_rede_social");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioRedeSocialModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<AnuncioRedeSocialModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id_anuncio as idAnuncio,");
            sql.Append("    id_rede_social as idRedeSocial,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao");
            sql.Append(" FROM");
            sql.Append("    anuncio_rede_social");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<AnuncioRedeSocialModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(AnuncioRedeSocialModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    anuncio_rede_social");
            sql.Append(" SET");
            
            sql.Append("     id_anuncio=@idAnuncio,");
            sql.Append("     id_rede_social=@idRedeSocial,");
            sql.Append("     id_ativo=@idAtivo,");
            sql.Append("     atualizacao=@atualizacao,");
            sql.Append("     criacao=@criacao");
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

        #endregion

    }
}
