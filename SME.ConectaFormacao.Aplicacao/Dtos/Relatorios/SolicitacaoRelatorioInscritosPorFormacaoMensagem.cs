using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Relatorios
{
    public record SolicitacaoRelatorioInscritosPorFormacaoMensagem(
        Guid RelatorioId,
        UsuarioContextoDto Solicitante,
        DateTime DataSolicitacao,
        FiltroRelatorioInscritosPorFormacaoDto Filtros
    );
}
