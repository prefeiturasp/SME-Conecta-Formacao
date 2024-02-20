using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaDreCommand : IRequest<bool>
    {
        public SalvarPropostaDreCommand(long propostaId, IEnumerable<PropostaDre> dres)
        {
            PropostaId = propostaId;
            Dres = dres;
        }

        public long PropostaId { get; }
        public IEnumerable<PropostaDre> Dres { get; }
    }

    public class SalvarPropostaDreCommandValidator : AbstractValidator<SalvarPropostaDreCommand>
    {
        public SalvarPropostaDreCommandValidator()
        {
        }
    }
}
