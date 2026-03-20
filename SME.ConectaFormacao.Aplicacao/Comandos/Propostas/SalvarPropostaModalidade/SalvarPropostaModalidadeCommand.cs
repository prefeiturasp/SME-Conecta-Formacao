using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaModalidadeCommand : IRequest<bool>
    {
        public SalvarPropostaModalidadeCommand(long propostaId, IEnumerable<PropostaModalidade> modalidades)
        {
            PropostaId = propostaId;
            Modalidades = modalidades;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaModalidade> Modalidades { get; set; }
    }

    public class SalvarPropostaModalidadeCommandValidator : AbstractValidator<SalvarPropostaModalidadeCommand>
    {
        public SalvarPropostaModalidadeCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar as modalidades da proposta");
        }
    }
}
