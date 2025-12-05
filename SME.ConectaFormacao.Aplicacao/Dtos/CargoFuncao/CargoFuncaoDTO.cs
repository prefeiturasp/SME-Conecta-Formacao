using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.CargoFuncao
{
    public class CargoFuncaoDTO
    {
        public long Id { get; set; }
        public string Nome { get; set; } = null!;
        public CargoFuncaoTipo Tipo { get; set; }
        public bool Outros { get; set; }
    }
}
