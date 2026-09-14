using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public interface IGeradorRelatorioCodafCursoNaoHomologadoExcelService
    {
        byte[] GerarRelatorio(DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto dadosBrutos);
    }
}