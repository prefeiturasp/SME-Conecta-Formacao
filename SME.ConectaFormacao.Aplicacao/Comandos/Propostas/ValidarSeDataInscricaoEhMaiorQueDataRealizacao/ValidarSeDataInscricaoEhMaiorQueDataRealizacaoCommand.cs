using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand : IRequest<string>
    {
        public ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(DateTime? dataInscricaoFim, DateTime? dataRealizacaoFim)
        {
            DataInscricaoFim = dataInscricaoFim;
            DataRealizacaoFim = dataRealizacaoFim;
        }

        public DateTime? DataInscricaoFim { get; set; }
        public DateTime? DataRealizacaoFim { get; set; }
    }

    public class ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommandValidator : AbstractValidator<ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand>
    {
        public ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommandValidator()
        {
            RuleFor(x => x.DataInscricaoFim).NotNull().WithMessage("Informe a Data fim de Inscrição");
            RuleFor(x => x.DataRealizacaoFim).NotNull().WithMessage("Informe a Data fim de Realização");
        }
    }
}