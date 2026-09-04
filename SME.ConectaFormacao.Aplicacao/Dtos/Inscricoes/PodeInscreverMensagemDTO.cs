namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class PodeInscreverMensagemDTO
    {
        public string Mensagem { get; set; }
        public bool PodeInscrever { get; set; }
        public string NomeFormacao { get; set; }
        public List<int> TiposInscricao { get; set; } = new();
    }
}