using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos
{
    public class AlterarNomeSocialServicoAcessosCommand(string login, string? NomeSocial) : IRequest<bool>
    {
        public string Login { get; } = login;
        public string? NomeSocial { get; } = NomeSocial;
    }

    public class AlterarNomeSocialServicoAcessosCommandValidator : AbstractValidator<AlterarNomeSocialServicoAcessosCommand>
    {
        public AlterarNomeSocialServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("É necessário informar o login para alterar o nome social do usuário");
        }
    }
}