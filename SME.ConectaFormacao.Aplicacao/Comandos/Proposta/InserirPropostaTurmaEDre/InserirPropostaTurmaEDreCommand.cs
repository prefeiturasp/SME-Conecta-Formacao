using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaTurmaAdicionalCommand : IRequest<long>
    {
        public InserirPropostaTurmaAdicionalCommand(long propostaTurmaOrigemId)
        {
            PropostaTurmaOrigemId = propostaTurmaOrigemId;
        }

        public long PropostaTurmaOrigemId { get; }
    }

    public class InserirPropostaTurmaAdicionalCommandValidator : AbstractValidator<InserirPropostaTurmaAdicionalCommand>
    {
        public InserirPropostaTurmaAdicionalCommandValidator()
        {
            RuleFor(x => x.PropostaTurmaOrigemId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta turma para adicionar uma turma adicional");
        }
    }
}
