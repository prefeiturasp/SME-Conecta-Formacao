using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterSugestoesPareceristas
{
    public class ObterSugestoesPareceristasQuery : IRequest<IEnumerable<PropostaPareceristaSugestaoDTO>>
    {
        public ObterSugestoesPareceristasQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; }
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
