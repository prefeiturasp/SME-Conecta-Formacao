using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class InscricaoFinalizadaFiltroDTO
    {
        public string? NomeFormacao { get; set; }
        public string? SituacaoInscricao { get; set; }
        public string? SituacaoAprovacao { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}
