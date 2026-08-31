using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class DesvincularPerfilExternoCoreSSOServicoAcessosCommand : IRequest<bool>
    {
        public DesvincularPerfilExternoCoreSSOServicoAcessosCommand(string login, Guid perfilId)
        {
            Login = login;
            PerfilId = perfilId;
        }

        public string Login { get; }
        public Guid PerfilId { get; }
    }

    [ExcludeFromCodeCoverage]
    public class DesvincularPerfilExternoCoreSSOServicoAcessosCommandValidator : AbstractValidator<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>
    {
        public DesvincularPerfilExternoCoreSSOServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
             .NotEmpty()
             .WithMessage("É necessário informar o login para desvincular o perfil no coreSSO");

            RuleFor(x => x.PerfilId)
                .NotEmpty()
                .WithMessage("É necessário informar o id do perfil para desvincular no coreSSO");
        }
    }
}
