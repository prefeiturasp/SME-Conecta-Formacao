using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorIdQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterUsuarioPorIdQueryValidator : AbstractValidator<ObterUsuarioPorIdQuery>
    {
        public ObterUsuarioPorIdQueryValidator()
        {
            RuleFor(f => f.Id)
                .NotEmpty()
                .WithMessage("Informe o id para obter o usuário por id");
        }
    }
}
