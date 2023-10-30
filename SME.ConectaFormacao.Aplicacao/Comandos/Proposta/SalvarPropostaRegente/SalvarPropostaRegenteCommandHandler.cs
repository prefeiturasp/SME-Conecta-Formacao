using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Proposta.SalvarPropostaRegente
{
    public class SalvarPropostaRegenteCommandHandler : IRequestHandler<SalvarPropostaRegenteCommand, long>
    {
        public SalvarPropostaRegenteCommandHandler(IMapper mapper, IRepositorioProposta repositorioProposta, ITransacao transacao)
        {
            _mapper = mapper;
            _repositorioProposta = repositorioProposta;
            _transacao = transacao;
        }

        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;

        public async Task<long> Handle(SalvarPropostaRegenteCommand request, CancellationToken cancellationToken)
        {
            var regenteAntes = await _repositorioProposta.ObterPropostaRegentePorId(request.propostaRegenteDTO.Id);

            var regenteDepois = _mapper.Map<PropostaRegente>(request.propostaRegenteDTO);
            var transacao = _transacao.Iniciar();
            try
            {
                if (regenteAntes != null)
                {
                    if (regenteAntes.ProfissionalRedeMunicipal != regenteDepois.ProfissionalRedeMunicipal
                        || regenteAntes.RegistroFuncional != regenteDepois.RegistroFuncional
                        || regenteAntes.NomeRegente != regenteDepois.NomeRegente
                        || regenteAntes.MiniBiografia != regenteDepois.MiniBiografia)
                    {
                        regenteDepois.PropostaId = request.PropostaId;
                        regenteDepois.ManterCriador(regenteAntes);
                        await _repositorioProposta.AtualizarPropostaRegente(regenteDepois);
                    }
                }
                else
                    await _repositorioProposta.InserirPropostaRegente(request.PropostaId, regenteDepois);
                
                var turmasAntes = await _repositorioProposta.ObterRegenteTurmasPorRegenteId(regenteDepois.Id);
                var turmasInserir = regenteDepois.Turmas.Where(w => !turmasAntes.Any(a => a.Id == w.Id));
                var turmasExcluir = turmasAntes.Where(w => !regenteDepois.Turmas.Any(a => a.Id == w.Id));

                if (turmasInserir.Any())
                    await _repositorioProposta.InserirPropostaRegenteTurma(regenteDepois.Id, turmasInserir);

                if (turmasExcluir.Any())
                    await _repositorioProposta.ExcluirPropostaRegenteTurmas(turmasExcluir);

                transacao.Commit();
                return regenteDepois.Id;
            }
            catch (Exception e)
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