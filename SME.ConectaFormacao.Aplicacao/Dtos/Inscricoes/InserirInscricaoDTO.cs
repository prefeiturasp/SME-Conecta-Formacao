namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

public class InserirInscricaoDTO
{
    public long PropostaId { get; set; }
    public List<InscricaoAutomaticaPropostaTurmaCursistasDTO> InscricaoAutomaticaPropostaTurmaCursistasDTO { get; set; }
}
