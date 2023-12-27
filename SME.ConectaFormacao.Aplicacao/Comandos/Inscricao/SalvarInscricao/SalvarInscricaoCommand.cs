using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoCommand : IRequest<long>
    {
        public SalvarInscricaoCommand(InscricaoDTO inscricaoDTO)
        {
            InscricaoDTO = inscricaoDTO;
        }

        public InscricaoDTO InscricaoDTO { get; }
    }

    public class SalvarInscricaoCommandValidator : AbstractValidator<SalvarInscricaoCommand>
    {
        public SalvarInscricaoCommandValidator()
        {
        }
    }
}
