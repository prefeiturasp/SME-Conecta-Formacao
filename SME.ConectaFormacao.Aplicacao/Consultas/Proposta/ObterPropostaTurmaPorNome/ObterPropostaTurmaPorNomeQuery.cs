using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaTurmaPorNomeQuery : IRequest<PropostaTurma>
    {
        public ObterPropostaTurmaPorNomeQuery(string propostaTurmaNome, long propostaId)
        {
            PropostaTurmaNome = propostaTurmaNome;
            PropostaId = propostaId;
        }

        public string PropostaTurmaNome { get; }
        public long PropostaId { get; }
    }

    public class ObterPropostaTurmaPorNomeQueryValidator : AbstractValidator<ObterPropostaTurmaPorNomeQuery>
    {
        public ObterPropostaTurmaPorNomeQueryValidator()
        {
            RuleFor(r => r.PropostaTurmaNome)
                .NotEmpty()
                .WithMessage("É necessário informar o nome para obter a proposta turma");

            RuleFor(r => r.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para obter a proposta turma");
        }
    }
}
