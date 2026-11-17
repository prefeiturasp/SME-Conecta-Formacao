using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterNumeroHomologacaoProposta(
        IRepositorioProposta repositorioProposta) : ICasoDeUsoObterNumeroHomologacaoProposta
    {
        public async Task<Resultado<PropostaNumeroHomologacaoDto>> ExecutarAsync(long propostaId)
        {
            if (propostaId <= 0)
                return Resultado<PropostaNumeroHomologacaoDto>.DeSucesso(new PropostaNumeroHomologacaoDto { NumeroHomologacao = null });

            var numeroHomologacao = await repositorioProposta.ObterNumeroHomologacaoPorPropostaIdAsync(propostaId);

            return Resultado<PropostaNumeroHomologacaoDto>.DeSucesso(new PropostaNumeroHomologacaoDto
            {
                NumeroHomologacao = numeroHomologacao
            });
        }
    }
}
