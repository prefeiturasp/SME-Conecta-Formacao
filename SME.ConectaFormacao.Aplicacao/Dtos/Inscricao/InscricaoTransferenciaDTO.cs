using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoTransferenciaDTO
    {
        public long IdFormacaoOrigem { get; set; }
        public long IdTurmaOrigem { get; set; }
        public IEnumerable<CursistaServicoEol> Cursistas { get; set; }
        public long IdFormacaoDestino { get; set; }
        public long IdTurmaDestino { get; set; }
        public string CargoCodigo { get; set; }
        public string? FuncaoDreCodigo { get; set; }
        public string? FuncaoCodigo { get; set; }
    }
}
