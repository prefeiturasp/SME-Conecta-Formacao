using SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Relatorios
{
    public class CasoDeUsoObterRelatorioLaudaCompletaDocx(
        IRepositorioProposta repositorioProposta,
        IGeradorLaudaDocxService geradorDocxService) : ICasoDeUsoObterRelatorioLaudaCompletaDocx
    {
        public async Task<byte[]> ExecutarAsync(long propostaId)
        {
            var dadosProposta = await repositorioProposta.ObterDadosLaudaCompletaAsync(propostaId) ?? 
                                throw new NegocioException("Dados da proposta não encontrados para geração da lauda.");

            return await geradorDocxService.GerarArquivoLaudaCompletaAsync(dadosProposta);
        }
    }
}
