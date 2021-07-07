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
                          IEmpresaRepository empresaRepository,
                          IConfiguracaoRepository configuracaoRepository,
                          IImagemRepository imagemRepository,
                          IMensagemRepository mensagemRepository,
                          ITicketsRepository ticketsRepository,
                          IFixoRepository fixoRepository,
                          IBaseRepository baseRepository,
                          ITokenRefreshRepository tokenRefreshRepository
        )
        {
            Usuario = usuarioRepository;
            UsuarioDados = usuarioDadosRepository;
            Empresa = empresaRepository;
            Configuracao = configuracaoRepository;
            Imagem = imagemRepository;
            Mensagem = mensagemRepository;
            Tickets = ticketsRepository;
            Fixo = fixoRepository;
            Base = baseRepository;
            TokenRefresh = tokenRefreshRepository;
        }
        public IUsuarioRepository Usuario { get; }
        public IUsuarioDadosRepository UsuarioDados { get; }
        public IEmpresaRepository Empresa { get; }
        public IConfiguracaoRepository Configuracao { get; }
        public IImagemRepository Imagem { get; }
        public IMensagemRepository Mensagem { get; }
        public ITicketsRepository Tickets { get; }
        public IFixoRepository Fixo { get; }
        public IBaseRepository Base { get; }
        public ITokenRefreshRepository TokenRefresh { get; }
    }
}
