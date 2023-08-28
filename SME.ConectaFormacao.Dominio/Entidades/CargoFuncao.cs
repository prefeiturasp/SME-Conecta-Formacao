using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class CargoFuncao : EntidadeBaseAuditavel
    {
        public string Nome { get; set; }
        public CargoFuncaoTipo Tipo { get; set; }
    }
}
