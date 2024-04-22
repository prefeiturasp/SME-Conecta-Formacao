using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraUsuarioLogadoQueryHandler : IRequestHandler<ObterAreaPromotoraUsuarioLogadoQuery, Dominio.Entidades.AreaPromotora?>
    {
        private readonly IMediator _mediator;

        public ObterAreaPromotoraUsuarioLogadoQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Dominio.Entidades.AreaPromotora?> Handle(ObterAreaPromotoraUsuarioLogadoQuery request, CancellationToken cancellationToken)
        {
            var grupoUsuarioLogadoId = await _mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia(), cancellationToken);
            if (grupoUsuarioLogadoId == Perfis.ADMIN_DF) 
                return default;
            
            var dres = await _mediator.Send(ObterDresUsuarioLogadoQuery.Instancia(), cancellationToken);

            var areaPromotora = await _mediator.Send(new ObterAreaPromotoraPorGrupoIdEDresQuery(grupoUsuarioLogadoId, dres), cancellationToken) ??
                                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO, System.Net.HttpStatusCode.NotFound);

            return areaPromotora;
        }
    }
}
