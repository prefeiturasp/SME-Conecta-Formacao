using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoGerarRelatorioCodaf(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IServicoRelatorio servicoRelatorio) :
        ICasoDeUsoGerarRelatorioCodaf
    {
        public async Task<Resultado<ArquivoDto>> ExecutarAsync(long codafId)
        {
            var listaPresenca = await repositorioCodafListaPresenca.ObterPorIdComPropostaEPropostaTurmaAsync(codafId);
            if (listaPresenca == null)
                return Erro.NaoEncontrado();

            var arquivoDto = await GerarRelatorioCodafAsync(codafId);
            await AtualizarStatusParaFinalizadoAsync(listaPresenca);
            return arquivoDto;
        }

        private async Task<ArquivoDto> GerarRelatorioCodafAsync(long codafId)
        {
            var arquivoBytes = await servicoRelatorio.GerarRelatorioCodafAsync(codafId);
            var nomeArquivo = $"CODAF_{codafId}.xlsx";
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
