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
    class PlanoRepository : IPlanoRepository
    {
        #region Base

        private readonly IConfiguration configuration;
        public PlanoRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Crud

        public async Task<int> AddAsync(PlanoModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO plano (");
            sql.Append("    id_ativo,");
            sql.Append("    id_plano_tipo,");
            sql.Append("    valor_mensal,");
            sql.Append("    valor,");
            sql.Append("    meses,");
            sql.Append("    quantidade_usuario,");
            sql.Append("    dias_acesso_livre,");
            sql.Append("    descricao,");
            sql.Append("    descricao_ing,");
            sql.Append("    descricao_esp,");
            sql.Append("    atualizacao");
            sql.Append("VALUES (");
            sql.Append("    @idAtivo,");
            sql.Append("    @idPlanoTipo,");
            sql.Append("    @valorMensal,");
            sql.Append("    @valor,");
            sql.Append("    @meses,");
            sql.Append("    @quantidadeUsuario,");
            sql.Append("    @diasAcessoLivre,");
            sql.Append("    @descricao,");
            sql.Append("    @descricaoIng,");
            sql.Append("    @descricaoEsp,");
            sql.Append("    @atualizacao");
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
            var sql = "DELETE FROM plano Where id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<PlanoModel>> GetAllAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_plano_tipo as idPlanoTipo,");
            sql.Append("    valor_mensal as valorMensal,");
            sql.Append("    valor,");
            sql.Append("    meses,");
            sql.Append("    quantidade_usuario as quantidadeUsuario,");
            sql.Append("    dias_acesso_livre as diasAcessoLivre,");
            sql.Append("    descricao,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    descricao_esp as descricaoEsp,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    plano");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PlanoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        public async Task<PlanoModel> GetByIdAsync(int id)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id");
            sql.Append("    id_ativo as idAtivo,");
            sql.Append("    id_plano_tipo as idPlanoTipo,");
            sql.Append("    valor_mensal as valorMensal,");
            sql.Append("    valor,");
            sql.Append("    meses,");
            sql.Append("    quantidade_usuario as quantidadeUsuario,");
            sql.Append("    dias_acesso_livre as diasAcessoLivre,");
            sql.Append("    descricao,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    descricao_esp as descricaoEsp,");
            sql.Append("    atualizacao");
            sql.Append(" FROM");
            sql.Append("    plano");
            sql.Append(" WHERE");
            sql.Append("    id = @id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<PlanoModel>(Helpers.QBuild(sql), new { id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(PlanoModel entity)
        {
            entity.atualizacao = Helpers.DataAtualInt;
            StringBuilder sql = new StringBuilder();
            sql.Append(" UPDATE");
            sql.Append("     plano");
            sql.Append(" SET");
            sql.Append("     id_ativo=@idAtivo,");
            sql.Append("     id_plano_tipo=@idPlanoTipo,");
            sql.Append("     valor_mensal=@valorMensal,");
            sql.Append("     valor=@valor,");
            sql.Append("     meses=@meses,");
            sql.Append("     quantidade_usuario=@quantidadeUsuario,");
            sql.Append("     dias_acesso_livre=@diasAcessoLivre,");
            sql.Append("     descricao=@descricao,");
            sql.Append("     descricao_ing=@descricaoIng,");
            sql.Append("     descricao_esp=@descricaoEsp,");
            sql.Append("     atualizacao=@atualizacao");
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
        public async Task<IReadOnlyList<PlanoTipoModel>> GetPlanoTipoAsync()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT");
            sql.Append("    id,");
            sql.Append("    nome,");
            sql.Append("    descricao,");
            sql.Append("    nome_ing as nomeIng,");
            sql.Append("    descricao_ing as descricaoIng,");
            sql.Append("    nome_esp as nomeEsp,");
            sql.Append("    descricao_esp as descricaoEsp");
            sql.Append(" FROM");
            sql.Append("    plano_tipo");
            sql.Append(" ORDER BY id");

            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<PlanoTipoModel>(Helpers.QBuild(sql));
                return result.ToList();
            }
        }

        #endregion

    }
}
