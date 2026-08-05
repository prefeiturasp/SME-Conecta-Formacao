using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class GerenciadorAnexosCodafCursoNaoHomologadoService(
        IRepositorioCodafCursoNaoHomologadoAnexo repositorioCodafCursoNaoHomologadoAnexo,
        IServicoArmazenamento servicoArmazenamento) : IGerenciadorAnexosCodafCursoNaoHomologadoService
    {
        public async Task ProcessarAnexosAsync(long codafCursoNaoHomologadoId, IEnumerable<CodafCursoNaoHomologadoAnexo> anexos)
        {
            var anexosAtuais = await repositorioCodafCursoNaoHomologadoAnexo.ObterPorCodafCursoNaoHomologadoIdAsync(codafCursoNaoHomologadoId);
            var novosAnexos = anexos ?? [];

            var idsNovos = novosAnexos.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaRemover = anexosAtuais.Where(a => !idsNovos.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoRemover in anexosParaRemover)
            {
                await repositorioCodafCursoNaoHomologadoAnexo.Remover(anexoRemover);
            }

            var idsAtuais = anexosAtuais.Select(x => x.ArquivoCodigo).ToHashSet();
            var anexosParaAdicionar = novosAnexos.Where(a => !idsAtuais.Contains(a.ArquivoCodigo)).ToList();

            foreach (var anexoAdicionar in anexosParaAdicionar)
            {
                var novoAnexo = new CodafCursoNaoHomologadoAnexo
                {
                    CodafCursoNaoHomologadoId = codafCursoNaoHomologadoId,
                    ArquivoCodigo = anexoAdicionar.ArquivoCodigo,
                    NomeArquivo = anexoAdicionar.NomeArquivo,
                    Extensao = Path.GetExtension(anexoAdicionar.NomeArquivo).ToLower()
                };

                await repositorioCodafCursoNaoHomologadoAnexo.Inserir(novoAnexo);
                await servicoArmazenamento.MoverGuid(anexoAdicionar.ArquivoCodigo);
            }
        }
    }
}