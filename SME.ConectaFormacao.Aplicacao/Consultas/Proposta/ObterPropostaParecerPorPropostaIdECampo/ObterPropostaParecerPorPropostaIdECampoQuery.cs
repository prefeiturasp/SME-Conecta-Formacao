using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaParecerPorPropostaIdECampoQuery : IRequest<PropostaPareceristaConsideracaoCompletoDTO>
    {
        public ObterPropostaParecerPorPropostaIdECampoQuery(long propostaId, CampoParecer campoParecer)
        {
            PropostaId = propostaId;
            CampoParecer = campoParecer;
        }

        public long PropostaId { get; set; }
        public CampoParecer CampoParecer { get; set; }
    }
    public class ObterPropostaParecerPorPropostaIdECampoQueryValidator : AbstractValidator<ObterPropostaParecerPorPropostaIdECampoQuery>
    {
        public ObterPropostaParecerPorPropostaIdECampoQueryValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da proposta");
        }
    }
}