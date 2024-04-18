using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class VerificarSeUsuarioPossuiCargoAtivoNoEolQuery : IRequest<IEnumerable<string>>
    {
        public VerificarSeUsuarioPossuiCargoAtivoNoEolQuery(string[] login)
        {
            Login = login;
        }

        public string[] Login { get; set; }
    }

    public class
        VerificarSeUsuarioPossuiCargoAtivoNoEolQueryValidator : AbstractValidator<
        VerificarSeUsuarioPossuiCargoAtivoNoEolQuery>
    {
        public VerificarSeUsuarioPossuiCargoAtivoNoEolQueryValidator()
        {
            RuleFor(x => x.Login).NotEmpty().WithMessage("Informe o Login do Usuário pare realizar consulta no EOl");
        }
    }
}