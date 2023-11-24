using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaMovimentacaoCommand : IRequest<bool>
    {
        public SalvarPropostaMovimentacaoCommand(long propostaId,PropostaMovimentacaoDTO propostaMovimentacaoDtoDto)
        {
            PropostaId = propostaId;
            PropostaMovimentacaoDto = propostaMovimentacaoDtoDto;
        }

        public long PropostaId { get; set; }
        public PropostaMovimentacaoDTO PropostaMovimentacaoDto { get; set; }

    }

    public class SalvarPropostaMovimentacaoCommandValidator : AbstractValidator<SalvarPropostaMovimentacaoCommand>
    {
        public SalvarPropostaMovimentacaoCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da Proposta para salvar o parecer da proposta");
            
            RuleFor(x => x.PropostaMovimentacaoDto.Justificativa)
                .NotEmpty()
                .WithMessage("Informe a justificativa para salvar o parecer da proposta");
            
            RuleFor(x => x.PropostaMovimentacaoDto.Situacao)
                .NotNull()
                .NotEmpty()
                .WithMessage("Informe a situação para salvar o parecer da proposta");
        }
    }
}