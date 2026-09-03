using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Validadores;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaCommand : IRequest<RetornoDTO>
    {
        public AlterarPropostaCommand(long id, PropostaDTO propostaDTO)
        {
            Id = id;
            PropostaDTO = propostaDTO;
        }

        public long Id { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    public class AlterarPropostaCommandValidator : PropostaValidadorBase<AlterarPropostaCommand>
    {
        public AlterarPropostaCommandValidator()
        {
            RuleFor(f => f.Id)
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id para alterar a proposta");

            AdicionarValidacoesComuns(x => x.PropostaDTO);
        }
    }
}
