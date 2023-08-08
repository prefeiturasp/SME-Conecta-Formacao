using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class VincularPerfilExternoCoreSSOServicoAcessosCommand : IRequest<bool>
    {
        public VincularPerfilExternoCoreSSOServicoAcessosCommand(string login, Guid perfilId)
        {
            Login = login;
            PerfilId = perfilId;
        }

        public string Login { get; }
        public Guid PerfilId { get; }
    }

    public class VincularPerfilExternoCoreSSOServicoAcessosCommandValidator : AbstractValidator<VincularPerfilExternoCoreSSOServicoAcessosCommand>
    {
        public VincularPerfilExternoCoreSSOServicoAcessosCommandValidator()
        {
            RuleFor(x => x.Login)
                .Empty()
                .WithMessage("É nescessário informar o login para vincular o perfil no coreSSO");

            RuleFor(x => x.PerfilId)
                .Empty()
                .WithMessage("É nescessário informar o id do perfil para vincular no coreSSO");
        }
    }
}
