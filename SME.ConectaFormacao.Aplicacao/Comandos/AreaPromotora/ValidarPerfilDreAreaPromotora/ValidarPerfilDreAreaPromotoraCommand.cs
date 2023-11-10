using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarPerfilDreAreaPromotoraCommand : IRequest
    {
        public ValidarPerfilDreAreaPromotoraCommand(long dreId, Guid grupoId, long ignorarAreaPromotoraId = 0)
        {
            DreId = dreId;
            GrupoId = grupoId;
            IgnorarAreaPromotoraId = ignorarAreaPromotoraId;
        }

        public long DreId { get; set; }
        public long IgnorarAreaPromotoraId { get; set; }
        public Guid GrupoId { get; set; }
    }
    public class ValidarPerfilDreAreaPromotoraCommandValidator : AbstractValidator<ValidarPerfilDreAreaPromotoraCommand>
    {
        public ValidarPerfilDreAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.GrupoId)
                .NotEmpty()
                .WithMessage("É necessário informar o perfil para validar a área promotora");
            RuleFor(x => x.DreId)
                .NotEmpty()
                .WithMessage("É necessário informar a Dre para validar a área promotora");
        }
    }
}