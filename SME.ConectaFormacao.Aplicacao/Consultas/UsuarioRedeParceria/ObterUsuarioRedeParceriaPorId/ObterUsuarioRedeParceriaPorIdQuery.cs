using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuarioRedeParceriaPorIdQuery : IRequest<UsuarioRedeParceriaDTO>
    {
        public ObterUsuarioRedeParceriaPorIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class ObterUsuarioRedeParceriaPorIdQueryValidator : AbstractValidator<ObterUsuarioRedeParceriaPorIdQuery>
    {
        public ObterUsuarioRedeParceriaPorIdQueryValidator()
        {
            RuleFor(f => f.Id)
                .NotEmpty()
                .WithMessage("Informe o id para obter o usuário rede parceria por id");
        }
    }
}
