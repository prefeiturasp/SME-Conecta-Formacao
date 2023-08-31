using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaRascunhoCommand : IRequest<long>
    {
        public InserirPropostaRascunhoCommand(long areaPromotoraId, PropostaDTO propostaDTO)
        {
            AreaPromotoraId = areaPromotoraId;
            PropostaDTO = propostaDTO;
        }

        public long AreaPromotoraId { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    public class InserirPropostaRascunhoCommandValidator : AbstractValidator<InserirPropostaRascunhoCommand>
    {
        public InserirPropostaRascunhoCommandValidator()
        {
            RuleFor(f => f.AreaPromotoraId)
                .GreaterThan(0)
                .WithMessage("É nescessário informar o Id da área promotora para inserir a proposta");
        }
    }
}
