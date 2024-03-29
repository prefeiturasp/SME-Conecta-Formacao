﻿using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Servicos.Eol;

public class ComponenteCurricularAnoTurmaServicoEol
{
    public long CodigoComponenteCurricular { get; set; }
    public string DescricaoComponenteCurricular { get; set; }
    public string CodigoAnoTurma { get; set; }
    public string DescricaoSerieEnsino { get; set; }
    public long CodigoSerieEnsino { get; set; }
    public Modalidade Modalidade { get; set; }
}