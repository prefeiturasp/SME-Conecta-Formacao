using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarEmailsAreaPromotoraCommand : IRequest
    {
        public ValidarEmailsAreaPromotoraCommand(IEnumerable<AreaPromotoraEmailDTO> emails, AreaPromotoraTipo areaPromotoraTipo)
        {
            Emails = emails;
            Tipo = areaPromotoraTipo;
        }

        public IEnumerable<AreaPromotoraEmailDTO> Emails { get; set; }
        public AreaPromotoraTipo Tipo { get; set; }
    }

    public class ValidarEmailsAreaPromotoraCommandValidator : AbstractValidator<ValidarEmailsAreaPromotoraCommand>
    {
        public ValidarEmailsAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.Emails)
                .NotEmpty()
                .WithMessage("É necessário informar o email da área promotora para validar");

            RuleFor(x => x.Tipo)
                .NotEmpty()
                .WithMessage("É necessário informar o tipo da área promotora para validar o email");
        }
    }
}
