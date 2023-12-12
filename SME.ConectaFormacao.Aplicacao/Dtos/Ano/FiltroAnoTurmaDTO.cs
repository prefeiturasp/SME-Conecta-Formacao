using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Ano
{
    public class FiltroAnoTurmaDTO
    {
        public int AnoLetivo { get; set; }
        public Modalidade[] Modalidade { get; set; }
        public bool ExibirOpcaoTodos { get; set; }
    }
}
