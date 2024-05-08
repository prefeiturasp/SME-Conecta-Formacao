using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas
{
    public class ObterSugestoesPareceristasQuery : IRequest<string>
    {
        public ObterSugestoesPareceristasQuery(long propostaId, SituacaoParecerista situacao)
        {
            PropostaId = propostaId;
            Situacao = situacao;
        }

        public long PropostaId { get; }
        public SituacaoParecerista Situacao { get; }
    }

    public class ObterSugestoesPareceristasQueryValidator : AbstractValidator<ObterSugestoesPareceristasQuery>
    {
        public ObterSugestoesPareceristasQueryValidator()
        {
            RuleFor(f => f.PropostaId)
                .NotEmpty()
                .WithMessage("Informe o Id da proposta para obter as sugestões de parecer");
        }
    }
}
