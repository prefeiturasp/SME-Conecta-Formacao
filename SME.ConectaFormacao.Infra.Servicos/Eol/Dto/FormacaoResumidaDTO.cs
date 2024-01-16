namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class FormacaoResumidaDTO
{
    public long PropostaId { get; set; }
    public int FormacaoHomologada { get; set; }
    public IEnumerable<PropostaTurmaResumidaDTO> PropostasTurmas { get; set; }
    public IEnumerable<long> PublicosAlvos { get; set; }
    public IEnumerable<long> FuncoesEspecificas { get; set; }
    public IEnumerable<string> AnosTurmas { get; set; }
    public IEnumerable<long> ComponentesCurriculares { get; set; }
    public int TipoInscricao { get; set; }
    public IEnumerable<long> Modalidades { get; set; }
}

public class PropostaTurmaResumidaDTO
{
    public long Id { get; set; }
    public string CodigoDre { get; set; }
}	

public class PropostaTurmaCursistasDTO 
{
    public long Id { get; set; }
    public IEnumerable<string> Cursistas { get; set; }
}	