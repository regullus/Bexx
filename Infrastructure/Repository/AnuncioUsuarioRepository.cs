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
    class AnuncioUsuarioRepository : IAnuncioUsuarioRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public AnuncioUsuarioRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(AnuncioUsuarioModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO anuncio_usuario (");
            sql.Append("    id_anuncio,");
            sql.Append("    id_usuario,");
            sql.Append("    id_anuncio_tipo,");
            sql.Append("    id_rede_social,");
            sql.Append("    id_ativo,");
            sql.Append("    criacao)");
            sql.Append(" VALUES (");
            sql.Append("    @idAnuncio,");
            sql.Append("    @idUsuario,");
            sql.Append("    @idAnuncioTipo,");
            sql.Append("    @idRedeSocial,");
            sql.Append("    @idAtivo,");
            sql.Append("    @criacao)");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                int result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity);
                return result; 
            }
        }

        public async Task<int> DeleteAsync(int idUsuario, int idAnuncioTipo, int idRedeSocial)
        {
            var sql = "DELETE FROM anuncio_usuario WHERE id_usuario = @idUsuario and id_anuncio_tipo = @idAnuncioTipo and id_rede_social = @idRedeSocial";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo, idRedeSocial = idRedeSocial });
                return result;
            }
        }

        public async Task<IReadOnlyList<AnuncioUsuarioModel>> GetbyIdUsuarioAsync(int idUsuario)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id_anuncio,");
            sql.Append("    id_usuario,");
            sql.Append("    id_anuncio_tipo,");
            sql.Append("    id_rede_social,");
            sql.Append("    id_ativo,");
            sql.Append("    criacao");
            sql.Append(" FROM");
            sql.Append("    anuncio_usuario");
            sql.Append(" WHERE");
            sql.Append("    id_usuario = @idUsuario and");
            sql.Append("    id_ativo = 1"); //1 - ativos
            sql.Append(" ORDER BY id_anuncio");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioUsuarioModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario });
                return result.ToList();
            }
        }

        #endregion

        #region Custom

        #endregion

    }
}
