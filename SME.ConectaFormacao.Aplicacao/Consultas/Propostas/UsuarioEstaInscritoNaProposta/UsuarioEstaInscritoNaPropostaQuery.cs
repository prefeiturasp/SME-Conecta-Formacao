using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class UsuarioEstaInscritoNaPropostaQuery : IRequest<bool>
    {
        public UsuarioEstaInscritoNaPropostaQuery(long usuarioId, long propostaId)
        {
            UsuarioId = usuarioId;
            PropostaId = propostaId;
        }

        public long UsuarioId { get; }
        public long PropostaId { get; }
    }

    [ExcludeFromCodeCoverage]
    public class ValidarUsuarioExisteInscricaoNaPropostaQueryValidator : AbstractValidator<UsuarioEstaInscritoNaPropostaQuery>
    {
        public ValidarUsuarioExisteInscricaoNaPropostaQueryValidator()
        {
            RuleFor(r => r.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador da proposta para identificar se usuário já está inscrito na proposta.");

            RuleFor(r => r.UsuarioId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador do usuário para identificar se usuário já está inscrito na proposta.");
        }
    }
}
