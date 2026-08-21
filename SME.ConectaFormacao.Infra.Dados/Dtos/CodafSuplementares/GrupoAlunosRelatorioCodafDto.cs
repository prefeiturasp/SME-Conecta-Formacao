#pragma warning disable CS8618
using System.Collections.Generic;

namespace SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares
{
    public class GrupoAlunosRelatorioCodafDto
    {
        public string TituloBloco { get; set; }
        public bool EhRedeParceira { get; set; }
        public List<AlunoRelatorioCodafDto> Alunos { get; set; }
    }
}

