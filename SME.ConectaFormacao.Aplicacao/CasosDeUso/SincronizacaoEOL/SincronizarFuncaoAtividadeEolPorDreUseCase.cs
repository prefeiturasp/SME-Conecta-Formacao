using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class SincronizarFuncaoAtividadeEolPorDreUseCase(IServicoEol servicoEol, IRepositorioSincronizador repositorioSincronizador) : ISincronizarFuncaoAtividadeEolPorDreUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var codigoDre = param.ObterObjetoMensagem<string>()
                ?? throw new ArgumentNullException(nameof(param), "Parâmetro código DRE não pode ser nulo.");

            var original = await servicoEol.ObterFuncaoAtividadeEolPorDre(codigoDre);

            var destino = original?.Select(c => new FuncaoAtividadeUsuario
            {
                CdRegistroFuncional = c.CdRegistroFuncional,
                CdTipoFuncao = c.CdTipoFuncao,
                CdUe = c.CdUe
            }).ToList();

            await repositorioSincronizador.SincronizarLoteFuncaoAtividadeEolAsync(destino, codigoDre);

            return true;
        }

    }
}
