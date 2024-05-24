using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao
{
    public class NotificacaoFiltroDTO
    {
        public long? Id { get; set; }
        public string? Titulo { get; set; }
        public NotificacaoCategoria? Categoria { get; set; }
        public NotificacaoTipo? Tipo { get; set; }
        public NotificacaoUsuarioSituacao? Situacao { get; set; }
    }
}
