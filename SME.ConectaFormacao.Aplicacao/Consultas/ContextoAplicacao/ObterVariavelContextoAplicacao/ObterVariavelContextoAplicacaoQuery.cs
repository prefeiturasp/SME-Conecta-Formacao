using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterVariavelContextoAplicacaoQuery : IRequest<string>
    {
        public ObterVariavelContextoAplicacaoQuery(string nome)
        {
            Nome = nome;
        }

        public string Nome { get; }
    }

    public class ObterVariavelContextoAplicacaoQueryValidator : AbstractValidator<ObterVariavelContextoAplicacaoQuery>
    {
        public ObterVariavelContextoAplicacaoQueryValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty()
                .WithMessage("É nescessário informar o nome da variável para obter do contexto da aplicação");
        }
    }
}
