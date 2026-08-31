using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class UsuarioExisteNoCoreSsoQuery : IRequest<bool>
    {
        public UsuarioExisteNoCoreSsoQuery(string cpf)
        {
            Cpf = cpf;
        }
        public string Cpf { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class UsuarioExisteNoCoreSsoQueryValidator : AbstractValidator<UsuarioExisteNoCoreSsoQuery>
    {
        public UsuarioExisteNoCoreSsoQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("Informe o CPF para verificar se o usuário existe no coresso");
        }
    }
}
