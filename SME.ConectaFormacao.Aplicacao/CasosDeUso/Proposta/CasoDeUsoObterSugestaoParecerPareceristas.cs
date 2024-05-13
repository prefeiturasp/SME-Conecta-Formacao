﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterSugestaoParecerPareceristas : CasoDeUsoAbstrato, ICasoDeUsoObterSugestaoParecerPareceristas
    {
        public CasoDeUsoObterSugestaoParecerPareceristas(IMediator mediator) : base(mediator)
        {
        }

        public async Task<IEnumerable<PropostaPareceristaSugestaoDTO>> Executar(long propostaId)
        {
            return await mediator.Send(new ObterSugestoesPareceristasQuery(propostaId));
        }
    }
}
