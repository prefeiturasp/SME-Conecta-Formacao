using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaCommandHandler : IRequestHandler<RemoverPropostaCommand, bool>
    {
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;

        public RemoverPropostaCommandHandler(ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(RemoverPropostaCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            proposta.Dres = await _repositorioProposta.ObterDrePorId(request.Id);
            proposta.PublicosAlvo = await _repositorioProposta.ObterPublicoAlvoPorId(request.Id);
            proposta.FuncoesEspecificas = await _repositorioProposta.ObterFuncoesEspecificasPorId(request.Id);
            proposta.CriteriosValidacaoInscricao = await _repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.Id);
            proposta.VagasRemanecentes = await _repositorioProposta.ObterVagasRemacenentesPorId(request.Id);
            proposta.Encontros = await _repositorioProposta.ObterEncontrosPorId(request.Id);
            proposta.PalavrasChaves = await _repositorioProposta.ObterPalavrasChavesPorId(request.Id);
            proposta.Turmas = await _repositorioProposta.ObterTurmasPorId(request.Id);
            proposta.Modalidades = await _repositorioProposta.ObterModalidadesPorId(request.Id);
            proposta.AnosTurmas = await _repositorioProposta.ObterAnosTurmasPorId(request.Id);
            proposta.ComponentesCurriculares = await _repositorioProposta.ObterComponentesCurricularesPorId(request.Id);
            
            var transacao = _transacao.Iniciar();
            try
            {
                if (proposta.Dres.Any())
                    await _repositorioProposta.RemoverDres(proposta.Dres);

                if (proposta.PublicosAlvo.Any())
                    await _repositorioProposta.RemoverPublicosAlvo(proposta.PublicosAlvo);

                if (proposta.FuncoesEspecificas.Any())
                    await _repositorioProposta.RemoverFuncoesEspecificas(proposta.FuncoesEspecificas);

                if (proposta.CriteriosValidacaoInscricao.Any())
                    await _repositorioProposta.RemoverCriteriosValidacaoInscricao(proposta.CriteriosValidacaoInscricao);

                if (proposta.VagasRemanecentes.Any())
                    await _repositorioProposta.RemoverVagasRemanecentes(proposta.VagasRemanecentes);

                if (proposta.Encontros.Any())
                    await _repositorioProposta.RemoverEncontros(proposta.Encontros);

                if (proposta.PalavrasChaves.Any())
                    await _repositorioProposta.RemoverPalavrasChaves(proposta.PalavrasChaves);

                if (proposta.Turmas.Any())
                    await _repositorioProposta.RemoverTurmas(proposta.Turmas);

                if (proposta.Modalidades.Any())
                    await _repositorioProposta.RemoverModalidades(proposta.Modalidades);

                if (proposta.AnosTurmas.Any())
                    await _repositorioProposta.RemoverAnosTurmas(proposta.AnosTurmas);
                
                if (proposta.ComponentesCurriculares.Any())
                    await _repositorioProposta.RemoverComponentesCurriculares(proposta.ComponentesCurriculares);
                
                await _repositorioProposta.Remover(proposta);

                transacao.Commit();

                return true;
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
