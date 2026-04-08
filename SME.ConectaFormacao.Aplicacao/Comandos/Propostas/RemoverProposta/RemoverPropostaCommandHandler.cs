using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaCommandHandler(
        ITransacao transacao,
        IRepositorioProposta repositorioProposta,
        IRepositorioPropostaEncontro repositorioPropostaEncontro) : 
        IRequestHandler<RemoverPropostaCommand, bool>
    {
        public async Task<bool> Handle(RemoverPropostaCommand request, CancellationToken cancellationToken)
        {
            var proposta = await repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            proposta.Dres = await repositorioProposta.ObterDrePorId(request.Id);
            proposta.PublicosAlvo = await repositorioProposta.ObterPublicoAlvoPorId(request.Id);
            proposta.FuncoesEspecificas = await repositorioProposta.ObterFuncoesEspecificasPorId(request.Id);
            proposta.CriteriosValidacaoInscricao = await repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.Id);
            proposta.VagasRemanecentes = await repositorioProposta.ObterVagasRemacenentesPorId(request.Id);
            proposta.Encontros = await repositorioPropostaEncontro.ObterEncontrosPorPropostaAsync(request.Id);
            proposta.PalavrasChaves = await repositorioProposta.ObterPalavrasChavesPorId(request.Id);
            proposta.Turmas = await repositorioProposta.ObterTurmasPorId(request.Id);
            proposta.Modalidades = await repositorioProposta.ObterModalidadesPorId(request.Id);
            proposta.AnosTurmas = await repositorioProposta.ObterAnosTurmasPorId(request.Id);
            proposta.ComponentesCurriculares = await repositorioProposta.ObterComponentesCurricularesPorId(request.Id);

            var transacaoAtual = transacao.Iniciar();
            try
            {
                if (proposta.Dres.Any())
                    await repositorioProposta.RemoverDres(proposta.Dres);

                if (proposta.PublicosAlvo.Any())
                    await repositorioProposta.RemoverPublicosAlvo(proposta.PublicosAlvo);

                if (proposta.FuncoesEspecificas.Any())
                    await repositorioProposta.RemoverFuncoesEspecificas(proposta.FuncoesEspecificas);

                if (proposta.CriteriosValidacaoInscricao.Any())
                    await repositorioProposta.RemoverCriteriosValidacaoInscricao(proposta.CriteriosValidacaoInscricao);

                if (proposta.VagasRemanecentes.Any())
                    await repositorioProposta.RemoverVagasRemanecentes(proposta.VagasRemanecentes);

                if (proposta.Encontros.Any())
                    await repositorioPropostaEncontro.RemoverEncontrosAsync(proposta.Encontros);

                if (proposta.PalavrasChaves.Any())
                    await repositorioProposta.RemoverPalavrasChaves(proposta.PalavrasChaves);

                if (proposta.Turmas.Any())
                    await repositorioProposta.RemoverTurmas(proposta.Turmas);

                if (proposta.Modalidades.Any())
                    await repositorioProposta.RemoverModalidades(proposta.Modalidades);

                if (proposta.AnosTurmas.Any())
                    await repositorioProposta.RemoverAnosTurmas(proposta.AnosTurmas);

                if (proposta.ComponentesCurriculares.Any())
                    await repositorioProposta.RemoverComponentesCurriculares(proposta.ComponentesCurriculares);

                await repositorioProposta.RemoverPropostaMovimentacao(proposta.Id);
                await repositorioProposta.Remover(proposta);

                transacaoAtual.Commit();

                return true;
            }
            catch
            {
                transacaoAtual.Rollback();
                throw;
            }
            finally
            {
                transacaoAtual.Dispose();
            }
        }
    }
}
