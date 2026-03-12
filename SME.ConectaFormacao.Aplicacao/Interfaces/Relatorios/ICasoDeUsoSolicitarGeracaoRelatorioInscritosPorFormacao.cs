using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Relatorios
{
    public interface ICasoDeUsoSolicitarGeracaoRelatorioInscritosPorFormacao
    {
        Task<Resultado> ExecutarAsync(FiltroRelatorioInscritosPorFormacaoDto filtro);
    }
}
