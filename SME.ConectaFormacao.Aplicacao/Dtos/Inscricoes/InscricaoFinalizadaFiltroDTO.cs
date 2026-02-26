using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class InscricaoFinalizadaFiltroDTO
    {
        public string? NomeFormacao { get; set; }
        public int? SituacaoInscricao { get; set; }
        public int? SituacaoAprovacao { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}
