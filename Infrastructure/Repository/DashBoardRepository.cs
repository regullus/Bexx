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
    class DashBoardRepository : IDashBoardRepository
    {
        #region Base

        private readonly IConfiguration configuration;

        public DashBoardRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Custom

        public async Task<IList<StreamerPublicitarioModel>> StreamerPublicitarioAsync(int id_usuario)
        {
            IList<StreamerPublicitarioModel> result;
            StringBuilder sql = new StringBuilder();
            sql.Append(" Select");
            sql.Append("    users.nome as nome,");
            sql.Append("    users.avatar as avatar,");
            sql.Append("    count(anu_str_log.id_anuncio_stream) as exibicoes,");
            sql.Append("    (sum(anu_str_log.visualizacao) / count(anu_str_log.id_anuncio_stream)) as media,");
            sql.Append("    anu_str.midia_social as midiasocial,");
            sql.Append("    users.id_sexo as idsexo");
            sql.Append(" From");
            sql.Append("    anuncio as anu,");
            sql.Append("    anuncio_stream as anu_str,");
            sql.Append("    anuncio_stream_log anu_str_log,");
            sql.Append("    users");
            sql.Append(" Where");
            sql.Append("    anu.id_usuario = @Id and");
            sql.Append("    anu.id_anuncio_situacao != 3 and");
            sql.Append("    anu.id = anu_str.id_anuncio and");
            sql.Append("    anu_str.id = anu_str_log.id_anuncio_stream and");
            sql.Append("    anu_str.id_usuario = users.id");
            sql.Append(" Group by");
            sql.Append("    users.nome,");
            sql.Append("    users.avatar,");
            sql.Append("    anu_str.midia_social,");
            sql.Append("    users.id_sexo");
            sql.Append(" Order by");
            sql.Append("    media desc");
            sql.Append("    limit 10");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var results = await connection.QueryAsync<StreamerPublicitarioModel>(Helpers.QBuild(sql), new { id = id_usuario });
                result = results.ToList();
            }
            return result;
        }

        public async Task<IList<DashExibicaoCampanhaModel>> DashExibicaoCampanhaAsync(int id_usuario)
        {
            IList<DashExibicaoCampanhaModel> result;
            StringBuilder sql = new StringBuilder();
            sql.Append(" Select");
            sql.Append("    nome as nome,");
            sql.Append("    total_tempo_exibido / 3600 as consumido,");
            sql.Append("    horas_disponibilizadas as total,");
            sql.Append("    (horas_disponibilizadas - total_tempo_exibido / 3600) as resta");
            sql.Append(" From");
            sql.Append("    anuncio");
            sql.Append(" Where");
            sql.Append("    id_usuario = @Id and");
            sql.Append("    id_anuncio_situacao != 3");
            sql.Append(" Order by");
            sql.Append("    criacao desc");
            sql.Append("    limit 6");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var results = await connection.QueryAsync<DashExibicaoCampanhaModel>(Helpers.QBuild(sql), new { id = id_usuario });
                result = results.ToList();
            }
            return result;
        }

        #endregion
    }
}
