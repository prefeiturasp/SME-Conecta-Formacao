using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarGrupoAreaPromotoraCommand : IRequest
    {
        public ValidarGrupoAreaPromotoraCommand(Guid grupoId, long ignorarAreaPromotoraId = 0)
        {
            GrupoId = grupoId;
            IgnorarAreaPromotoraId = ignorarAreaPromotoraId;
        }

        public Guid GrupoId { get; }
        public long IgnorarAreaPromotoraId { get; }
    }

    public class ValidarGrupoAreaPromotoraCommandValidator : AbstractValidator<ValidarGrupoAreaPromotoraCommand>
    {
        public ValidarGrupoAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.GrupoId)
                .NotEmpty()
                .WithMessage("É necessário informar o perfil para inserir a área promotora");
        }
    }
}
