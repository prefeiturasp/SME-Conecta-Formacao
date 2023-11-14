using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarDatasExistentesNaPropostaCommand :IRequest
    {
        public ValidarDatasExistentesNaPropostaCommand(long propostaId, PropostaDTO propostaDto)
        {
            PropostaId = propostaId;
            PropostaDto = propostaDto;
        }

        public long PropostaId { get; set; }
        public PropostaDTO PropostaDto { get; set; }
    }

    public class ValidarDatasExistentesNaPropostaCommandValidator : AbstractValidator<ValidarDatasExistentesNaPropostaCommand>
    {
        public ValidarDatasExistentesNaPropostaCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informa o Id da Proposta para realizar as validações");
        }
    }
}