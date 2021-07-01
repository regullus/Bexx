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
    class RedeSocialRepository : IRedeSocialRepository
    {
        #region Base

        #region Base

        private readonly IConfiguration configuration;
        public RedeSocialRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #endregion

        #region Crud

        public async Task<int> AddAsync(RedeSocialModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO rede_social (");
            sql.Append("    id_ativo,");
            sql.Append("    id_rede_social_tipo,");
            sql.Append("    nome,");
            sql.Append("    criacao");
            sql.Append("VALUES (");
            sql.Append("    @idAtivo,");
            sql.Append("    @idRedeSocialTipo,");
            sql.Append("    @nome,");
            sql.Append("    @criacao");
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
            var sql = "DELETE FROM rede_social Where id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<RedeSocialModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_rede_social_tipo as idRedeSocialTipo,");
            sql.Append("    nome,");
            sql.Append("    criacao");
            sql.Append(" FROM");
            sql.Append("    rede_social");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<RedeSocialModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<RedeSocialModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_rede_social_tipo as idRedeSocialTipo,");
            sql.Append("    nome,");
            sql.Append("    criacao");
            sql.Append(" FROM");
            sql.Append("    rede_social");
            sql.Append(" WHERE");
            sql.Append("    id = @id and");
            sql.Append("    id_ativo = 1"); //Ativo

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<RedeSocialModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(RedeSocialModel entity)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("     rede_social");
            sql.Append(" SET");
            sql.Append("     id_ativo=@idAtivo,");
            sql.Append("     id_rede_social_tipo=@idRedeSocialTipo,");
            sql.Append("     nome=@nome");
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

        public async Task<IReadOnlyList<RedeSocialModel>> GetRedeSocialByIdTipoAsync(int idRedeSocialTipo)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_rede_social_tipo as idRedeSocialTipo,");
            sql.Append("    nome,");
            sql.Append("    criacao");
            sql.Append(" FROM");
            sql.Append("    rede_social");
            sql.Append(" WHERE");
            sql.Append("    id_rede_social_tipo = @idRedeSocialTipo and");
            sql.Append("    id_ativo = 1"); //Ativo
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<RedeSocialModel>(Helpers.QBuild(sql), new { idRedeSocialTipo = idRedeSocialTipo });
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<RedeSocialTipoModel>> GetRedeSocialTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    rede_social_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<RedeSocialTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<RedeSocialSelecionadoModel>> GetRedeSocialSelecionadoAsync(int idAnuncio)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    rs.id as id,");
            sql.Append("    rs.id_ativo as isAtivo,");
            sql.Append("    rs.id_rede_social_tipo as idRedeSocialTipo,");
            sql.Append("    rs.nome as nome,");
            sql.Append("    rs.criacao as criacao,");
            sql.Append("    CASE WHEN ars.id_ativo = 1 Then true ELSE false END as selecionado");
            sql.Append(" FROM");
            sql.Append("    rede_social rs");
            sql.Append(" LEFT JOIN");
            sql.Append("    anuncio_rede_social ars");
            sql.Append(" ON");
            sql.Append("    rs.id = ars.id_rede_social and");
            sql.Append("    ars.id_anuncio = @idAnuncio");
            sql.Append(" WHERE");
            sql.Append("    rs.id_rede_social_tipo = 2"); //Tipo 2 é sao os liberados para uso dos streamers
            sql.Append(" ORDER BY");
            sql.Append(" rs.id");


            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<RedeSocialSelecionadoModel>(Helpers.QBuild(sql), new { idAnuncio = idAnuncio });
                return result.ToList();
            }
        }

        public async Task<int> DeleteRedeSocialSelecionadoAsync(int idAnuncio)
        {
            var sql = "DELETE FROM anuncio_rede_social Where id_anuncio = @idAnuncio";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { idAnuncio = idAnuncio });
                return result;
            }
        }

        #endregion

    }
}
