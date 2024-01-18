
namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class InscricaoCursistaDTO
{
    public FormacaoResumidaDTO FormacaoResumida { get; set; }
    public IEnumerable<FuncionarioRfNomeDreCodigoCargoFuncaoDTO> CursistasEOL { get; set; }  
    public int QtdeCursistasSuportadosPorTurma { get; set; }
}