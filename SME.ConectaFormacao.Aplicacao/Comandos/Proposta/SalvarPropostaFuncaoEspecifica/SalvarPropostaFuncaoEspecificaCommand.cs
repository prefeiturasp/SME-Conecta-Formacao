using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaFuncaoEspecificaCommand : IRequest<bool>
    {
        public SalvarPropostaFuncaoEspecificaCommand(long propostaId, IEnumerable<PropostaFuncaoEspecifica> funcaoEspecificas)
        {
            PropostaId = propostaId;
            FuncaoEspecificas = funcaoEspecificas;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaFuncaoEspecifica> FuncaoEspecificas { get; set; }
    }

    public class SalvarPropostaFuncaoEspecificaCommandValidator : AbstractValidator<SalvarPropostaFuncaoEspecificaCommand>
    {
        public SalvarPropostaFuncaoEspecificaCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É nescessário informar o id da proposta para salvar os funções específicas");
        }
    }
}
