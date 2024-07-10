using SME.ConectaFormacao.Dominio.Enumerados;
using System.ComponentModel.DataAnnotations;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Usuario
{
    public class TipoEmailUsuarioDTO
    {
        [Required(ErrorMessage = "É necessário informar o tipo de e-mail"), Range((int)TipoEmail.FuncionarioUnidadeParceira, (int)TipoEmail.Estagiario, ErrorMessage = "Tipo de e-mail inválido")]
        public int Tipo { get; set; }
    }
}
