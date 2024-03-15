using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterArquivosInscricaoImportadosQuery : IRequest<PaginacaoResultadoDTO<ArquivoInscricaoImportadoDTO>>
    {
        public ObterArquivosInscricaoImportadosQuery(int quantidadeRegistroIgnorados, int numeroRegistros, long propostaId)
        {
            QuantidadeRegistrosIgnorados = quantidadeRegistroIgnorados;
            NumeroRegistros = numeroRegistros;
            PropostaId = propostaId;
  
        }

        public int QuantidadeRegistrosIgnorados { get; set; }
        public int NumeroRegistros { get; set; }
        public long PropostaId { get; set; }
    }

    public class ObterArquivosInscricaoImportadosQueryValidator : AbstractValidator<ObterArquivosInscricaoImportadosQuery>
    {
        public ObterArquivosInscricaoImportadosQueryValidator()
        {
            RuleFor(x => x.PropostaId).GreaterThan(0).WithMessage("Informe o Id da proposta para obter os arquivos importados");
        }
    }
}
