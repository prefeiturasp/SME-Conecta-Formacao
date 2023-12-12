using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaRascunhoCommandHandler : IRequestHandler<InserirPropostaRascunhoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMediator _mediator;

        public InserirPropostaRascunhoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(InserirPropostaRascunhoCommand request, CancellationToken cancellationToken)
        {
            var proposta = _mapper.Map<Proposta>(request.PropostaDTO);
            proposta.AreaPromotoraId = request.AreaPromotoraId;
            proposta.Situacao = SituacaoProposta.Rascunho;

            var transacao = _transacao.Iniciar();

            try
            {
                var id = await _repositorioProposta.Inserir(proposta);

                await _mediator.Send(new SalvarPropostaPublicoAlvoCommand(id, proposta.PublicosAlvo), cancellationToken);

                await _mediator.Send(new SalvarPropostaFuncaoEspecificaCommand(id, proposta.FuncoesEspecificas), cancellationToken);

                await _mediator.Send(new SalvarPropostaCriteriosValidacaoInscricaoCommand(id, proposta.CriteriosValidacaoInscricao), cancellationToken);

                await _mediator.Send(new SalvarPropostaVagaRemanecenteCommand(id, proposta.VagasRemanecentes), cancellationToken);

                await _mediator.Send(new SalvarPropostaPalavraChaveCommand(id, proposta.PalavrasChaves), cancellationToken);

                await _mediator.Send(new ValidarArquivoImagemDivulgacaoPropostaCommand(proposta.ArquivoImagemDivulgacaoId), cancellationToken);

                await _mediator.Send(new SalvarPropostaDreCommand(id, proposta.Dres), cancellationToken);

                await _mediator.Send(new SalvarPropostaTurmaCommand(id, proposta.Turmas), cancellationToken);
                
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
