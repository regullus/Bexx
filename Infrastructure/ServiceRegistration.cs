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
            services.AddTransient<IDashBoardRepository, DashBoardRepository>();
            services.AddTransient<IFixoRepository, FixoRepository>();
            services.AddTransient<IAnuncioRepository, AnuncioRepository>();
            services.AddTransient<IEmpresaRepository, EmpresaRepository>();
            services.AddTransient<IConfiguracaoRepository, ConfiguracaoRepository>();
            services.AddTransient<IFilialRepository, FilialRepository>();
            services.AddTransient<IImagemRepository, ImagemRepository>();
            services.AddTransient<IMensagemRepository, MensagemRepository>();
            services.AddTransient<IPlanoRepository, PlanoRepository>();
            services.AddTransient<ITicketsRepository, TicketsRepository>();
            services.AddTransient<ITronarRepository, TronarRepository>();
            services.AddTransient<IRedeSocialRepository, RedeSocialRepository>();
            services.AddTransient<IAnuncioRedeSocialRepository, AnuncioRedeSocialRepository>();
            services.AddTransient<IBaseRepository, BaseRepository>();
            services.AddTransient<IAnuncioUsuarioRepository, AnuncioUsuarioRepository>();
            services.AddTransient<IFinanceiroContaRepository, FinanceiroContaRepository>();
            services.AddTransient<IFinanceiroLancamentoRepository, FinanceiroLancamentoRepository>();
            services.AddTransient<IFinanceiroSaldoRepository, FinanceiroSaldoRepository>();
            services.AddTransient<ITokenRefreshRepository, TokenRefreshRepository>();
            services.AddTransient<IScheduleRepository, ScheduleRepository>();
        }
    }
}
