using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioPorIdQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorIdQuery(long usuarioId = default)
        {
            UsuarioId = usuarioId;
        }

        public long UsuarioId { get; set; }
    }

    public class ObterUsuarioPorIdQueryValidator : AbstractValidator<ObterUsuarioPorIdQuery>
    {
        public ObterUsuarioPorIdQueryValidator()
        {
            RuleFor(x => x.UsuarioId).GreaterThan(0).WithMessage("Informe o Id do Usuario para realizar a consulta");
        }
    }
}