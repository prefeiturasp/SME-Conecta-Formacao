using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaDreCommand(long propostaId, IEnumerable<PropostaDre> dres) : IRequest<bool>
    {
        public long PropostaId { get; } = propostaId;
        public IEnumerable<PropostaDre> Dres { get; } = dres;
    }

    public class SalvarPropostaDreCommandValidator : AbstractValidator<SalvarPropostaDreCommand>
    {
        public SalvarPropostaDreCommandValidator()
        {
        }
    }
}
