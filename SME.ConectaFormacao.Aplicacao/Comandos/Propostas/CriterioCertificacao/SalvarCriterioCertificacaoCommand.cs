using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarCriterioCertificacaoCommand : IRequest<bool>
    {
        public SalvarCriterioCertificacaoCommand(long propostaId, IEnumerable<PropostaCriterioCertificacao> criterioCertificacaos)
        {
            PropostaId = propostaId;
            CriterioCertificacaos = criterioCertificacaos;
        }

        public long PropostaId { get; set; }
        public IEnumerable<PropostaCriterioCertificacao> CriterioCertificacaos { get; set; }

        [ExcludeFromCodeCoverage]
        public class CriterioCertificacaoCommandValidator : AbstractValidator<SalvarCriterioCertificacaoCommand>
        {
            public CriterioCertificacaoCommandValidator()
            {
                RuleFor(x => x.PropostaId)
                    .NotEmpty()
                    .WithMessage("É necessário informar o id da proposta para salvar as palavras chaves da proposta");
            }
        }
    }
}