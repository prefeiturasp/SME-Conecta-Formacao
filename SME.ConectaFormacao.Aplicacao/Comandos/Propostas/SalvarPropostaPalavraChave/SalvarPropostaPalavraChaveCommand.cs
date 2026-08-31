using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarPropostaPalavraChaveCommand : IRequest<bool>
    {
        public SalvarPropostaPalavraChaveCommand(long propostaId, IEnumerable<PropostaPalavraChave> palavrasChaves)
        {
            PropostaId = propostaId;
            PalavrasChaves = palavrasChaves;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaPalavraChave> PalavrasChaves { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class SalvarPropostaPalavraChaveCommandValidator : AbstractValidator<SalvarPropostaPalavraChaveCommand>
    {
        public SalvarPropostaPalavraChaveCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar as palavras chaves da proposta");
        }
    }
}
