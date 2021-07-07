using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface IUnitOfWork
    {
        IUsuarioRepository Usuario { get; }
        IUsuarioDadosRepository UsuarioDados { get; }
        IFixoRepository Fixo { get; }
        IEmpresaRepository Empresa { get; }
        IConfiguracaoRepository Configuracao { get; }
        IImagemRepository Imagem { get; }
        IMensagemRepository Mensagem { get; }
        ITicketsRepository Tickets { get; }
        IBaseRepository Base { get; }
        ITokenRefreshRepository TokenRefresh { get; }
    }
}