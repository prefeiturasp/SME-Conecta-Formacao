using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto
{
    public class MensagemRabbit
    {
        public MensagemRabbit(string action, object mensagem, Guid? codigoCorrelacao, string usuarioLogadoRF, bool notificarErroUsuario = false, string? perfilUsuario = null, string? administrador = null)
        {
            Action = action;
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao ?? Guid.NewGuid();
            NotificarErroUsuario = notificarErroUsuario;
            UsuarioLogadoRF = usuarioLogadoRF;
            PerfilUsuario = perfilUsuario;
            Administrador = administrador;
        }

        public MensagemRabbit(object mensagem, Guid? codigoCorrelacao, string? usuarioLogadoNomeCompleto, string? usuarioLogadoRF, Guid? perfil, bool notificarErroUsuario = false, string? administrador = null, string? acao = null)
        {
            Mensagem = mensagem;
            CodigoCorrelacao = codigoCorrelacao ?? Guid.NewGuid(); ;
            UsuarioLogadoNomeCompleto = usuarioLogadoNomeCompleto;
            UsuarioLogadoRF = usuarioLogadoRF;
            NotificarErroUsuario = notificarErroUsuario;
            PerfilUsuario = perfil?.ToString();
            Administrador = administrador;
            Action = acao;
        }

        public MensagemRabbit(object mensagem)
        {
            Mensagem = mensagem;
        }

        public MensagemRabbit()
        {

        }
        public string Action { get; set; }
        public object Mensagem { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public string? UsuarioLogadoNomeCompleto { get; set; }
        public string? UsuarioLogadoRF { get; set; }
        public bool NotificarErroUsuario { get; set; }
        public string PerfilUsuario { get; set; }
        public string Administrador { get; set; }

        public T ObterObjetoMensagem<T>() where T : class
        {
            var obj = Mensagem.ToString();
            return obj == null ? throw new ArgumentNullException(nameof(Mensagem)) : obj.JsonParaObjeto<T>();
        }
    }
}
