
namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class InscricaoCursistaDTO
{
    public FormacaoResumidaDTO FormacaoResumida { get; set; }
    public IEnumerable<FuncionarioRfDreCodigoDTO> CursistasEOL { get; set; }  
    public int QtdeCursistasSuportadosPorTurma { get; set; } //via parâmetro do sistema - deverá criar parâmetro
}