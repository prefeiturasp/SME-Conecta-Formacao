using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosDaIncricaoInconsistentesQuery : IRequest<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDTO>>
    {
        public ObterRegistrosDaIncricaoInconsistentesQuery(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId)
        {
            QuantidadeRegistrosIgnorados = quantidadeRegistroIgnorados;
            NumeroRegistros = numeroRegistros;
            ArquivoId = arquivoId;

        }

        public int QuantidadeRegistrosIgnorados { get; set; }
        public int NumeroRegistros { get; set; }
        public long ArquivoId { get; set; }
    }

    public class ObterRegistrosDaIncricaoInconsistentesQueryValidator : AbstractValidator<ObterRegistrosDaIncricaoInconsistentesQuery>
    {
        public ObterRegistrosDaIncricaoInconsistentesQueryValidator()
        {
            RuleFor(x => x.ArquivoId).GreaterThan(0).WithMessage("Informe o Id do arquivo para obter os registros inconsistentes");
        }
    }
}
