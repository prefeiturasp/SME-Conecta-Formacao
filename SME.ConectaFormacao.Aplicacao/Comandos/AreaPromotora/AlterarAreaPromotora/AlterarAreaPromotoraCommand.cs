using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarAreaPromotoraCommand : IRequest<bool>
    {
        public AlterarAreaPromotoraCommand(long id, AreaPromotoraDTO areaPromotoraDTO)
        {
            Id = id;
            AreaPromotoraDTO = areaPromotoraDTO;
        }

        public long Id { get; }
        public AreaPromotoraDTO AreaPromotoraDTO { get; }
    }

    public class AlterarAreaPromotoraCommandValidator : AbstractValidator<AlterarAreaPromotoraCommand>
    {
        public AlterarAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.AreaPromotoraDTO.Nome)
                .NotEmpty()
                .WithMessage("É nescessário informar a área promotora para alterar");

            RuleFor(x => x.AreaPromotoraDTO.Nome)
                .MaximumLength(50)
                .WithMessage("A área promotora não pode conter mais que 50 caracteres");

            RuleFor(x => x.AreaPromotoraDTO.GrupoId)
                .NotEmpty()
                .WithMessage("É nescessário informar o perfil para alterar a área promotora");

            RuleFor(x => x.AreaPromotoraDTO.Email)
                .MaximumLength(100)
                .WithMessage("O email não pode conter mais que 50 caracteres para alterar a área promotora");
        }
    }
}
