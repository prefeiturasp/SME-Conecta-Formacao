using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class GerenciadorAnexosCodafConferenciaService(
        IRepositorioCodafConferenciaAnexo repositorioCodafConferenciaAnexo,
        IServicoArmazenamento servicoArmazenamento) : IGerenciadorAnexosCodafConferenciaService
    {
        public async Task ProcessarAnexosAsync(long codafConferenciaId, IEnumerable<CodafConferenciaAnexo> anexos)
        {
            var anexosAtuais = await repositorioCodafConferenciaAnexo.ObterPorCodafConferenciaIdAsync(codafConferenciaId);
            var novosAnexos = anexos ?? [];

            var idsNovos = novosAnexos.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaRemover = anexosAtuais.Where(a => !idsNovos.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoRemover in anexosParaRemover)
            {
                await repositorioCodafConferenciaAnexo.Remover(anexoRemover);
            }

            var idsAtuais = anexosAtuais.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaAdicionar = novosAnexos.Where(a => !idsAtuais.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoAdicionar in anexosParaAdicionar)
            {
                var novoAnexo = new CodafConferenciaAnexo
                {
                    CodafConferenciaId = codafConferenciaId,
                    ArquivoCodigo = anexoAdicionar.ArquivoCodigo,
                    NomeArquivo = anexoAdicionar.NomeArquivo,
                    Extensao = Path.GetExtension(anexoAdicionar.NomeArquivo).ToLower()
                };

                await repositorioCodafConferenciaAnexo.Inserir(novoAnexo);
                await servicoArmazenamento.MoverGuid(anexoAdicionar.ArquivoCodigo);
            }
        }
    }
}
