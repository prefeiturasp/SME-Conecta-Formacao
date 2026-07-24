using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.SalvarLog;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioLogadoQueryHandler(IContextoAplicacao contextoAplicacao, IMediator mediator, ICacheDistribuido cacheDistribuido) : 
        IRequestHandler<ObterUsuarioLogadoQuery, Usuario>
    {
        public async Task<Usuario> Handle(ObterUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            await mediator.Send(new SalvarLogCommand(typeof(ObterUsuarioLogadoQueryHandler).FullName!, LogNivel.Informacao, "Iniciando consulta do usuário logado", null, Guid.NewGuid(), null), cancellationToken);
            var nomeChave = CacheDistribuidoNomes.UsuarioLogado.Parametros(contextoAplicacao.UsuarioLogado);

            return await cacheDistribuido.ObterAsync(nomeChave, () => mediator.Send(new ObterUsuarioPorLoginQuery(contextoAplicacao.UsuarioLogado), cancellationToken));
        }
    }
}