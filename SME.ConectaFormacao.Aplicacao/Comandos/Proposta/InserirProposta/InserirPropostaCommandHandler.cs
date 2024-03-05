using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaCommandHandler : IRequestHandler<InserirPropostaCommand, RetornoDTO>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;

        public InserirPropostaCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<RetornoDTO> Handle(InserirPropostaCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);

            await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);

            await _mediator.Send(new ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand(request.PropostaDTO.PublicosAlvo, request.PropostaDTO.FuncoesEspecificas,
                request.PropostaDTO.Modalidades, request.PropostaDTO.AnosTurmas, request.PropostaDTO.ComponentesCurriculares), cancellationToken);

            var proposta = _mapper.Map<Proposta>(request.PropostaDTO);
            proposta.AreaPromotoraId = request.AreaPromotoraId;

            await _mediator.Send(new ValidarAreaPromotoraCommand(proposta.AreaPromotoraId, proposta.IntegrarNoSGA), cancellationToken);

            var transacao = _transacao.Iniciar();

            try
            {
                var id = await _repositorioProposta.Inserir(proposta);

                await _mediator.Send(new SalvarPropostaCommand(id, proposta, null), cancellationToken);

                transacao.Commit();

                return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PROPOSTA_X_INSERIDA_COM_SUCESSO, id), id);
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
