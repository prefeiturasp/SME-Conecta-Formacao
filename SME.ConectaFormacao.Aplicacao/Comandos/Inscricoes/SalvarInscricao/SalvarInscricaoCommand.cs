using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoCommand : IRequest<RetornoDTO>
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
