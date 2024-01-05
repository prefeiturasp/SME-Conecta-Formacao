using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQuery : IRequest<IEnumerable<DadosListagemInscricaoDTO>>
    {
        public ObterInscricaoPorIdQuery(long inscricaoId, FiltroListagemInscricaoDTO filtroListagemInscricaoDto)
        {
            InscricaoId = inscricaoId;
            filtros = filtroListagemInscricaoDto;
        }

        public long InscricaoId { get; set; }
        public FiltroListagemInscricaoDTO filtros { get; set; }
    }

    class ObterInscricaoPorIdQueryValidator : AbstractValidator<ObterInscricaoPorIdQuery>
    {
        public ObterInscricaoPorIdQueryValidator()
        {
            RuleFor(x => x.InscricaoId).GreaterThan(0).WithMessage("Informe o Id da Inscrição para realizar a consulta");
        }
    }
}