using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares
{
    public class CasoDeUsoGerarRelatorioCodafSuplementar(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IServicoRelatorio servicoRelatorio) :
        ICasoDeUsoGerarRelatorioCodafSuplementar    
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafListaPresencaId)
        {
            var listaPresenca = await repositorioCodafListaPresenca.ObterPorIdComPropostaEPropostaTurmaAsync(codafListaPresencaId);
            if (listaPresenca == null)  
                return Erro.NaoEncontrado();

            var nomeArquivo = $"CODAF_{listaPresenca.Proposta.NumeroHomologacao}-{listaPresenca.PropostaTurma.Nome}.xlsx";
            var arquivoDto = await GerarRelatorioCodafAsyncSuplementar(codafListaPresencaId, nomeArquivo);
            await AtualizarStatusParaFinalizadoAsync(listaPresenca);
            return arquivoDto;
        }

        private async Task<ArquivoDto> GerarRelatorioCodafAsyncSuplementar(long codafListaPresencaId, string nomeArquivo)
        {
            var arquivoBytes = await servicoRelatorio.GerarRelatorioCodafSuplementarAsync(codafListaPresencaId);    
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return new ArquivoDto(nomeArquivo, contentType, new MemoryStream(arquivoBytes, writable: false));
        }

        private async Task AtualizarStatusParaFinalizadoAsync(CodafListaPresenca listaPresenca)
        {
            if (listaPresenca.Status == StatusCodafListaPresenca.Finalizado)
                return;

            listaPresenca.Finalizar();
            await repositorioCodafListaPresenca.Atualizar(listaPresenca);

        }
    }
}
