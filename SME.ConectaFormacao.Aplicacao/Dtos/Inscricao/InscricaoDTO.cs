﻿using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class InscricaoDTO
    {
        public long PropostaTurmaId { get; set; }
        [Required(ErrorMessage = "E-mail é obrigatório")]
        public string Email { get; set; }
        public long? ArquivoId { get; set; }

        public long? CargoCodigo { get; set; }
        public string? CargoDreCodigo { get; set; }
        public string? CargoUeCodigo { get; set; }

        public long? FuncaoCodigo { get; set; }
        public string? FuncaoDreCodigo { get; set; }
        public string? FuncaoUeCodigo { get; set; }
    }
}
