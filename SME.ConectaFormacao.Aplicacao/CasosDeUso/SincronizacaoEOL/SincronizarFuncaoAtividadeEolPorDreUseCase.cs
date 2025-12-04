using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class SincronizarFuncaoAtividadeEolPorDreUseCase(
        IServicoEol servicoEol,
        IRepositorioFuncaoAtividadeUsuario repositorioFuncaoAtividadeUsuario,
        IRepositorioSincronizador repositorioSincronizador) : ISincronizarFuncaoAtividadeEolPorDreUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var codigoDre = param.ObterObjetoMensagem<string>()
                ?? throw new ArgumentNullException(nameof(param), "Parâmetro código DRE não pode ser nulo.");

            var dataUltimaAtualizacao = await repositorioFuncaoAtividadeUsuario.ObterDataUltimaAtualizacaoAsync();

            var origem = await servicoEol.ObterFuncaoAtividadeEolPorDre(codigoDre, dataUltimaAtualizacao);

            var destino = origem?
                .Select(c => new FuncaoAtividadeUsuario
                {
                    CdRegistroFuncional = c.CdRegistroFuncional,
                    CdTipoFuncao = Convert.ToInt32(c.CdTipoFuncao),
                    CdUe = c.CdUe
                })
                .DistinctBy(x => new { x.CdRegistroFuncional, x.CdTipoFuncao, x.CdUe }) // remove duplicados
                .ToList() ?? [];

            await repositorioSincronizador.SincronizarLoteFuncaoAtividadeEolAsync(destino, codigoDre);

            return true;
        }


    }
}