using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular
{
    public class FiltroListaComponenteCurricularDTO
    {
        [Required(ErrorMessage = "O ano deve ser informada.")]
        public long[] AnoTurmaId { get; set; }

        public bool ExibirOpcaoTodos { get; set; }
    }
}
