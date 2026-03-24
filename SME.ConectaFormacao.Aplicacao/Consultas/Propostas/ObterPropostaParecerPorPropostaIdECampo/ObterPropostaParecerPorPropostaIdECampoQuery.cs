using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaParecerPorPropostaIdECampoQuery : IRequest<PropostaPareceristaConsideracaoCompletoDTO>
    {
        public ObterPropostaParecerPorPropostaIdECampoQuery(long propostaId, CampoConsideracao campoConsideracao)
        {
            PropostaId = propostaId;
            CampoConsideracao = campoConsideracao;
        }

        public long PropostaId { get; set; }
        public CampoConsideracao CampoConsideracao { get; set; }
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