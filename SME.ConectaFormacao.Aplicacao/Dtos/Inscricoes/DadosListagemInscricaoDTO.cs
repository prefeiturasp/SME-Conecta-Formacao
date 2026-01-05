using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class DadosListagemInscricaoDto
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
        public DadosListagemInscricaoPermissaoDto? Permissao { get; set; }
        public string DataInscricao { get; set; } = string.Empty;        
        public List<DadosAnexosInscricao> Anexos { get; set; } = [];
    }

    public class DadosListagemInscricaoPermissaoDto
    {
        public bool PodeCancelar { get; set; }
        public bool PodeColocarEmEspera { get; set; }
        public bool PodeConfirmar { get; set; }
        public bool PodeReativar { get; set; }
    }
}