using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeExisteRegenteTurmaCommand : IRequest<string>
    {
        public ValidarSeExisteRegenteTurmaCommand(long propostaId, short quantidadeTurmas)
        {
            PropostaId = propostaId;
            QuantidadeTurmas = quantidadeTurmas;
        }

        public long PropostaId { get; set; }
        public short QuantidadeTurmas { get; set; }

    }
    public class ValidarSeExisteRegenteTurmaCommandValidator : AbstractValidator<ValidarSeExisteRegenteTurmaCommand>
    {
        public ValidarSeExisteRegenteTurmaCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta para realizar as validações");
        }
    }
}