using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarEmailsAreaPromotoraCommand : IRequest
    {
        public ValidarEmailsAreaPromotoraCommand(string email, AreaPromotoraTipo areaPromotoraTipo)
        {
            Email = email;
            Tipo = areaPromotoraTipo;
        }

        public string Email { get; }
        public AreaPromotoraTipo Tipo { get; set; }
    }

    public class ValidarEmailsAreaPromotoraCommandValidator : AbstractValidator<ValidarEmailsAreaPromotoraCommand>
    {
        public ValidarEmailsAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("É nescessário informar o email da área promotora para validar");

            RuleFor(x => x.Tipo)
                .NotEmpty()
                .WithMessage("É nescessário informar o tipo da área promotora para validar o email");
        }
    }
}
