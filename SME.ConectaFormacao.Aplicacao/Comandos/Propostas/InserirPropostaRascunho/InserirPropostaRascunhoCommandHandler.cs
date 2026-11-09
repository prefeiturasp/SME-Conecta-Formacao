using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaRascunhoCommandHandler : IRequestHandler<InserirPropostaRascunhoCommand, RetornoDTO>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;
        private readonly IServicoLogs? _servicoLogs;

        public InserirPropostaRascunhoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta, IMediator mediator, IServicoLogs? servicoLogs = null)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _servicoLogs = servicoLogs;
        }

        public async Task<RetornoDTO> Handle(InserirPropostaRascunhoCommand request, CancellationToken cancellationToken)
        {
            var proposta = _mapper.Map<Proposta>(request.PropostaDTO);
            proposta.AreaPromotoraId = request.AreaPromotoraId;
            proposta.Situacao = SituacaoProposta.Rascunho;
            proposta.AjustarDatasInscricao();

            var id = await PropostaTransacaoHelper.InserirComTransacaoAsync(
                _transacao,
                _repositorioProposta,
                _mediator,
                _servicoLogs,
                proposta,
                request.PropostaDTO.IntegrarNoSGA,
                "InserirPropostaRascunho",
                cancellationToken);

            return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PROPOSTA_X_INSERIDA_COM_SUCESSO, id), id);
        }
    }
}
