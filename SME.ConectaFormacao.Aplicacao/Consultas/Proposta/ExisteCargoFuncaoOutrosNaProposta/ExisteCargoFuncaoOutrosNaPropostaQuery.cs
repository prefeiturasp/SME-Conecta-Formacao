using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExisteCargoFuncaoOutrosNaPropostaQuery : IRequest<bool>
    {
        public ExisteCargoFuncaoOutrosNaPropostaQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    public class ObterFuncoesEspecificasPorPorpostaIdQueryValidator : AbstractValidator<ExisteCargoFuncaoOutrosNaPropostaQuery>
    {
        public ObterFuncoesEspecificasPorPorpostaIdQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para consultar a função especifica");
        }
    }
}