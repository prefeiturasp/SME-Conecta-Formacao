namespace SME.ConectaFormacao.Infra.Dados.Dtos.Propostas
{
    public class RegrasAprovacaoCursistaDto
    {
        public int FrequenciaMinima { get; set; }
        public IEnumerable<string> ConceitosAceitos { get; set; } = [];
        public bool ExigeAtividadeObrigatoria { get; set; }
        public bool PossuiRegraAvaliacao => FrequenciaMinima > 0 || ConceitosAceitos.Any() || ExigeAtividadeObrigatoria;
    }
}
