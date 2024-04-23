using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaCommand : IRequest<long>
    {
        public SalvarPropostaPareceristaCommand(long propostaId,PropostaPareceristaDTO parecerista)
        {
            PropostaId = propostaId;
            Parecerista = parecerista;
        }

        public long PropostaId { get; }

        public PropostaPareceristaDTO Parecerista { get; }
    }

    public class SalvarPropostaPareceristaCommandValidator : AbstractValidator<SalvarPropostaPareceristaCommand>
    {
        public SalvarPropostaPareceristaCommandValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da Proposta Para salvar o Parecerista");
            RuleFor(x => x.Parecerista.RegistroFuncional).NotEmpty().WithMessage("Informe o RF  Para salvar o Parecerista");
            RuleFor(x => x.Parecerista.NomeParecerista).NotEmpty().WithMessage("Informe o nome Para salvar o Parecerista");
        }        
    }
}