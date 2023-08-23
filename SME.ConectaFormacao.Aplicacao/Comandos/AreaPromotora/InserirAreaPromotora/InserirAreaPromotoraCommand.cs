using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirAreaPromotoraCommand : IRequest<long>
    {
        public InserirAreaPromotoraCommand(AreaPromotoraDTO areaPromotoraDTO)
        {
            AreaPromotoraDTO = areaPromotoraDTO;
        }

        public AreaPromotoraDTO AreaPromotoraDTO { get; }
    }

    public class InserirAreaPromotoraCommandValidator : AbstractValidator<InserirAreaPromotoraCommand>
    {
        public InserirAreaPromotoraCommandValidator()
        {
            RuleFor(x => x.AreaPromotoraDTO.Nome)
                .NotEmpty()
                .WithMessage("É nescessário informar o nome para inserir a área promotora");

            RuleFor(x => x.AreaPromotoraDTO.Nome)
                .MaximumLength(50)
                .WithMessage("O nome da área promotora não pode conter mais que 50 caracteres");

            RuleFor(x => x.AreaPromotoraDTO.GrupoId)
                .NotEmpty()
                .WithMessage("É nescessário informar o perfil para inserir a área promotora");

            RuleFor(x => x.AreaPromotoraDTO.Email)
                .EmailAddress()
                .WithMessage("É nescessário informar um email válido para inserir a área promotora");
        }
    }
}
