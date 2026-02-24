using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class InscricaoProximaFiltroDTO
    {
        public long? CodigoFormacao { get; set; }
        public string? NomeFormacao { get; set; }
        public string? NomeTurma { get; set; }
        public string? Situacao { get; set; }
        public DateTime? DataInscricao { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}
