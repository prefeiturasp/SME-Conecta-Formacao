using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInformacoesCadastranteQueryHandler : IRequestHandler<ObterInformacoesCadastranteQuery, PropostaInformacoesCadastranteDTO>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ObterInformacoesCadastranteQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PropostaInformacoesCadastranteDTO> Handle(ObterInformacoesCadastranteQuery request, CancellationToken cancellationToken)
        {
            AreaPromotora areaPromotora;
            Usuario usuario;

            if (request.PropostaId.HasValue)
            {
                var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(request.PropostaId.Value), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

                areaPromotora = await _mediator.Send(new ObterAreaPromotoraPorIdQuery(proposta.AreaPromotoraId), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

                usuario = await _mediator.Send(new ObterUsuarioPorLoginQuery(proposta.CriadoLogin), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO, System.Net.HttpStatusCode.NotFound);
            }
            else
            {
                var grupoUsuarioLogadoId = await _mediator.Send(ObterGrupoUsuarioLogadoQuery.Instancia(), cancellationToken);
                var dres = await _mediator.Send(ObterDresUsuarioLogadoQuery.Instancia(), cancellationToken);

                areaPromotora = await _mediator.Send(new ObterAreaPromotoraPorGrupoIdEDresQuery(grupoUsuarioLogadoId, dres), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO, System.Net.HttpStatusCode.NotFound);

                usuario = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia(), cancellationToken);
            }

            var informacoesCadastrante = _mapper.Map<PropostaInformacoesCadastranteDTO>(areaPromotora);

            informacoesCadastrante.UsuarioLogadoNome = usuario.Nome;
            informacoesCadastrante.UsuarioLogadoEmail = usuario.Email;

            return informacoesCadastrante;
        }
    }
}
