using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAlterarProposta : CasoDeUsoAbstrato, ICasoDeUsoAlterarProposta
    {
        const long CODIGO_DRE_TODOS = 14;
        public CasoDeUsoAlterarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<long> Executar(long id, PropostaDTO propostaDTO)
        {

            if (propostaDTO.Situacao == Dominio.Enumerados.SituacaoProposta.Rascunho)
                return await mediator.Send(new AlterarPropostaRascunhoCommand(id, propostaDTO));

            var existeDreTodos = propostaDTO.Turmas.Select(x => x.DresIds).Where(x => x.Contains(CODIGO_DRE_TODOS)).Count();

            if(existeDreTodos > 0)
                throw new NegocioException(MensagemNegocio.DRE_NAO_INFORMADA_PARA_TODAS_AS_TURMAS);

            await SalvarMovimentacao(id, propostaDTO.Situacao);
            return await mediator.Send(new AlterarPropostaCommand(id, propostaDTO));
        }
        private async Task SalvarMovimentacao(long propostaId, SituacaoProposta situacao)
        {
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));
        }
    }
}
