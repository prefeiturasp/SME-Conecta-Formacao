using FluentValidation;
using MediatR;

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

    public class UsuarioExisteNoCoreSsoQueryValidator : AbstractValidator<UsuarioExisteNoCoreSsoQuery>
    {
        public UsuarioExisteNoCoreSsoQueryValidator()
        {
            RuleFor(x => x.Cpf).NotNull().WithMessage("Informe o CPF para verificar se o usuário existe no coresso");
        }
    }
}
