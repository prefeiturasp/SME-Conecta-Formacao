using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class GerenciadorAnexosCodafSuplementarService(
        IRepositorioCodafSuplementarAnexo repositorioCodafSuplementarAnexo,
        IServicoArmazenamento servicoArmazenamento) : IGerenciadorAnexosCodafSuplementarService
    {
        public async Task ProcessarAnexosAsync(long codafSuplementarId, IEnumerable<CodafSuplementarAnexo> anexos)
        {
            var anexosAtuais = await repositorioCodafSuplementarAnexo.ObterPorCodafSuplementarIdAsync(codafSuplementarId);
            var novosAnexos = anexos ?? [];

            var idsNovos = novosAnexos.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaRemover = anexosAtuais.Where(a => !idsNovos.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoRemover in anexosParaRemover)
            {
                await repositorioCodafSuplementarAnexo.Remover(anexoRemover);
            }

            var idsAtuais = anexosAtuais.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaAdicionar = novosAnexos.Where(a => !idsAtuais.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoAdicionar in anexosParaAdicionar)
            {
                var novoAnexo = new CodafSuplementarAnexo
                {
                    CodafSuplementarId = codafSuplementarId,
                    ArquivoCodigo = anexoAdicionar.ArquivoCodigo,
                    NomeArquivo = anexoAdicionar.NomeArquivo,
                    Extensao = Path.GetExtension(anexoAdicionar.NomeArquivo).ToLower(),
                    TipoAnexoId = anexoAdicionar.TipoAnexoId
                };

                await repositorioCodafSuplementarAnexo.Inserir(novoAnexo);
                await servicoArmazenamento.MoverGuid(anexoAdicionar.ArquivoCodigo);
            }
        }
    }
}