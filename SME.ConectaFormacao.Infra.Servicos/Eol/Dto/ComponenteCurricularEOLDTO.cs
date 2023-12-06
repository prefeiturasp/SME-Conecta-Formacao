using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

public class ComponenteCurricularEOLDTO
{
    public long Codigo { get; set; }
    public string Descricao { get; set; }
    public string AnoTurma { get; set; }
    public string SerieEnsino { get; set; }
    public long CodigoSerieEnsino { get; set; }
    public Modalidade Modalidade { get; set; }
}