using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
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

            var arquivoBytes = await servicoRelatorio.GerarRelatorioCodafAsync(codafId);
            var nomeArquivo = $"CODAF_{listaPresenca.Proposta.NumeroHomologacao}-{listaPresenca.PropostaTurma.Nome}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var stream = new MemoryStream(arquivoBytes, writable: false);
            return new ArquivoDto(nomeArquivo, contentType, stream);
        }
    }
}
