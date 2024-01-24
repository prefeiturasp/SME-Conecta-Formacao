namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

public class InserirInscricaoDTO
{
    public long PropostaId { get; set; }
    public List<InscricaoAutomaticaPropostaTurmaCursistasDTO> InscricaoAutomaticaPropostaTurmaCursistasDTO { get; set; }
}
