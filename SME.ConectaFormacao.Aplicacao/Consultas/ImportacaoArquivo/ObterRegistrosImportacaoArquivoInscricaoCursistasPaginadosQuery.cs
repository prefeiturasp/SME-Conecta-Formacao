using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery : IRequest<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>>
    {
        public ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery(int numeroPagina, int numeroRegistros, long importacaoArquivoId)
        {
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            ImportacaoArquivoId = importacaoArquivoId;
        }

        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
        public long ImportacaoArquivoId { get; set; }
    }
    
    public class ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryValidator : AbstractValidator<ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQuery>
    {
        public ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryValidator()
        {
            RuleFor(x => x.ImportacaoArquivoId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da importação arquivo para obter os registros das inscrições dos cursistas");
        }
    }
}