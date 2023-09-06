using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterModalidadesQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterModalidadesQuery(TipoFormacao tipoFormacao)
        {
            TipoFormacao = tipoFormacao;
        }

        public TipoFormacao TipoFormacao { get; }
    }

    public class ObterModalidadesQueryValidator : AbstractValidator<ObterModalidadesQuery>
    {
        public ObterModalidadesQueryValidator()
        {
            RuleFor(x => x.TipoFormacao)
                .NotEmpty()
                .WithMessage("É nescessário informar o tipo de formação para obter as modalidades");
        }
    }
}
