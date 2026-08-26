using FluentValidation;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class AlterarUnidadeEolUsuarioCommand : IRequest<bool>
    {
        public AlterarUnidadeEolUsuarioCommand(string login, string codigoEolUnidade)
        {
            Login = login;
            CodigoEolUnidade = codigoEolUnidade;
        }

        public string Login { get; }
        public string CodigoEolUnidade { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AlterarUnidadeEolUsuarioCommandValidator : AbstractValidator<AlterarUnidadeEolUsuarioCommand>
    {
        public AlterarUnidadeEolUsuarioCommandValidator()
        {
            RuleFor(r => r.Login).NotEmpty().WithMessage("Informe o login do usuário para realizar a alteração da unidade eol");
            RuleFor(r => r.CodigoEolUnidade).NotEmpty().WithMessage("Informe o código da unidade do usuário para realizar a alteração da unidade eol");
        }
    }
}

