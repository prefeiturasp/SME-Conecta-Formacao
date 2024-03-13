using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTurmaCommand : IRequest<bool>
    {
        public SalvarPropostaTurmaCommand(long propostaId, IEnumerable<PropostaTurma> turmas, SituacaoProposta situacao)
        {
            PropostaId = propostaId;
            Turmas = turmas;
            Situacao = situacao;
        }

        public long PropostaId { get; }
        public IEnumerable<PropostaTurma> Turmas { get; }
        public SituacaoProposta Situacao { get; }
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
