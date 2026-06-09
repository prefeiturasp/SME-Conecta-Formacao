using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class PropostaGrupoPeriodoService(IRepositorioPropostaGrupoPeriodo repositorioPropostaGrupoPeriodo) : IPropostaGrupoPeriodoService
    {
        public async Task ProcessarGruposAsync(long propostaId, IEnumerable<PropostaGrupoPeriodo> grupos)
        {
            if (grupos == null || !grupos.Any())
                return;

            foreach (var grupo in grupos)
            {
                
            }
        }
    }
}
