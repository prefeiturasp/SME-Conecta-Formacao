using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoAutomaticaPropostaDTO
    {
        public long PropostaId { get; set; }
        public IEnumerable<PropostaInscricaoAutomaticaTurma> PropostasTurmas { get; set; }
        public IEnumerable<long> PublicosAlvos { get; set; }
        public IEnumerable<long> FuncoesEspecificas { get; set; }
        public IEnumerable<string> AnosTurmas { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public TipoInscricao TipoInscricao { get; set; }
        public IEnumerable<long> Modalidades { get; set; }
        
        public bool EhTipoJornadaJEIF
        {
            get { return TipoInscricao == TipoInscricao.AutomaticaJEIF; }
        }
        public bool IntegrarNoSGA { get; set; }
    }
}
