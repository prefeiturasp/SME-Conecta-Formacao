using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.Relatorios
{
    public static class FiltroRelatorioInscritosValidador
    {
        public static Resultado Validar(FiltroRelatorioInscritosPorFormacaoDto filtro)
        {
            if (filtro == null)
                return new Erro(TipoFalha.Validacao, "Filtros para geração do relatório não informados");

            if (filtro.PropostaId is null && filtro.NumeroHomologacao is null && filtro.PropostaTurmaId is null)
            {
                if (filtro.PeriodoDeRealizacaoInicial.Year < 2000 || filtro.PeriodoDeRealizacaoFinal.Year < 2000)
                    return new Erro(TipoFalha.Validacao, "O período de realização deve ser informado");

                if (filtro.PeriodoDeRealizacaoFinal < filtro.PeriodoDeRealizacaoInicial)
                    return new Erro(TipoFalha.Validacao, "O período de realização final não pode ser menor que o período de realização inicial");

                if (filtro.PeriodoDeRealizacaoFinal > filtro.PeriodoDeRealizacaoInicial.AddYears(1))
                    return new Erro(TipoFalha.Validacao, "O período de realização não pode ser superior a 1 ano");
            }

            return Resultado.DeSucesso();
        }
    }
}
