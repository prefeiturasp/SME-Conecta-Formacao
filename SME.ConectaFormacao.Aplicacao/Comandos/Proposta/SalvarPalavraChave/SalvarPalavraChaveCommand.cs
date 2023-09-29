using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPalavraChaveCommand : IRequest<bool>
    {
        public SalvarPalavraChaveCommand(long propostaId, IEnumerable<PropostaPalavraChave> palavrasChaves)
        {
            PropostaId = propostaId;
            PalavrasChaves = palavrasChaves;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaPalavraChave> PalavrasChaves { get; set; }
    }

    public class SalvarPalavraChaveCommandValidator : AbstractValidator<SalvarPalavraChaveCommand>
    {
        public SalvarPalavraChaveCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar as palavras chaves da proposta");
        }
    }
}
