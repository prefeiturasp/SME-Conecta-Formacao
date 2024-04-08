using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInformacoesInscricoesEstaoAbertasPorIdQuery : IRequest<PodeInscreverMensagemDTO>
    {
        public ObterInformacoesInscricoesEstaoAbertasPorIdQuery(long propostaId)
        {
            PropostaId = propostaId;
        }

        public long PropostaId { get; set; }
    }

    class ObterInscricoesEstaoAbertasPorIdQueryValidator : AbstractValidator<ObterInformacoesInscricoesEstaoAbertasPorIdQuery>
    {
        public ObterInscricoesEstaoAbertasPorIdQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para verificar se as inscrições estão abertas");
        }
    }
}