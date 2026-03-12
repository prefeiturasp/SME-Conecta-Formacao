using SME.ConectaFormacao.Infra.Dados.Dtos.InscritosPorFormacao;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public interface IGeradorRelatorioInscritosExcelService
    {
        Task<string> GerarEArmazenarRelatorioAsync(RelatorioInscritosFormacaoDto dados);
    }
}