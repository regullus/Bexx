using Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IUsuarioRepository usuarioRepository,
                          IUsuarioDadosRepository usuarioDadosRepository,
                          IDashBoardRepository dashBoardRepository,
                          IAnuncioRepository anuncioRepository,
                          IEmpresaRepository empresaRepository,
                          IConfiguracaoRepository configuracaoRepository,
                          IFilialRepository filialRepository,
                          IImagemRepository imagemRepository,
                          IMensagemRepository mensagemRepository,
                          IPlanoRepository planoRepository,
                          ITicketsRepository ticketsRepository,
                          ITronarRepository tronarRepository,
                          IFixoRepository fixoRepository,
                          IRedeSocialRepository redeSocialRepository,
                          IAnuncioRedeSocialRepository anuncioRedeSocialRepository,
                          IBaseRepository baseRepository,
                          IAnuncioUsuarioRepository anuncioUsuarioRepository,
                          IFinanceiroContaRepository financeiroContaRepository,
                          IFinanceiroLancamentoRepository financeiroLancamentoRepository,
                          IFinanceiroSaldoRepository financeiroSaldoRepository,
                          ITokenRefreshRepository tokenRefreshRepository,
                          IScheduleRepository scheduleRepository
        )
        {
            Usuario = usuarioRepository;
            UsuarioDados = usuarioDadosRepository;
            DashBoard = dashBoardRepository;
            Anuncio = anuncioRepository;
            Empresa = empresaRepository;
            Configuracao = configuracaoRepository;
            Filial = filialRepository;
            Imagem = imagemRepository;
            Mensagem = mensagemRepository;
            Plano = planoRepository;
            Tickets = ticketsRepository;
            Tronar = tronarRepository;
            Fixo = fixoRepository;
            RedeSocial = redeSocialRepository;
            AnuncioRedeSocial = anuncioRedeSocialRepository;
            Base = baseRepository;
            AnuncioUsuario = anuncioUsuarioRepository;
            FinanceiroConta = financeiroContaRepository;
            FinanceiroLancamento = financeiroLancamentoRepository;
            FinanceiroSaldo = financeiroSaldoRepository;
            TokenRefresh = tokenRefreshRepository;
            Schedule = scheduleRepository;
        }
        public IUsuarioRepository Usuario { get; }
        public IUsuarioDadosRepository UsuarioDados { get; }
        public IDashBoardRepository DashBoard { get; }
        public IAnuncioRepository Anuncio { get; }
        public IEmpresaRepository Empresa { get; }
        public IConfiguracaoRepository Configuracao { get; }
        public IFilialRepository Filial { get; }
        public IImagemRepository Imagem { get; }
        public IMensagemRepository Mensagem { get; }
        public IPlanoRepository Plano { get; }
        public ITicketsRepository Tickets { get; }
        public ITronarRepository Tronar { get; }
        public IFixoRepository Fixo { get; }
        public IRedeSocialRepository RedeSocial { get; }
        public IAnuncioRedeSocialRepository AnuncioRedeSocial { get; }
        public IBaseRepository Base { get; }
        public IAnuncioUsuarioRepository AnuncioUsuario { get; }
        public IFinanceiroContaRepository FinanceiroConta { get; }
        public IFinanceiroLancamentoRepository FinanceiroLancamento { get; }
        public IFinanceiroSaldoRepository FinanceiroSaldo { get; }
        public ITokenRefreshRepository TokenRefresh { get; }
        public IScheduleRepository Schedule { get; }
    }
}
