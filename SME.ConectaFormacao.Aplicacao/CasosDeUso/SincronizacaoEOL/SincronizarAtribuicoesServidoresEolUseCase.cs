using SME.ConectaFormacao.Aplicacao.Interfaces.SincronizacaoEOL;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.SincronizacaoEOL
{
    public class SincronizarAtribuicoesServidoresEolUseCase(
        IServicoEol servicoEol,
        IRepositorioAtribuicaoAulaServidor repositorioAtribuicaoAulaServidor,
        IRepositorioSincronizador repositorioSincronizador) : ISincronizarAtribuicoesServidoresEolUseCase
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var dataUltimaAtualizacao = await repositorioAtribuicaoAulaServidor.ObterDataUltimaAtualizacaoAsync();

            var atribuicoesServidoresEol = await servicoEol.ObterAtribuicoesServidorEolPorDataAtualizacaoAsync(dataUltimaAtualizacao);
            if (atribuicoesServidoresEol is null || !atribuicoesServidoresEol.Any())
                return true;
            
            atribuicoesServidoresEol = [.. atribuicoesServidoresEol.DistinctBy(a => new
            {
                a.Modalidade,
                a.AnoSerie,
                a.CdComponenteCurricular,
                a.CdRegistroFuncional,
                a.CodigoUe
            })];

            var excluidos = atribuicoesServidoresEol
                            .Where(atribuicao => atribuicao.Excluido)
                            .ToList();
            var upserts = atribuicoesServidoresEol.Except(excluidos).Select(atribuicao => new AtribuicaoServidorEol(
                atribuicao.Modalidade, atribuicao.AnoSerie, atribuicao.CdComponenteCurricular, 
                atribuicao.CdRegistroFuncional, atribuicao.CodigoUe, atribuicao.ChaveNegocio)).ToList();

            var chavesParaLimpeza = atribuicoesServidoresEol
                .Select(atribuicao => atribuicao.ChaveNegocio)
                .ToList();

            await repositorioSincronizador.LimparAtribuicaoServidorEolAsync(chavesParaLimpeza);
            await repositorioSincronizador.SincronizarLoteAtribuicaoServidorEolAsync(upserts);
            return true;
        }
    }
}