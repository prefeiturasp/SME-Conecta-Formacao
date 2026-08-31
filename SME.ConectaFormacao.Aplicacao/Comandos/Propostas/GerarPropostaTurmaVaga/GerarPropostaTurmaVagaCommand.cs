using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarPropostaTurmaVagaCommand : IRequest<bool>
    {
        public GerarPropostaTurmaVagaCommand(long propostaId, short quantidadeVagasTurma)
        {
            PropostaId = propostaId;
            QuantidadeVagasTurma = quantidadeVagasTurma;
        }

        public long PropostaId { get; }
        public short QuantidadeVagasTurma { get; }
    }

    [ExcludeFromCodeCoverage]
    public class GerarPropostaTurmaVagaCommandValidator : AbstractValidator<GerarPropostaTurmaVagaCommand>
    {
        public GerarPropostaTurmaVagaCommandValidator()
        {
            RuleFor(t => t.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para gerar a tabela de proposta turma vagas");

            RuleFor(t => t.QuantidadeVagasTurma)
                .NotEmpty()
                .WithMessage("É necessário informar a quantidade de vagas por turma para gerar a tabela de proposta turma vagas");
        }
    }
}
