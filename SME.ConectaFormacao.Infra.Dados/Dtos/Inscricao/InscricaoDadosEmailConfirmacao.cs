namespace SME.ConectaFormacao.Infra;

public class InscricaoDadosEmailConfirmacao
{
    public long UsuarioId { get; set; }
    public string Email { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string HoraInicio { get; set; }
    public string HoraFim { get; set; }
    public string Local { get; set; }
    public bool IntegradoSga { get; set; }
    public string NomeFormacao { get; set; }
    public string NomeDestinatario { get; set; }
}