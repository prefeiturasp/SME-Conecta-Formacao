using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.ObjetosDeValor
{
    public class FormacaoResumida
    {
        public long PropostaId { get; set; }
        public FormacaoHomologada FormacaoHomologada { get; set; }
        public IEnumerable<PropostaTurmaResumida> PropostasTurmas { get; set; }
        public IEnumerable<long> PublicosAlvos { get; set; }
        public IEnumerable<long> FuncoesEspecificas { get; set; }
        public IEnumerable<string> AnosTurmas { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public TipoInscricao TipoInscricao { get; set; }
        public IEnumerable<long> Modalidades { get; set; }
        
        public bool EhFormacaoHomologada
        {
            get { return FormacaoHomologada == Enumerados.FormacaoHomologada.Sim; }
        }
        public bool EhTipoJornadaJEIF
        {
            get { return TipoInscricao == Enumerados.TipoInscricao.AutomaticaJEIF; }
        }
    }

    public class PropostaTurmaResumida
    {
        public long Id { get; set; }
        public string CodigoDre { get; set; }
    }
}
