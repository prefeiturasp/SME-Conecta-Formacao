using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaRegenteCommandHandler : IRequestHandler<SalvarPropostaRegenteCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;

        public SalvarPropostaRegenteCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta, ITransacao transacao, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(SalvarPropostaRegenteCommand request, CancellationToken cancellationToken)
        {
            var regenteAntes = await _repositorioProposta.ObterPropostaRegentePorId(request.PropostaRegenteDTO.Id);
            var regenteDepois = _mapper.Map<PropostaRegente>(request.PropostaRegenteDTO);

            if (regenteDepois.Cpf.NaoEhNulo() && !regenteDepois.Cpf.CpfEhValido())
                throw new NegocioException(MensagemNegocio.CPF_INVALIDO);

            var turmasAntes = await _repositorioProposta.ObterRegenteTurmasPorRegenteId(regenteDepois.Id);
            var arrayTurma = request.PropostaRegenteDTO.Turmas.Select(x => x.TurmaId);
            var turmasConsultar = arrayTurma.Where(w => !turmasAntes.Any(a => a.TurmaId == w)).ToArray();

            await _mediator.Send(new ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand(regenteDepois.RegistroFuncional, regenteDepois.Cpf, regenteDepois.NomeRegente, turmasConsultar), cancellationToken);

            var transacao = _transacao.Iniciar();
            try
            {
                if (regenteAntes != null)
                {
                    if (regenteAntes.ProfissionalRedeMunicipal != regenteDepois.ProfissionalRedeMunicipal
                        || regenteAntes.RegistroFuncional != regenteDepois.RegistroFuncional
                        || regenteAntes.NomeRegente != regenteDepois.NomeRegente
                        || regenteAntes.MiniBiografia != regenteDepois.MiniBiografia
                        || regenteAntes.Cpf != regenteDepois.Cpf)
                    {
                        regenteDepois.PropostaId = request.PropostaId;
                        regenteDepois.ManterCriador(regenteAntes);
                        await _repositorioProposta.AtualizarPropostaRegente(regenteDepois);
                    }
                }
                else
                    await _repositorioProposta.InserirPropostaRegente(request.PropostaId, regenteDepois);

                var turmasInserir = regenteDepois.Turmas.Where(w => !turmasAntes.Any(a => a.Id == w.Id));
                var turmasExcluir = turmasAntes.Where(w => !regenteDepois.Turmas.Any(a => a.Id == w.Id));
                if (turmasInserir.Any())
                    await _repositorioProposta.InserirPropostaRegenteTurma(regenteDepois.Id, turmasInserir);

                if (turmasExcluir.Any())
                    await _repositorioProposta.ExcluirPropostaRegenteTurmas(turmasExcluir);

                transacao.Commit();
                return regenteDepois.Id;
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