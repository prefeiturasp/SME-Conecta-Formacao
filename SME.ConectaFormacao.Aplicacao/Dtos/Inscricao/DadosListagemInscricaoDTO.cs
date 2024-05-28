using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class DadosListagemInscricaoDTO
    {
        public long InscricaoId { get; set; }
        public string? NomeTurma { get; set; }
        public string? RegistroFuncional { get; set; }
        public string? Cpf { get; set; }
        public string? NomeCursista { get; set; }
        public string? CargoFuncao { get; set; }
        public SituacaoInscricao SituacaoCodigo { get; set; }
        public string? Situacao { get; set; }
        public string? Origem { get; set; }
        public bool IntegrarNoSga { get; set; }
        public bool Iniciado { get; set; }
        public List<DadosAnexosInscricao> Anexos { get; set; } = new List<DadosAnexosInscricao>();
    }
}