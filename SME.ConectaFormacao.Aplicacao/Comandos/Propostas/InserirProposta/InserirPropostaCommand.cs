using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Validadores;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaCommand : IRequest<RetornoDTO>
    {
        public InserirPropostaCommand(long areaPromotoraId, PropostaDTO propostaDTO)
        {
            PropostaDTO = propostaDTO;
            AreaPromotoraId = areaPromotoraId;
        }

        public long AreaPromotoraId { get; set; }

        public PropostaDTO PropostaDTO { get; }
    }

    public class InserirPropostaCommandValidator : PropostaValidadorBase<InserirPropostaCommand>
    {
        public InserirPropostaCommandValidator()
        {
            RuleFor(f => f.AreaPromotoraId)
                .GreaterThan(0)
                .WithMessage("É necessário informar o Id da área promotora para inserir a proposta");

            AdicionarValidacoesComuns(x => x.PropostaDTO);
        }
    }
}
