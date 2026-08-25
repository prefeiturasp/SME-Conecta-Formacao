using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDresPorGrupoUsuarioLogadoQueryHandler : IRequestHandler<ObterDresPorGrupoUsuarioLogadoQuery, IEnumerable<Dre>>
    {
        private readonly IMediator _mediator;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterDresPorGrupoUsuarioLogadoQueryHandler(IMediator mediator, IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<IEnumerable<Dre>> Handle(ObterDresPorGrupoUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            var grupoId = await _mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia(), cancellationToken);
            return await _repositorioAreaPromotora.ObterDresPorGrupoId(grupoId);
        }
    }
}
