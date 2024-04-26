﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoAlterarProposta : CasoDeUsoAbstrato, ICasoDeUsoAlterarProposta
    {
        public CasoDeUsoAlterarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<RetornoDTO> Executar(long id, PropostaDTO propostaDTO)
        {
            if (propostaDTO.Situacao.EhParaSalvarRascunho())
                return await mediator.Send(new AlterarPropostaRascunhoCommand(id, propostaDTO));

            AlteraSituacaoParaAguardandoParecerista(propostaDTO);

            await SalvarMovimentacao(id, propostaDTO.Situacao);

            return await mediator.Send(new AlterarPropostaCommand(id, propostaDTO));
        }

        private async Task SalvarMovimentacao(long propostaId, SituacaoProposta situacao)
        {
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));
        }

        private void AlteraSituacaoParaAguardandoParecerista(PropostaDTO propostaDTO)
        {
            if (propostaDTO.Situacao.EstaAguardandoAnaliseDf() && 
                propostaDTO.FormacaoHomologada.EstaHomologada())
                propostaDTO.Situacao = SituacaoProposta.AguardandoAnaliseParecerista;
        }
    }
}
