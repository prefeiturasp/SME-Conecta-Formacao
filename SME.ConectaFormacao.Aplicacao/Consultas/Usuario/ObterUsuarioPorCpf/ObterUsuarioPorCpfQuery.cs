using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuarioPorCpf
{
    public class ObterUsuarioPorCpfQuery(string cpf) : IRequest<Dominio.Entidades.Usuario?>
    {
        public string Cpf { get; } = cpf;

    }

    [ExcludeFromCodeCoverage]
    public class ObterUsuarioPorCpfQueryValidator : AbstractValidator<ObterUsuarioPorCpfQuery>
    {
        public ObterUsuarioPorCpfQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().NotNull().WithMessage("É necessário informar o cpf para obter o usuário");
        }
    }

}