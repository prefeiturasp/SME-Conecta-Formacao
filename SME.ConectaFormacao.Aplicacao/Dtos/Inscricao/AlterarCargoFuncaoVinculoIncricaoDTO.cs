﻿using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricao
{
    public class AlterarCargoFuncaoVinculoIncricaoDTO
    {
        [Required(ErrorMessage = "O Cargo é obrigatório")]
        public string CargoCodigo { get; set; } = string.Empty;
        public int TipoVinculo { get; set; }
    }
}