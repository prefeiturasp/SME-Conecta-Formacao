using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarUsuarioTokenServicoAcessosQuery : IRequest<bool>
    {
        public ValidarUsuarioTokenServicoAcessosQuery(Guid token)
        {
            Token = token;
        }

        public Guid Token { get; }
    }

    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryValidator : AbstractValidator<ValidarUsuarioTokenServicoAcessosQuery>
    {
        public ValidarTokenRecuperacaoSenhaServicoAcessosQueryValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("É necessário informar o token para validar a recuperação de senha");
        }
    }
}
