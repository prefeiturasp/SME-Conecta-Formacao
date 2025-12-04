using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit
{
    public class PublicarNaFilaRabbitCommand(string rota, object filtros, Guid? codigoCorrelacao = null, Usuario? usuarioLogado = null, 
        bool notificarErroUsuario = false, string? exchange = null) : IRequest<bool>
    {
        public string Rota { get; set; } = rota;
        public object Filtros { get; set; } = filtros;
        public Guid CodigoCorrelacao { get; set; } = codigoCorrelacao ?? Guid.NewGuid();
        public Usuario? Usuario { get; set; } = usuarioLogado;
        public bool NotificarErroUsuario { get; set; } = notificarErroUsuario;
        public string? Exchange { get; set; } = exchange;
    }
}