﻿using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum NotificacaoCategoria
    {
        [Display(Name = "Alerta")]
        Alerta = 1,

        [Display(Name = "Ação")]
        Workflow_Aprovacao = 2,

        [Display(Name = "Aviso")]
        Aviso = 3,

        [Display(Name = "Informe")]
        Informe = 4
    }
}