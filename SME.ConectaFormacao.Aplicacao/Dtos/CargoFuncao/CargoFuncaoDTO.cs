using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao
{
    public class CargoFuncaoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public CargoFuncaoTipo Tipo { get; set; }
    }
}
