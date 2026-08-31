using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    [ExcludeFromCodeCoverage]
    public class SalvarPropostaTipoInscricaoCommand : IRequest<bool>
    {
        public SalvarPropostaTipoInscricaoCommand(long propostaId, IEnumerable<PropostaTipoInscricao> tiposInscricao)
        {
            PropostaId = propostaId;
            TiposInscricao = tiposInscricao;
        }

        public long PropostaId { get; }
        public IEnumerable<PropostaTipoInscricao> TiposInscricao { get; }
    }

    [ExcludeFromCodeCoverage]
    public class SalvarPropostaTipoInscricaoCommandValidator : AbstractValidator<SalvarPropostaTipoInscricaoCommand>
    {
        public SalvarPropostaTipoInscricaoCommandValidator()
        {
            RuleFor(x => x.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o id da proposta para salvar os tipos de inscrições");
        }
    }
}
