using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class GerenciadorMovimentacaoCodafService(
        IRepositorioCodafListaPresenca repositorioCodaf,
        IRepositorioCodafMovimentacaoListaPresenca repositorioMovimentacao)
        : IGerenciadorMovimentacaoCodafService
    {
        public async Task RegistrarMovimentacaoAsync(CodafListaPresenca codaf, long? comentarioId = null)
        {
            await repositorioCodaf.Atualizar(codaf);
            if (codaf.Status != StatusCodafListaPresenca.AguardandoDf)
            {
                var ultimaMovimentacao = await repositorioMovimentacao.ObterUltimaMovimentacaoPorListaPresencaIdAsync(codaf.Id);
                if (ultimaMovimentacao != null && ultimaMovimentacao.StatusCodafListaPresenca == codaf.Status)
                    return;
            }
            await repositorioMovimentacao.InserirAsync(new CodafMovimentacaoListaPresenca
            {
                CodafListaPresencaId = codaf.Id,
                StatusCodafListaPresenca = codaf.Status,
                CodafComentarioListaPresencaId = comentarioId
            });
        }
    }
}
