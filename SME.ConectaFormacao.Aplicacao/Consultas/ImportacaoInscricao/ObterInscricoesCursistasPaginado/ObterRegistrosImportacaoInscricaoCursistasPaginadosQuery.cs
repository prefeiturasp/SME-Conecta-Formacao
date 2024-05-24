using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery : IRequest<PaginacaoResultadoDTO<ImportacaoArquivoRegistroDTO>>
    {
        public ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery(int numeroPagina, int numeroRegistros, long importacaoArquivoId, SituacaoImportacaoArquivoRegistro? ignorarSituacao)
        {
            NumeroPagina = numeroPagina;
            NumeroRegistros = numeroRegistros;
            ImportacaoArquivoId = importacaoArquivoId;
            IgnorarSituacao = ignorarSituacao;
        }

        public int NumeroPagina { get; set; }
        public int NumeroRegistros { get; set; }
        public long ImportacaoArquivoId { get; set; }
        public SituacaoImportacaoArquivoRegistro? IgnorarSituacao { get; set; }
    }

    public class ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryValidator : AbstractValidator<ObterRegistrosImportacaoInscricaoCursistasPaginadosQuery>
    {
        public ObterRegistrosImportacaoArquivoInscricaoCursistasPaginadosQueryValidator()
        {
            RuleFor(x => x.ImportacaoArquivoId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da importação arquivo para obter os registros das inscrições dos cursistas");
        }
    }
}