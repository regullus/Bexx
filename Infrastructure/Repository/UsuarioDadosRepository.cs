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

namespace Infrastructure.Repository
{
    public class UsuarioDadosRepository : IUsuarioDadosRepository
    {
        #region Crud

        private readonly IConfiguration configuration;
        public UsuarioDadosRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<int> AddAsync(UsuarioDadosModel entity)
        {
            //entity.AddedOn = DateTime.Now;
            var sql = "Insert into user_data (atualizacao, data_ultimo_acesso, exibe_boas_vindas, social_twitch, social_facebook, social_twitter, social_youtube, social_instagram) VALUES (@Atualizacao,@DataUltimoAcesso,@ExibeBoasVindas,@SocialTwitch,@SocialFacebook,@SocialTwitter,@SocialYouTube,@SocialInstagram)";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            var sql = "DELETE FROM user_data WHERE id = @Id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                return result;
            }
        }

        public async Task<IReadOnlyList<UsuarioDadosModel>> GetAllAsync()
        {
            var sql = "SELECT * FROM user_data";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QueryAsync<UsuarioDadosModel>(sql);
                return result.ToList();
            }
        }

        public async Task<UsuarioDadosModel> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM user_data WHERE id = @Id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.QuerySingleOrDefaultAsync<UsuarioDadosModel>(sql, new { Id = id });
                return result;
            }
        }

        public async Task<int> UpdateAsync(UsuarioDadosModel entity)
        {
            entity.Atualizacao = Helpers.DataAtualInt; 
            var sql = "UPDATE user_data SET atualizacao = @atualizacao, data_ultimo_acesso = @dataUltimoAcesso, exibe_boas_vindas = @exibeBoasVindas, social_twitch = @socialTwitch, social_facebook = @socialFacebook, social_twitter = @socialTwitter, social_youtube = @socialYouTube, social_instagram = @socialInstagram,  WHERE id = @id";
            using (var connection = new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(sql, entity);
                return result;
            }
        }

        #endregion

        #region Custom

        #endregion
    }
}
