using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Interface;

namespace SME.ConectaFormacao.Infra.Dados.Extensoes
{
    public static class CargaHorariaExtensao
    {
        public static string DefinirCargaHoraria(this ICargaHoraria dto)
        {
            if (dto.HorasTotais.HasValue && dto.HorasTotais.Value < 99)
                return dto.HorasTotais.Value.ToString("00");

            return dto.CargaHorariaTotalOutra.ConverterHoraMinutoParaInteiro().ToString("00");
        }
    }
}