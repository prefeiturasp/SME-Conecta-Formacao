using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class TodosRegistroForamProcessadosQuery : IRequest<bool>
    {
        public TodosRegistroForamProcessadosQuery(long importacaoArquivoId)
        {
            ImportacaoArquivoId = importacaoArquivoId;
        }

        public long ImportacaoArquivoId { get; set; }
    }

    public class TodosRegistroForamProcessadosQueryValidator : AbstractValidator<TodosRegistroForamProcessadosQuery>
    {
        public TodosRegistroForamProcessadosQueryValidator()
        {
            RuleFor(x => x.ImportacaoArquivoId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da importação arquivo para verificar se todos registros foram processados");
        }
    }
}
