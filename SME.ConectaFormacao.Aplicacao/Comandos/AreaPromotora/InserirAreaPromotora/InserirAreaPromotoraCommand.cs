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
                .WithMessage("É nescessário informar a área promotora para inserir");

            RuleFor(x => x.AreaPromotoraDTO.Nome)
                .MaximumLength(50)
                .WithMessage("A área promotora não pode conter mais que 50 caracteres");

            RuleFor(x => x.AreaPromotoraDTO.GrupoId)
                .NotEmpty()
                .WithMessage("É nescessário informar o perfil para inserir a área promotora");
        }
    }
}
