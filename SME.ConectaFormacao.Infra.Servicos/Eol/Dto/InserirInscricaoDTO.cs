namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class InserirInscricaoDTO
{
    public long PropostaId { get; set; }
    public List<PropostaTurmaCursistasDTO> PropostasTurmasCursistas { get; set; }
}