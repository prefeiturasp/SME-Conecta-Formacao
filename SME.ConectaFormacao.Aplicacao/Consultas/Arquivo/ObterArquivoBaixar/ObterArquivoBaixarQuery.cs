using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterArquivoBaixarQuery : IRequest<ArquivoBaixadoDTO>
    {
        public ObterArquivoBaixarQuery(Guid codigo)
        {
            Codigo = codigo;
        }

        public Guid Codigo { get; }
    }

    public class ObterArquivoBaixarQueryValidator : AbstractValidator<ObterArquivoBaixarQuery>
    {
        public ObterArquivoBaixarQueryValidator()
        {
            RuleFor(b => b.Codigo)
                .NotEmpty()
                .WithMessage("É necessário informar o código do arquivo para baixar");
        }
    }
}
