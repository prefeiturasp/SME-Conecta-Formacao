using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorCpfQuery : IRequest<Dominio.Entidades.Usuario>
    {
        public ObterUsuarioPorCpfQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; }

    }

    public class ObterUsuarioPorCpfQueryValidator : AbstractValidator<ObterUsuarioPorCpfQuery>
    {
        public ObterUsuarioPorCpfQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().NotNull().WithMessage("É necessário informar o cpf para obter o usuário");
        }
    }

}