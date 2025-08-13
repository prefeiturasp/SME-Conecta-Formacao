using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TransferirInscricaoCommand : IRequest<RetornoInscricaoDTO>
    {
        public TransferirInscricaoCommand(InscricaoTransferenciaDTO inscricaoTransferenciaDTO)
        {
            InscricaoTransferenciaDTO = inscricaoTransferenciaDTO;
        }
        public InscricaoTransferenciaDTO InscricaoTransferenciaDTO { get; set; }
    }

    public class TransferirInscricaoCommandValidator : AbstractValidator<TransferirInscricaoCommand>
    {
        public TransferirInscricaoCommandValidator()
        {
            RuleFor(t => t.InscricaoTransferenciaDTO)
                .NotNull()
                .WithMessage("É necessário informar os dados da transferência");

            RuleFor(t => t.InscricaoTransferenciaDTO.IdTurmaDestino)
                .NotEmpty()
                .WithMessage("É necessário informar o id da turma destino")
                .When(t => t.InscricaoTransferenciaDTO != null);

            RuleFor(t => t.InscricaoTransferenciaDTO.Cursistas)
               .NotEmpty()
               .WithMessage("É necessário informar ao menos um cursista")
               .When(t => t.InscricaoTransferenciaDTO != null);
        }
    }
}
