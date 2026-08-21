using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafDeclaracoes;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes
{
    public class CasoDeUsoObterDeclaracaoCodafParaDownload(
        IRepositorioCodafDeclaracao repositorioCodafDeclaracao,
        IServicoArmazenamento servicoArmazenamento) : ICasoDeUsoObterDeclaracaoCodafParaDownload
    {
        public async Task<Resultado<CodafDeclaracaoParaDownloadDto>> ExecutarAsync(long declaracaoCodafId)
        {
            var declaracaoCodaf = await repositorioCodafDeclaracao.ObterDeclaracaoDisponivelDoUsuarioAsync(declaracaoCodafId);
            if (declaracaoCodaf == null)
                return Erro.NaoEncontrado("Declaração CODAF não encontrada para o ID informado.");
            if (string.IsNullOrEmpty(declaracaoCodaf.ChaveObjetoArmazenamento))
                return Erro.Validacao("Declaração CODAF não possui arquivo associado para download.");
            var urlArquivo = await servicoArmazenamento.ObterUrlPorChaveObjetoAsync(declaracaoCodaf.ChaveObjetoArmazenamento);
            if (urlArquivo == null)
                return Erro.NaoEncontrado("Não foi possível obter o arquivo da declaração CODAF.");
            var arquivoDto = new CodafDeclaracaoParaDownloadDto
            {
                Id = declaracaoCodaf.Id,
                UrlDownload = urlArquivo,
                NomeCompleto = declaracaoCodaf.NomeCompleto,
                NomeFormacao = declaracaoCodaf.NomeFormacao
            };
            return arquivoDto;
        }
    }
}
