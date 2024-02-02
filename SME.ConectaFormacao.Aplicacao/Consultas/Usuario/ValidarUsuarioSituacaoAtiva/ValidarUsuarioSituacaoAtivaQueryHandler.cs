using System.Net;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarUsuarioSituacaoAtivaQueryHandler : IRequestHandler<ValidarUsuarioSituacaoAtivaQuery>
    {
        private readonly ICacheDistribuido _cacheDistribuido;
        private readonly IRepositorioUsuario _repositorioUsuario;

        public ValidarUsuarioSituacaoAtivaQueryHandler(ICacheDistribuido cacheDistribuido,IRepositorioUsuario repositorioUsuario)
        {
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task Handle(ValidarUsuarioSituacaoAtivaQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _cacheDistribuido.ObterAsync(string.Format(CacheDistribuidoNomes.Usuario, request.Login), () => _repositorioUsuario.ObterPorLogin(request.Login));

            if (usuario.EhNulo())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO, HttpStatusCode.Unauthorized);
            
            if (usuario.EstaAguardandoValidacaoEmail())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_VALIDOU_EMAIL, HttpStatusCode.Unauthorized);
        }
    }
}