using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Inscricoes
{
    public class ObterRegistrosDaIncricaoInconsistentesQuery(int quantidadeRegistroIgnorados, int numeroRegistros, long arquivoId) : 
        IRequest<PaginacaoResultadoComSucessoDTO<RegistroDaInscricaoInsconsistenteDto>>
    {
        public int QuantidadeRegistrosIgnorados { get; set; } = quantidadeRegistroIgnorados;
        public int NumeroRegistros { get; set; } = numeroRegistros;
        public long ArquivoId { get; set; } = arquivoId;
    }

    public class ObterRegistrosDaIncricaoInconsistentesQueryValidator : AbstractValidator<ObterRegistrosDaIncricaoInconsistentesQuery>
    {
        public ObterRegistrosDaIncricaoInconsistentesQueryValidator()
        {
            RuleFor(x => x.ArquivoId).GreaterThan(0).WithMessage("Informe o Id do arquivo para obter os registros inconsistentes");
        }
    }
}
