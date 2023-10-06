﻿using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Proposta
{
    public class ComunicadoAcaoFormativaDTO
    {
        public string Descricao { get; set; }
        public string Url { get; set; }
    }
}
