using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PublicarNaFilaRabbitCommand : IRequest<bool>
    {
        public PublicarNaFilaRabbitCommand(string rota, object filtros, Guid? codigoCorrelacao = null, Usuario usuarioLogado = null, bool notificarErroUsuario = false, string exchange = null)
        {
            Rota = rota;
            Filtros = filtros;
            CodigoCorrelacao = codigoCorrelacao ?? Guid.NewGuid();
            Usuario = usuarioLogado;
            NotificarErroUsuario = notificarErroUsuario;
            Exchange = exchange;
        }

        public string Rota { get; set; }
        public object Filtros { get; set; }
        public Guid CodigoCorrelacao { get; set; }
        public Usuario? Usuario { get; set; }
        public bool NotificarErroUsuario { get; set; }
        public string Exchange { get; set; }
    }

    public class PublicarNaFilaRabbitCommandValidator : AbstractValidator<PublicarNaFilaRabbitCommand>
    {
        public PublicarNaFilaRabbitCommandValidator()
        {
            RuleFor(a => a.Filtros)
                .NotEmpty()
                .WithMessage("O payload da mensagem deve ser informado para a execução da fila");
            
            RuleFor(a => a.Rota)
                .NotEmpty()
                .WithMessage("A rota deve ser informado para a execução da fila");
        }
    }
}