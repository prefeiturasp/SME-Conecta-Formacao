using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaTotalParecer
    {
        public long IdProposta { get; set; }
        public CampoParecer Campo { get; set; }
        public int Quantidade { get; set; }
    }
}
