using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.ObjetosDeValor
{
    public class PropostaInscricaoAutomatica
    {
        public long PropostaId { get; set; }
        public bool IntegrarNoSGA { get; set; }
        public SituacaoProposta Situacao { get; set; }
        public short QuantidadeVagasTurmas { get; set; }

        public IEnumerable<PropostaInscricaoAutomaticaTurma> PropostasTurmas { get; set; }
        public IEnumerable<TipoInscricao> TiposInscricao { get; set; }
        public IEnumerable<long> PublicosAlvos { get; set; }
        public IEnumerable<long> FuncoesEspecificas { get; set; }
        public IEnumerable<string> AnosTurmas { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public IEnumerable<long> Modalidades { get; set; }

        public bool EhInscricaoAutomatica
        {
            get { return TiposInscricao.Contains(TipoInscricao.Automatica) || TiposInscricao.Contains(TipoInscricao.AutomaticaJEIF); }
        }

        public bool EhTipoJornadaJEIF
        {
            get { return TiposInscricao.Contains(TipoInscricao.AutomaticaJEIF); }
        }
    }
}
