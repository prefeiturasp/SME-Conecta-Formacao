using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarTokenRecuperacaoSenhaServicoAcessosQuery : IRequest<bool>
    {
        public ValidarTokenRecuperacaoSenhaServicoAcessosQuery(Guid token)
        {
            Token = token;
        }

        public Guid Token { get; }
    }

    public class ValidarTokenRecuperacaoSenhaServicoAcessosQueryValidator : AbstractValidator<ValidarTokenRecuperacaoSenhaServicoAcessosQuery>
    {
        public ValidarTokenRecuperacaoSenhaServicoAcessosQueryValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("É necessário informar o token para validar a recuperação de senha");
        }
    }
}
