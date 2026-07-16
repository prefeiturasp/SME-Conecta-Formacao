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
    public class SalvarPropostaRegenteCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta, ITransacao transacao, IMediator mediator) : 
        IRequestHandler<SalvarPropostaRegenteCommand, long>
    {
        private readonly ITransacao _transacao = transacao;

        public async Task<long> Handle(SalvarPropostaRegenteCommand request, CancellationToken cancellationToken)
        {
            var regenteAntes = await repositorioProposta.ObterPropostaRegentePorId(request.PropostaRegenteDTO.Id);
            var regenteDepois = mapper.Map<PropostaRegente>(request.PropostaRegenteDTO);

            if (regenteDepois.Cpf is not null && !regenteDepois.Cpf.CpfEhValido())
                throw new NegocioException(MensagemNegocio.CPF_INVALIDO);

            var turmasAntes = await repositorioProposta.ObterRegenteTurmasPorRegenteId(regenteDepois.Id);
            var arrayTurma = request.PropostaRegenteDTO.Turmas.Select(x => x.TurmaId);
            var turmasConsultar = arrayTurma.Where(w => !turmasAntes.Any(a => a.TurmaId == w)).ToArray();

            await mediator.Send(new ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand(regenteDepois.RegistroFuncional, regenteDepois.Cpf, regenteDepois.NomeRegente, turmasConsultar), cancellationToken);

            var transacaoDb = _transacao.Iniciar();
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
                        await repositorioProposta.AtualizarPropostaRegente(regenteDepois);
                    }
                }
                else
                    await repositorioProposta.InserirPropostaRegente(request.PropostaId, regenteDepois);

                var turmasInserir = regenteDepois.Turmas.Where(w => !turmasAntes.Any(a => a.Id == w.Id));
                var turmasExcluir = turmasAntes.Where(w => !regenteDepois.Turmas.Any(a => a.Id == w.Id));
                if (turmasInserir.Any())
                    await repositorioProposta.InserirPropostaRegenteTurma(regenteDepois.Id, turmasInserir);

                if (turmasExcluir.Any())
                    await repositorioProposta.ExcluirPropostaRegenteTurmas(turmasExcluir);

                transacaoDb.Commit();
                return regenteDepois.Id;
            }
            catch
            {
                transacaoDb.Rollback();
                throw;
            }
            finally
            {
                transacaoDb.Dispose();
            }
        }
    }
}