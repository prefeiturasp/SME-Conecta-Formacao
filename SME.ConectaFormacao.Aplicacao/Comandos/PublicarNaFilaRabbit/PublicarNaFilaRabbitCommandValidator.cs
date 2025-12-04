using FluentValidation;

namespace SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit
{
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