using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaCommandHandler : IRequestHandler<InserirPropostaCommand, long>
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

        public async Task<long> Handle(InserirPropostaCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new ValidarFuncaoEspecificaOutrosCommand(request.PropostaDTO.FuncoesEspecificas, request.PropostaDTO.FuncaoEspecificaOutros), cancellationToken);
            
            await _mediator.Send(new ValidarCriterioValidacaoInscricaoOutrosCommand(request.PropostaDTO.CriteriosValidacaoInscricao, request.PropostaDTO.CriterioValidacaoInscricaoOutros), cancellationToken);

            await _mediator.Send(new ValidarPublicoAlvoFuncaoModalidadeAnoTurmaComponenteCommand(request.PropostaDTO.PublicosAlvo, request.PropostaDTO.FuncoesEspecificas, 
                request.PropostaDTO.Modalidades, request.PropostaDTO.AnosTurmas, request.PropostaDTO.ComponentesCurriculares), cancellationToken);
            
            var proposta = _mapper.Map<Proposta>(request.PropostaDTO);
            proposta.AreaPromotoraId = request.AreaPromotoraId;

            var transacao = _transacao.Iniciar();

            try
            {
                var id = await _repositorioProposta.Inserir(proposta);

                await _mediator.Send(new SalvarPropostaPublicoAlvoCommand(id, proposta.PublicosAlvo), cancellationToken);

                await _mediator.Send(new SalvarPropostaFuncaoEspecificaCommand(id, proposta.FuncoesEspecificas), cancellationToken);

                await _mediator.Send(new SalvarPropostaCriteriosValidacaoInscricaoCommand(id, proposta.CriteriosValidacaoInscricao), cancellationToken);

                await _mediator.Send(new SalvarPropostaVagaRemanecenteCommand(id, proposta.VagasRemanecentes), cancellationToken);

                await _mediator.Send(new SalvarPropostaPalavraChaveCommand(id, proposta.PalavrasChaves), cancellationToken);

                await _mediator.Send(new SalvarCriterioCertificacaoCommand(id, proposta.CriterioCertificacao), cancellationToken);

                await _mediator.Send(new ValidarArquivoImagemDivulgacaoPropostaCommand(proposta.ArquivoImagemDivulgacaoId), cancellationToken);

                await _mediator.Send(new SalvarPropostaDreCommand(id, proposta.Dres), cancellationToken);

                await _mediator.Send(new SalvarPropostaModalidadeCommand(id, proposta.Modalidades), cancellationToken);
                
                await _mediator.Send(new SalvarPropostaAnoTurmaCommand(id, proposta.AnosTurmas), cancellationToken);
                
                await _mediator.Send(new SalvarPropostaComponenteCurricularCommand(id, proposta.ComponentesCurriculares), cancellationToken);

                transacao.Commit();

                return id;
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
