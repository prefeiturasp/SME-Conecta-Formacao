﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.AreaPromotora
{
    public class CasoDeUsoObterAreaPromotoraLista : CasoDeUsoAbstrato, ICasoDeUsoObterAreaPromotoraLista
    {
        public CasoDeUsoObterAreaPromotoraLista(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Executar()
        {
            var areaPromotoraUsuarioLogado = await mediator.Send(ObterAreaPromotoraUsuarioLogadoQuery.Instancia());
            return await mediator.Send(new ObterAreaPromotoraListaQuery(areaPromotoraUsuarioLogado?.Id));
        }
    }
}
