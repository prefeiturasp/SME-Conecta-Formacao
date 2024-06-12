using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria
{
    public class FiltroUsuarioRedeParceriaDTO
    {
        public long[]? AreaPromotoraIds { get; set; }
        public string? Nome { get; set; }
        public string? Cpf { get; set; }
        public SituacaoUsuario? Situacao { get; set; }
    }
}
