using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class FuncaoAtividadeUsuario
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string CdRegistroFuncional { get; set; }
        public int CdTipoFuncao { get; set; }
        public string CdUe { get; set; }
        public DateTime DataAtualizacao { get; private set; } = DateTime.UtcNow;


        public string ObterChaveNegocio()
        {
            return $"{CdRegistroFuncional}-{CdTipoFuncao}-{CdUe}";
        }
    }
}
