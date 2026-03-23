using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdDashboardQueryHandler : IRequestHandler<ObterPropostasIdDashboardQuery, IEnumerable<Proposta>>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public ObterPropostasIdDashboardQueryHandler(IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<Proposta>> Handle(ObterPropostasIdDashboardQuery request, CancellationToken cancellationToken)
        {
            var usuarioLogado = await _mediator.Send(new ObterUsuarioLogadoQuery());
            var perfilUsuarioLogado = await _mediator.Send(new ObterGrupoUsuarioLogadoQuery());

            return await _repositorioProposta.ObterPropostasIdsDashBoard(
                request.AreaPromotoraIdUsuarioLogado,
                request.Filtro.Id,
                request.Filtro.AreaPromotoraId,
                request.Filtro.Formato,
                request.Filtro.PublicoAlvoIds,
                request.Filtro.NomeFormacao,
                request.Filtro.NumeroHomologacao,
                request.Filtro.PeriodoRealizacaoInicio,
                request.Filtro.PeriodoRealizacaoFim,
                request.Filtro.Situacao,
                request.Filtro.FormacaoHomologada,
                request.Situacoes,
                usuarioLogado.Login,
                perfilUsuarioLogado);
        }
    }
}