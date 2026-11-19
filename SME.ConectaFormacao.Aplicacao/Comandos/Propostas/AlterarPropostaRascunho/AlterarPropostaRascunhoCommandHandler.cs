using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Aplicacao.Utilitarios;
using SME.ConectaFormacao.Infra.Servicos.Log;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaRascunhoCommandHandler : IRequestHandler<AlterarPropostaRascunhoCommand, RetornoDTO>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;
        private readonly IServicoLogs? _servicoLogs;

        public AlterarPropostaRascunhoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta, IMediator mediator, IServicoLogs? servicoLogs = null)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _servicoLogs = servicoLogs;
        }

        public async Task<RetornoDTO> Handle(AlterarPropostaRascunhoCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? 
                           throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);

            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.IntegrarNoSGA = request.PropostaDTO.IntegrarNoSGA ?? proposta.IntegrarNoSGA;
            propostaDepois.AjustarDatasInscricao();
            propostaDepois.ManterCriador(proposta);
            propostaDepois.AcaoFormativaTexto = proposta.AcaoFormativaTexto;
            propostaDepois.AcaoFormativaLink = proposta.AcaoFormativaLink;

            await PropostaTransacaoHelper.AtualizarComTransacaoAsync(
                new AtualizarPropostaTransacaoParametros
                {
                    TransacaoManager = _transacao,
                    RepositorioProposta = _repositorioProposta,
                    Mediator = _mediator,
                    ServicoLogs = _servicoLogs,
                    PropostaBanco = proposta,
                    PropostaDepois = propostaDepois,
                    IntegrarNoSgaDto = request.PropostaDTO.IntegrarNoSGA,
                    Operacao = "AlterarPropostaRascunho",
                    CancellationToken = cancellationToken
                }); 

            return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PROPOSTA_X_ALTERADA_COM_SUCESSO, request.Id), request.Id);
        }
    }
}
