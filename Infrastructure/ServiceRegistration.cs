using Interfaces;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUsuarioRepository, UsuarioRepository>();
            services.AddTransient<IUsuarioDadosRepository, UsuarioDadosRepository>();
            services.AddTransient<IFixoRepository, FixoRepository>();
            services.AddTransient<IEmpresaRepository, EmpresaRepository>();
            services.AddTransient<IConfiguracaoRepository, ConfiguracaoRepository>();
            services.AddTransient<IMensagemRepository, MensagemRepository>();
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<ITokenRefreshRepository, TokenRefreshRepository>();
        }
    }
}
