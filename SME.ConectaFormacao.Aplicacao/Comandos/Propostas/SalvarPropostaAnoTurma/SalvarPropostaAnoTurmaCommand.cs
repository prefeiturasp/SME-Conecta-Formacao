using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaAnoTurmaCommand : IRequest<bool>
    {
        public SalvarPropostaAnoTurmaCommand(long propostaId, IEnumerable<PropostaAnoTurma> anosTurmas)
        {
            PropostaId = propostaId;
            AnosTurmas = anosTurmas;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaAnoTurma> AnosTurmas { get; set; }
    }

    public class SalvarPropostaAnoTurmaCommandValidator : AbstractValidator<SalvarPropostaAnoTurmaCommand>
    {
        public SalvarPropostaAnoTurmaCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar os anos turmas da proposta");
        }
    }
}
