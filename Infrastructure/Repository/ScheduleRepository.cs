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
    class ScheduleRepository : IScheduleRepository
    {
        #region Base

        private readonly IConfiguration configuration;

        public ScheduleRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Custom

        public async Task<int> UsuarioCalculoAsync(ImputUsuarioCalculoModel imput)
        {
            
            UsuarioCalculoModel entity = new UsuarioCalculoModel();
            entity.idUsuario = imput.idUsuario;
            entity.periodo = imput.periodo.ToString("yyyy-MM-01"); //@periodo ('2021-06-01')
            entity.mes = imput.periodo.ToString("MM"); //@mes (tipo 6)
            entity.ano = imput.periodo.ToString("yyyy"); //@ano (tipo 2021)

            StringBuilder sql0 = new StringBuilder();
            sql0.Append(" DELETE FROM");
            sql0.Append("    anuncio_usuario_calculo");
            sql0.Append(" WHERE");
            sql0.Append("    extract(month FROM periodo) = @mes and");
            sql0.Append("    extract(year FROM periodo) = @ano and");
            sql0.Append("    id_usuario = @idUsuario");
            sql0.Append(" RETURNING id;");

            StringBuilder sql1 = new StringBuilder();
            sql1.Append(" INSERT INTO");
            sql1.Append("    anuncio_usuario_calculo");
            sql1.Append(" SELECT ");
            sql1.Append("    ast.id_anuncio as id_anuncio,");
            sql1.Append("    ast.id_usuario as id_usuario,");
            sql1.Append("    anu.nome as nome,");
            sql1.Append("    anu.id_usuario as id_anunciante,");
            sql1.Append("    @periodo as periodo,");
            sql1.Append("    count(ast.id_anuncio) as quantidade_exibicoes,");
            sql1.Append("    sum(asl.visualizacao * asl.tempo_exibicao)  as assistido_seg,");
            sql1.Append("    sum(asl.visualizacao * asl.tempo_exibicao) / 3600 as assistido_horas,");
            sql1.Append("    null as valor,");
            sql1.Append("    to_char(CURRENT_DATE, 'YYYYMMDD')::integer as atualizacao,");
            sql1.Append("    to_char(CURRENT_DATE, 'YYYYMMDD')::integer as criacao");
            sql1.Append(" FROM");
            sql1.Append("    anuncio anu,");
            sql1.Append("    anuncio_stream_log asl,");
            sql1.Append("    anuncio_stream ast");
            sql1.Append(" WHERE");
            sql1.Append("    anu.id = ast.id_anuncio  and");
            sql1.Append("    ast.id = asl.id_anuncio_stream and");
            sql1.Append("    ast.id_usuario = @idUsuario and");
            sql1.Append("    extract(month FROM asl.data) = @mes and");
            sql1.Append("    extract(year FROM asl.data) = @ano");
            sql1.Append(" GROUP BY");
            sql1.Append("    ast.id_anuncio,");
            sql1.Append("    ast.id_usuario,");
            sql1.Append("    anu.nome,");
            sql1.Append("    anu.id_usuario");
            sql1.Append(" ORDER BY");
            sql1.Append("    ast.id_anuncio");
            sql1.Append(" RETURNING id;");

            StringBuilder sql2 = new StringBuilder();
            sql2.Append(" UPDATE");
            sql2.Append("    anuncio_usuario_calculo as auc");
            sql2.Append(" SET");
            sql2.Append("    valor =");
            sql2.Append("       (SELECT");
            sql2.Append("           MAX(ac.hora_assistida)");
            sql2.Append("        FROM");
            sql2.Append("           anuncio_preco ac");
            sql2.Append("        WHERE");
            sql2.Append("          auc.hora_assistida - ac.hora_assistida > 0 and");
            sql2.Append("          ac.id_usuario = auc.id_anunciante");
            sql2.Append("       )");
            sql2.Append(" WHERE");
            sql2.Append("    EXTRACT(month FROM auc.periodo) = @mes  and");
            sql2.Append("    EXTRACT(year FROM auc.periodo) = @mes;");
            sql2.Append(" RETURNING id;");

            //Insert na tabela financeiro_saldo 
            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" UPDATE");
            sql3.Append("    anuncio_usuario_calculo as auc");
            sql3.Append(" Set");
            sql3.Append("    valor =");
            sql3.Append("       (SELECT");
            sql3.Append("           MAX(ac.hora_assistida)");
            sql3.Append("        FROM");
            sql3.Append("           anuncio_preco ac");
            sql3.Append("        WHERE");
            sql3.Append("           auc.hora_assistida - ac.hora_assistida > 0 and");
            sql3.Append("           ac.id_usuario is null");
            sql3.Append("       )");
            sql3.Append(" WHERE");
            sql3.Append("    EXTRACT(month FROM auc.periodo) = @mes and");
            sql3.Append("    EXTRACT(year FROM auc.periodo) = @ano and");
            sql3.Append("    auc.valor is null;");
            sql3.Append(" RETURNING id;");

            int result = 0;

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        //remove registros de anuncio_usuario_calculo
                        result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql0), entity);
                        //insere registros de anuncio_usuario_calculo
                        result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql1), entity);
                        //update valor na tabela anuncio_usuario_calculo - com tabela de valores do publicitario
                        result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql2), entity);
                        //update valor na tabela anuncio_usuario_calculo com tabela default caso não tenha incluido na query acima
                        result = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql3), entity);

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
            return result;
        }

        #endregion
    }
}
