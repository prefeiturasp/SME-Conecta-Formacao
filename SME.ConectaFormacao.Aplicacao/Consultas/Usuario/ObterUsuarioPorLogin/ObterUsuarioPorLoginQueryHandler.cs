using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorLoginQueryHandler : IRequestHandler<ObterUsuarioPorLoginQuery, Usuario>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterUsuarioPorLoginQueryHandler(IRepositorioUsuario repositorioUsuario,ICacheDistribuido cacheDistribuido)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public Task<Usuario> Handle(ObterUsuarioPorLoginQuery request, CancellationToken cancellationToken)
        {
            return _cacheDistribuido.ObterAsync(string.Format(CacheDistribuidoNomes.Usuario,request.Login), () => _repositorioUsuario.ObterPorLogin(request.Login));
        }
    }
}
