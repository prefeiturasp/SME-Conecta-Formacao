using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormatosQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterFormatosQuery(TipoFormacao tipoFormacao)
        {
            TipoFormacao = tipoFormacao;
        }

        public TipoFormacao TipoFormacao { get; }
    }

    public class ObterFormatosQueryValidator : AbstractValidator<ObterFormatosQuery>
    {
        public ObterFormatosQueryValidator()
        {
            RuleFor(x => x.TipoFormacao)
                .NotEmpty()
                .WithMessage("É necessário informar o tipo de formação para obter os formatos");
        }
    }
}
