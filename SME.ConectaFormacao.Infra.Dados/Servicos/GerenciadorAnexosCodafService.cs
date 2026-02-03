using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class GerenciadorAnexosCodafService(
        IRepositorioCodafAnexo repositorioCodafAnexo,
        IServicoArmazenamento servicoArmazenamento) : IGerenciadorAnexosCodafService
    {
        public async Task ProcessarAnexosAsync(long codafListaPresencaId, IEnumerable<CodafAnexo> anexos)
        {
            var anexosAtuais = await repositorioCodafAnexo.ObterPorCodafIdAsync(codafListaPresencaId);
            var novosAnexos = anexos ?? [];

            var idsNovos = novosAnexos.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaRemover = anexosAtuais.Where(a => !idsNovos.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoRemover in anexosParaRemover)
            {
                await repositorioCodafAnexo.Remover(anexoRemover);
            }

            var idsAtuais = anexosAtuais.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaAdicionar = novosAnexos.Where(a => !idsAtuais.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoAdicionar in anexosParaAdicionar)
            {
                var novoAnexo = new CodafAnexo
                {
                    CodafListaPresencaId = codafListaPresencaId,
                    ArquivoCodigo = anexoAdicionar.ArquivoCodigo,
                    NomeArquivo = anexoAdicionar.NomeArquivo,
                    Extensao = Path.GetExtension(anexoAdicionar.NomeArquivo).ToLower(),
                    TipoAnexoId = anexoAdicionar.TipoAnexoId
                };

                await repositorioCodafAnexo.Inserir(novoAnexo);
                await servicoArmazenamento.MoverGuid(anexoAdicionar.ArquivoCodigo);
            }
        }
    }
}
