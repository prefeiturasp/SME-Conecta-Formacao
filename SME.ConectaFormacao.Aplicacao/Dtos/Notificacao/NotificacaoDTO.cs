using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Notificacao
{
    public class NotificacaoDTO
    {
        public long Id { get; set; }
        public string Titulo { get; set; }
        public string Mensagem { get; set; }
        public NotificacaoCategoria Categoria { get; set; }
        public string CategoriaDescricao { get; set; }
        public NotificacaoTipo Tipo { get; set; }
        public string TipoDescricao { get; set; }
        public string Parametros { get; set; }
    }
}
