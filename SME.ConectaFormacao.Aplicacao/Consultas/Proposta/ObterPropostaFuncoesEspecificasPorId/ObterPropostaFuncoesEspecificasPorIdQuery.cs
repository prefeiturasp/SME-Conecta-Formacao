using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaFuncoesEspecificasPorIdQuery : IRequest<IEnumerable<PropostaFuncaoEspecifica>>
    {
        public ObterPropostaFuncoesEspecificasPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; }
    }

    public class ObterPropostaFuncoesEspecificasPorIdQueryValidator: AbstractValidator<ObterPropostaFuncoesEspecificasPorIdQuery>
    {
        public ObterPropostaFuncoesEspecificasPorIdQueryValidator()
        {
            RuleFor(r => r.PropostaId).NotEmpty().WithMessage("É necessário informar o id da proposta para obter as funções especificas");
        }
    }
}
