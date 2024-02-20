using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaCommand : IRequest<bool>
    {
        public SalvarPropostaTurmaCommand(long propostaId, IEnumerable<PropostaTurma> turmas)
        {
            PropostaId = propostaId;
            Turmas = turmas;
        }

        public long PropostaId { get; }
        public IEnumerable<PropostaTurma> Turmas { get; }
    }

    public class SalvarPropostaTurmaCommandValidator : AbstractValidator<SalvarPropostaTurmaCommand>
    {
        public SalvarPropostaTurmaCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar as turmas");
        }
    }
}
