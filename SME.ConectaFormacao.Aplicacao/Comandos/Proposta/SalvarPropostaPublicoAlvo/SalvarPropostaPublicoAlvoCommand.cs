using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPublicoAlvoCommand : IRequest<bool>
    {
        public SalvarPropostaPublicoAlvoCommand(long propostaId, IEnumerable<PropostaPublicoAlvo> publicosAlvo)
        {
            PropostaId = propostaId;
            PublicosAlvo = publicosAlvo;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaPublicoAlvo> PublicosAlvo { get; set; }
    }

    public class SalvarPropostaPublicoAlvoCommandValidator : AbstractValidator<SalvarPropostaPublicoAlvoCommand>
    {
        public SalvarPropostaPublicoAlvoCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar os publicos alvo");
        }
    }
}
