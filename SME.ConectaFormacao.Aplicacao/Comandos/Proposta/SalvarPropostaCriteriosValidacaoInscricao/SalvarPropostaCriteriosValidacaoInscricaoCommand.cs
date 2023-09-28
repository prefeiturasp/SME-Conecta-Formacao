using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaCriteriosValidacaoInscricaoCommand : IRequest<bool>
    {
        public SalvarPropostaCriteriosValidacaoInscricaoCommand(long propostaId, IEnumerable<PropostaCriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            PropostaId = propostaId;
            CriteriosValidacaoInscricao = criteriosValidacaoInscricao;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaCriterioValidacaoInscricao> CriteriosValidacaoInscricao { get; set; }
    }

    public class SalvarPropostaCriteriosValidacaoInscricaoCommandValidator : AbstractValidator<SalvarPropostaCriteriosValidacaoInscricaoCommand>
    {
        public SalvarPropostaCriteriosValidacaoInscricaoCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É nescessário informar o id da proposta para salvar os critérios de validação das inscrições");
        }
    }
}
