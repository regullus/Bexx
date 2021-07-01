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
    class AnuncioRepository : IAnuncioRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public AnuncioRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        //Não utilizar, utilizar AddWithSaldoAsync no custom
        public async Task<int> AddAsync(AnuncioModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO anuncio(");
            sql.Append("    id_usuario,");
            sql.Append("    id_anuncio_situacao,");
            sql.Append("    id_midia,");
            sql.Append("    id_anuncio_tipo,");
            sql.Append("    id_anuncio_grupo,");
            sql.Append("    nome,");
            sql.Append("    url,");
            sql.Append("    propaganda,");
            sql.Append("    tempo_exibicao,");
            sql.Append("    total_exibicao,");
            sql.Append("    total_tempo_exibido,");
            sql.Append("    acessos,");
            sql.Append("    token,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    termino,");
            sql.Append("    valor,");
            sql.Append("    valor_unitario,");
            sql.Append("    horas_disponibilizadas");
            sql.Append("    valor_consumido,");
            sql.Append("    observacao");
            sql.Append("    )");
            sql.Append(" VALUES (");
            sql.Append("    @idUsuario,");
            sql.Append("    @idAnuncioSituacao,");
            sql.Append("    @idMidia,");
            sql.Append("    @idAnuncioTipo,");
            sql.Append("    @idAnuncioGrupo,");
            sql.Append("    @nome,");
            sql.Append("    @url,");
            sql.Append("    @propaganda,");
            sql.Append("    @tempoExibicao,");
            sql.Append("    @totalExibicao,");
            sql.Append("    @totalTempoExibido,");
            sql.Append("    @acessos,");
            sql.Append("    @token,");
            sql.Append("    @atualizacao,");
            sql.Append("    @criacao,");
            sql.Append("    @termino,");
            sql.Append("    @valor,");
            sql.Append("    @valorUnitario,");
            sql.Append("    @horasDisponibilizadas,");
            sql.Append("    @valorConsumido,");
            sql.Append("    @observacao,");
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
            var sql = "DELETE FROM anuncio WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<AnuncioModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio_situacao as idAnuncioSituacao,");
            sql.Append("    id_midia as idMidia,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    id_anuncio_grupo as idAnuncioGrupo,");
            sql.Append("    nome,");
            sql.Append("    url,");
            sql.Append("    propaganda,");
            sql.Append("    tempo_exibicao as tempoExibicao,");
            sql.Append("    total_exibicao as totalExibicao,");
            sql.Append("    total_tempo_exibido as totalTempoExibido,");
            sql.Append("    acessos,");
            sql.Append("    token,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    termino,");
            sql.Append("    valor,");
            sql.Append("    valor_unitario as valorUnitario,");
            sql.Append("    horas_disponibilizadas as horasDisponibilizadas,");
            sql.Append("    valor_consumido,");
            sql.Append("    observacao");
            sql.Append(" FROM");
            sql.Append("    anuncio");
            sql.Append(" WHERE");
            sql.Append("    id_anuncio_situacao != 3"); //3 - são os Excluidos logicamente
            sql.Append(" ORDER BY criacao, id DESC;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        //Não utilizar, utilizar GetWithSaldoByIdAsync no custom
        public async Task<AnuncioModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_usuario as idUsuario,");
            sql.Append("    id_anuncio_situacao as idAnuncioSituacao,");
            sql.Append("    id_midia as idMidia,");
            sql.Append("    id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    id_anuncio_grupo as idAnuncioGrupo,");
            sql.Append("    nome,");
            sql.Append("    url,");
            sql.Append("    propaganda,");
            sql.Append("    tempo_exibicao as tempoExibicao,");
            sql.Append("    total_exibicao as totalExibicao,");
            sql.Append("    total_tempo_exibido as totalTempoExibido,");
            sql.Append("    acessos,");
            sql.Append("    token,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    termino,");
            sql.Append("    valor,");
            sql.Append("    valor_unitario as valorUnitario,");
            sql.Append("    horas_disponibilizadas as horasDisponibilizadas,");
            sql.Append("    valor_consumido,");
            sql.Append("    observacao");
            sql.Append(" FROM");
            sql.Append("    anuncio");
            sql.Append(" WHERE");
            sql.Append("    id = @id and");
            sql.Append("    id_anuncio_situacao != 3"); //3 - são os Excluidos logicamente

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<AnuncioModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        //Não utilizar, utilizar UpdateWithSaldoAsync no custom
        public async Task<int> UpdateAsync(AnuncioModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    anuncio");
            sql.Append(" SET");
            sql.Append("    id_usuario=@idUsuario,");
            sql.Append("    id_anuncio_situacao=@idAnuncioSituacao,");
            sql.Append("    id_midia=@idMidia,");
            sql.Append("    id_anuncio_tipo=@idAnuncioTipo,");
            sql.Append("    id_anuncio_grupo=@idAnuncioGrupo,");
            sql.Append("    nome=@nome,");
            sql.Append("    url=@url,");
            sql.Append("    propaganda=@propaganda,");
            sql.Append("    tempo_exibicao=@tempoExibicao,");
            sql.Append("    total_exibicao=@totalExibicao,");
            sql.Append("    total_tempo_exibido=@totalTempoExibido,");
            sql.Append("    acessos=@acessos,");
            sql.Append("    token=@token,");
            sql.Append("    atualizacao=@atualizacao,");
            sql.Append("    termino=@termino,");
            sql.Append("    valor=@valor,");
            sql.Append("    valor_unitario=@valorUnitario,");
            sql.Append("    horas_disponibilizadas=@horasDisponibilizadas,");
            sql.Append("    valorConsumido=@valorConsumido,");
            sql.Append("    observacao=@observacao");
            sql.Append(" WHERE");
            sql.Append("    id=@id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        public async Task<int> DeleteLogicAsync(int id)
        {
            AnuncioModel entity = await GetByIdAsync(id);

            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    anuncio");
            sql.Append(" SET");
            sql.Append("    id_anuncio_situacao=3,"); //3 é exclusão
            sql.Append("    atualizacao=@atualizacao");
            sql.Append(" WHERE");
            sql.Append("    id=@id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(Helpers.QBuild(sql), entity);
                return result;
            }
        }

        #endregion

        #region Custom

        public async Task<IReadOnlyList<AnuncioSaldoModel>> GetbyIdUsuarioAsync(int idUsuario)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    anu.id,");
            sql.Append("    anu.id_usuario as idUsuario,");
            sql.Append("    anu.id_anuncio_situacao as idAnuncioSituacao,");
            sql.Append("    anu.id_midia as idMidia,");
            sql.Append("    anu.id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    anu.id_anuncio_grupo as idAnuncioGrupo,");
            sql.Append("    anu.nome,");
            sql.Append("    anu.url,");
            sql.Append("    anu.propaganda,");
            sql.Append("    anu.tempo_exibicao as tempoExibicao,");
            sql.Append("    anu.total_exibicao as totalExibicao,");
            sql.Append("    anu.total_tempo_exibido as totalTempoExibido,");
            sql.Append("    anu.acessos,");
            sql.Append("    anu.token,");
            sql.Append("    anu.atualizacao,");
            sql.Append("    anu.criacao,");
            sql.Append("    anu.termino,");
            sql.Append("    sal.valor as valor,");
            sql.Append("    sal.token as financeiroToken,");
            sql.Append("    sal.data as financeiroData,");
            sql.Append("    anu.valor,");
            sql.Append("    anu.valor_unitario as valorUnitario,");
            sql.Append("    anu.horas_disponibilizadas as horasDisponibilizadas,");
            sql.Append("    anu.valor_consumido as valorConsumido,");
            sql.Append("    anu.observacao");
            sql.Append(" FROM");
            sql.Append("    anuncio anu");
            sql.Append(" LEFT JOIN");
            sql.Append("    financeiro_saldo sal");
            sql.Append(" ON");
            sql.Append("    anu.id = sal.id_anuncio");
            sql.Append(" WHERE");
            sql.Append("    anu.id_usuario = @idUsuario and");
            sql.Append("    anu.id_anuncio_situacao != 3 ");
            sql.Append(" ORDER BY anu.criacao DESC, anu.id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioSaldoModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario });
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioMidiaModel>> GetAnuncioMidiaAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    anuncio_midia");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioMidiaModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioSituacaoModel>> GetAnuncioSituacaoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    anuncio_situacao");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioSituacaoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioTipoModel>> GetAnuncioTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("    icone");
            sql.Append(" FROM");
            sql.Append("    anuncio_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<AnuncioTipoModel> GetAnuncioTipoByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("    icone");
            sql.Append(" FROM");
            sql.Append("    anuncio_tipo");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<AnuncioTipoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<AnuncioTipoModel>> GetForStreamerAnuncioTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("    icone");
            sql.Append(" FROM");
            sql.Append("    anuncio_tipo");
            sql.Append(" Where");
            sql.Append("    id > 1 "); //1 é anuncio normal, não utilizado no streamer
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioTempoModel>> GetAnuncioTempoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    valor");
            sql.Append(" FROM");
            sql.Append("    anuncio_tempo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioTempoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioQuantidadeModel>> GetAnuncioQuantidadeAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    valor");
            sql.Append(" FROM");
            sql.Append("    anuncio_quantidade");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioQuantidadeModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioSelecionadoModel>> GetAnuncioSelecionadoAsync(int idUsuario, int idAnuncioTipo, int idRedeSocial)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    anu.id,");
            sql.Append("    anu.id_usuario as idUsuario,");
            sql.Append("    anu.id_anuncio_situacao as idAnuncioSituacao,");
            sql.Append("    anu.id_midia as idMidia,");
            sql.Append("    anu.id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    anu.id_anuncio_grupo as idAnuncioGrupo,");
            sql.Append("    anu.nome,");
            sql.Append("    anu.url,");
            sql.Append("    anu.propaganda,");
            sql.Append("    anu.tempo_exibicao as tempoExibicao,");
            sql.Append("    anu.total_exibicao as totalExibicao,");
            sql.Append("    anu.total_tempo_exibido as totalTempoExibido,");
            sql.Append("    anu.acessos,");
            sql.Append("    anu.token,");
            sql.Append("    anu.atualizacao,");
            sql.Append("    anu.criacao,");
            sql.Append("    anu.termino,");
            sql.Append("    sal.valor as valor,");
            sql.Append("    sal.token as financeiroToken,");
            sql.Append("    sal.data as financeiroData,");
            sql.Append("    anu.valor,");
            sql.Append("    anu.valor_unitario as valorUnitario,");
            sql.Append("    anu.horas_disponibilizadas as horasDisponibilizadas,");
            sql.Append("    anu.valor_unitario as valorUnitario,");
            sql.Append("    anu.valor_consumido as valorConsumido,");
            sql.Append("    anu.observacao,");
            sql.Append("    CASE WHEN anusu.id_ativo = 1 Then true ELSE false END as selecionado");
            sql.Append(" FROM");
            sql.Append("    anuncio_rede_social ars,");
            sql.Append("    anuncio anu");
            sql.Append(" LEFT JOIN");
            sql.Append("    financeiro_saldo sal");
            sql.Append(" ON");
            sql.Append("    anu.id = sal.id_anuncio");
            sql.Append(" LEFT JOIN");
            sql.Append("    anuncio_usuario anusu");
            sql.Append(" ON");
            sql.Append("    anu.id = anusu.id_anuncio and");
            sql.Append("    anusu.id_rede_social = @idRedeSocial ");
            sql.Append(" WHERE");
            sql.Append("    anu.id_usuario = 3 and");
            sql.Append("    anu.id_anuncio_tipo = @idAnuncioTipo and");
            sql.Append("    anu.id_anuncio_situacao != 3 and");
            sql.Append("    anu.id = ars.id_anuncio and");
            sql.Append("    ars.id_rede_social = @idRedeSocial");
            sql.Append(" ORDER BY anu.criacao DESC, anu.id;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioSelecionadoModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario, idAnuncioTipo = idAnuncioTipo, idRedeSocial = idRedeSocial });
                return result.ToList();
            }
        }

        public async Task<IReadOnlyList<AnuncioGrupoModel>> GetAnuncioGrupoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    criacao,");
            sql.Append("    nome,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    nome_esp as nomeEsp");
            sql.Append(" FROM");
            sql.Append("    anuncio_grupo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioGrupoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<int> AddWithSaldoAsync(AnuncioSaldoModel entity)
        {
            //Insert na tabela Anuncio
            StringBuilder sql = new StringBuilder();
            sql.Append(" INSERT INTO anuncio(");
            sql.Append("    id_usuario,");
            sql.Append("    id_anuncio_situacao,");
            sql.Append("    id_midia,");
            sql.Append("    id_anuncio_tipo,");
            sql.Append("    id_anuncio_grupo,");
            sql.Append("    nome,");
            sql.Append("    url,");
            sql.Append("    propaganda,");
            sql.Append("    tempo_exibicao,");
            sql.Append("    total_exibicao,");
            sql.Append("    total_tempo_exibido,");
            sql.Append("    acessos,");
            sql.Append("    token,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    termino,");
            sql.Append("    valor,");
            sql.Append("    valor_unitario,");
            sql.Append("    horas_disponibilizadas,");
            sql.Append("    valor_consumido,");
            sql.Append("    observacao");
            sql.Append("    )");
            sql.Append(" VALUES (");
            sql.Append("    @idUsuario,");
            sql.Append("    @idAnuncioSituacao,");
            sql.Append("    @idMidia,");
            sql.Append("    @idAnuncioTipo,");
            sql.Append("    @idAnuncioGrupo,");
            sql.Append("    @nome,");
            sql.Append("    @url,");
            sql.Append("    @propaganda,");
            sql.Append("    @tempoExibicao,");
            sql.Append("    @totalExibicao,");
            sql.Append("    @totalTempoExibido,");
            sql.Append("    @acessos,");
            sql.Append("    @token,");
            sql.Append("    @atualizacao,");
            sql.Append("    @criacao,");
            sql.Append("    @termino,");
            sql.Append("    @valor,");
            sql.Append("    @valorUnitario,");
            sql.Append("    @valorConsumido,");
            sql.Append("    @horasDisponibilizadas,");
            sql.Append("    @observacao");
            sql.Append("    )");
            sql.Append(" RETURNING id;");

            //Insert na tabela financeiro_lancamento
            StringBuilder sql2 = new StringBuilder();
            sql2.Append(" INSERT INTO financeiro_lancamento(");
            sql2.Append("    id_financeiro_conta,");
            sql2.Append("    id_financeiro_lancamento_tipo,");
            sql2.Append("    id_anuncio,");
            sql2.Append("    id_referencia,");
            sql2.Append("    descricao,");
            sql2.Append("    valor,");
            sql2.Append("    data,");
            sql2.Append("    token,");
            sql2.Append("    observacao,");
            sql2.Append("    criacao,");
            sql2.Append("    id_usuario");
            sql2.Append("    )");
            sql2.Append(" VALUES (");
            sql2.Append("    @idFinanceiroConta,");
            sql2.Append("    2,"); //Campanha
            sql2.Append("    @idAnuncio,");
            sql2.Append("    null,");
            sql2.Append("    'adicionado a campanha',"); //ToDo Idioma
            sql2.Append("    @valor,");
            sql2.Append("    @financeiroData,");
            sql2.Append("    '',");
            sql2.Append("    @financeiroObservacao,");
            sql2.Append("    " + Helpers.DataAtualInt + ",");
            sql2.Append("    1");
            sql2.Append("    )");
            sql2.Append(" RETURNING id;");

            //Update na tabela financeiro_saldo do anuncio
            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" UPDATE financeiro_saldo");
            sql3.Append(" SET");
            sql3.Append("    valor = @valor,");
            sql3.Append("    data = @financeiroData");
            sql3.Append(" WHERE");
            sql3.Append("    id_financeiro_conta = @idFinanceiroConta and");
            sql3.Append("    id_anuncio = @idAnuncio;");

            //Insert na tabela financeiro_saldo do anuncio
            StringBuilder sql4 = new StringBuilder();
            sql4.Append(" INSERT INTO financeiro_saldo (");
            sql4.Append("    id_financeiro_conta,");
            sql4.Append("    id_anuncio,");
            sql4.Append("    valor,");
            sql4.Append("    data,");
            sql4.Append("    token)");
            sql4.Append(" VALUES (");
            sql4.Append("    @idFinanceiroConta,");
            sql4.Append("    @idAnuncio,");
            sql4.Append("    @valor,");
            sql4.Append("    @financeiroData,");
            sql4.Append("    ''");
            sql4.Append("    )");
            sql4.Append(" RETURNING id;");

            //Verifica se existe registro em financeiro_saldo
            StringBuilder sql5 = new StringBuilder();
            sql5.Append("SELECT COUNT(1) as retorno FROM financeiro_saldo WHERE id_financeiro_conta = @idFinanceiroConta and id_anuncio = @idAnuncio;");

            //Update na tabela financeiro_saldo  atualiza saldo do publicitario
            StringBuilder sql6 = new StringBuilder();
            sql6.Append(" UPDATE financeiro_saldo");
            sql6.Append(" SET");
            sql6.Append("    valor = valor - @valor,");
            sql6.Append("    data = @financeiroData");
            sql6.Append(" WHERE");
            sql6.Append("    id_financeiro_conta = @idFinanceiroConta and");
            sql6.Append("    id_anuncio is null;");

            int resultTabAnuncio = 0;
            int resultTabFinanceiroLancamento = 0;
            int resultTabFinanceiroSaldo = 0;
            int resultTabFinanceiroSaldoAnunciante = 0;

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //   Verifica se já existe o registro de saldo
                var registroSaldo = await connection.QueryFirstAsync<int>(Helpers.QBuild(sql5), new { idFinanceiroConta = entity.idFinanceiroConta, idAnuncio = entity.idAnuncio });

                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        //Insert na tabela anuncio
                        resultTabAnuncio = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql), entity, transaction: trans);
                        //Obtem o id do anuncio criado para uso no insert da tabela financeiro_lancamento
                        entity.idAnuncio = resultTabAnuncio;
                        //Insert na tabela financeiro_lancamento
                        resultTabFinanceiroLancamento = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql2), entity);
                        //Insert na tabela financeiro_saldo
                        if (registroSaldo > 0)
                        {
                            //Update sql3
                            resultTabFinanceiroSaldo = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql3), entity);
                        }
                        else
                        {
                            //Insert sql4
                            resultTabFinanceiroSaldo = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql4), entity);
                        }

                        //Altera saldo do anunciante na tabela financeiro_saldo
                        resultTabFinanceiroSaldoAnunciante = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql6), entity);

                        //Dando tudo certo, comita
                        await trans.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        //Erro! ToDo LogErro
                        await trans.RollbackAsync();
                        throw;
                    }
                }
            }
            return resultTabAnuncio;
        }

        public async Task<int> UpdateWithSaldoAsync(AnuncioSaldoModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("    anuncio");
            sql.Append(" SET");
            sql.Append("    id_usuario=@idUsuario,");
            sql.Append("    id_anuncio_situacao=@idAnuncioSituacao,");
            sql.Append("    id_midia=@idMidia,");
            sql.Append("    id_anuncio_tipo=@idAnuncioTipo,");
            sql.Append("    id_anuncio_grupo=@idAnuncioGrupo,");
            sql.Append("    nome=@nome,");
            sql.Append("    url=@url,");
            sql.Append("    propaganda=@propaganda,");
            sql.Append("    tempo_exibicao=@tempoExibicao,");
            sql.Append("    total_exibicao=@totalExibicao,");
            sql.Append("    total_tempo_exibido=@totalTempoExibido,");
            sql.Append("    acessos=@acessos,");
            sql.Append("    token=@token,");
            sql.Append("    atualizacao=@atualizacao,");
            sql.Append("    termino=@termino,");
            sql.Append("    valor=@valor,");
            sql.Append("    valor_unitario=@valorUnitario,");
            sql.Append("    horas_disponibilizadas=@horasDisponibilizadas,");
            sql.Append("    valor_consumido=@valorConsumido,");
            sql.Append("    observacao=@observacao");
            sql.Append(" WHERE");
            sql.Append("    id=@id;");

            //Insert na tabela financeiro_lancamento
            StringBuilder sql2 = new StringBuilder();
            sql2.Append(" INSERT INTO financeiro_lancamento(");
            sql2.Append("    id_financeiro_conta,");
            sql2.Append("    id_financeiro_lancamento_tipo,");
            sql2.Append("    id_anuncio,");
            sql2.Append("    id_referencia,");
            sql2.Append("    descricao,");
            sql2.Append("    valor,");
            sql2.Append("    data,");
            sql2.Append("    token,");
            sql2.Append("    observacao,");
            sql2.Append("    criacao,");
            sql2.Append("    id_usuario");
            sql2.Append("    )");
            sql2.Append(" VALUES (");
            sql2.Append("    @idFinanceiroConta,");
            sql2.Append("    2,"); //Campanha
            sql2.Append("    @idAnuncio,");
            sql2.Append("    null,");
            sql2.Append("    'alterado a campanha',"); //ToDo Idioma
            sql2.Append("    @valor,");
            sql2.Append("    @financeiroData,");
            sql2.Append("    '',");
            sql2.Append("    @financeiroObservacao,");
            sql2.Append("    " + Helpers.DataAtualInt + ",");
            sql2.Append("    1");
            sql2.Append("    )");
            sql2.Append(" RETURNING id;");

            //Update na tabela financeiro_saldo do anuncio
            StringBuilder sql3 = new StringBuilder();
            sql3.Append(" UPDATE financeiro_saldo");
            sql3.Append(" SET");
            sql3.Append("    valor = @valor,");
            sql3.Append("    data = @financeiroData");
            sql3.Append(" WHERE");
            sql3.Append("    id_financeiro_conta = @idFinanceiroConta and");
            sql3.Append("    id_anuncio = @idAnuncio;");

            //obtem ultimo valor lancado
            StringBuilder sql4 = new StringBuilder();
            sql4.Append("SELECT valor FROM anuncio WHERE id = @id;");

            //Update na tabela financeiro_saldo  atualiza saldo do publicitario
            StringBuilder sql5 = new StringBuilder();
            sql5.Append(" UPDATE financeiro_saldo");
            sql5.Append(" SET");
            sql5.Append("    valor = valor - @valor,");
            sql5.Append("    data = @financeiroData");
            sql5.Append(" WHERE");
            sql5.Append("    id_financeiro_conta = @idFinanceiroConta and");
            sql5.Append("    id_anuncio is null;");

            int resultTabAnuncio = 0;
            int resultTabFinanceiroLancamento = 0;
            int resultTabFinanceiroSaldo = 0;
            int resultTabFinanceiroSaldoAnunciante = 0;

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                //   Verifica se já existe o registro de saldo
                decimal registroValor = await connection.QueryFirstAsync<decimal>(Helpers.QBuild(sql4), new { id = entity.id });

                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        //Update na tabela anuncio
                        resultTabAnuncio = await connection.ExecuteAsync(Helpers.QBuild(sql), entity, transaction: trans);

                        //caso valor seja diferente efetua lancamento no financeiro
                        //Caso valor nao mude não há se efetua lancamento 
                        if (registroValor != entity.valor)
                        {
                            //Obtem a diferenja do valor informado com o que já se tinha - Este devera ser o lancamento que 
                            //pode ser positivo ou negativo
                            decimal? valorLancamento = entity.valor - registroValor;
                            entity.valor = valorLancamento;

                            //Insert na tabela financeiro_lancamento
                            resultTabFinanceiroLancamento = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql2), entity);

                            //Update sql3
                            resultTabFinanceiroSaldo = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql3), entity);

                            //Altera saldo do anunciante na tabela financeiro_saldo
                            resultTabFinanceiroSaldoAnunciante = await connection.ExecuteScalarAsync<int>(Helpers.QBuild(sql5), entity);
                        }

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
            return resultTabAnuncio;

        }

        public async Task<AnuncioSaldoModel> GetWithSaldoByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    anu.id,");
            sql.Append("    anu.id_usuario as idUsuario,");
            sql.Append("    anu.id_anuncio_situacao as idAnuncioSituacao,");
            sql.Append("    anu.id_midia as idMidia,");
            sql.Append("    anu.id_anuncio_tipo as idAnuncioTipo,");
            sql.Append("    anu.id_anuncio_grupo as idAnuncioGrupo,");
            sql.Append("    anu.nome,");
            sql.Append("    anu.url,");
            sql.Append("    anu.propaganda,");
            sql.Append("    anu.tempo_exibicao as tempoExibicao,");
            sql.Append("    anu.total_exibicao as totalExibicao,");
            sql.Append("    anu.total_tempo_exibido as totalTempoExibido,");
            sql.Append("    anu.acessos,");
            sql.Append("    anu.token,");
            sql.Append("    anu.atualizacao,");
            sql.Append("    anu.criacao,");
            sql.Append("    anu.termino,");
            sql.Append("    sal.valor as valor,");
            sql.Append("    sal.token as financeiroToken,");
            sql.Append("    sal.data as financeiroData,");
            sql.Append("    anu.valor,");
            sql.Append("    anu.valor_unitario as valorUnitario,");
            sql.Append("    anu.horas_disponibilizadas as horasDisponibilizadas,");
            sql.Append("    anu.valor_consumido as valorConsumido,");
            sql.Append("    anu.observacao");
            sql.Append(" FROM");
            sql.Append("    anuncio anu");
            sql.Append(" LEFT JOIN");
            sql.Append("    financeiro_saldo sal");
            sql.Append(" ON");
            sql.Append("    anu.id = sal.id_anuncio");
            sql.Append(" WHERE");
            sql.Append("    anu.id = @id");

            sql.Append(" ORDER BY anu.criacao DESC, anu.id;");
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<AnuncioSaldoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<AnuncioPrecoModel>> GetAnuncioPrecoByIdUsuarioAsync(int? idUsuario)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_grupo as idGrupo,");
            sql.Append("    hora_minima as horaMinima,");
            sql.Append("    hora_assistida as horaAssistida,");
            sql.Append("    proposta,");
            sql.Append("    atualizacao,");
            sql.Append("    criacao,");
            sql.Append("    id_usuario as idUsuario");
            sql.Append(" FROM");
            sql.Append("    anuncio_preco");
            sql.Append(" WHERE");
            if (idUsuario == null || idUsuario == 0)
                sql.Append("    id_usuario is null;");
            else
                sql.Append("    id_usuario = @idUsuario;");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<AnuncioPrecoModel>(Helpers.QBuild(sql), new { idUsuario = idUsuario });
                return result.ToList();
            }
        }


        #endregion

    }
}
