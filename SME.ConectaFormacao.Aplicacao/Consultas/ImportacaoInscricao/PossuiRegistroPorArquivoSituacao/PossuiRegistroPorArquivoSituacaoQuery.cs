using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class PossuiRegistroPorArquivoSituacaoQuery : IRequest<bool>
    {
        public PossuiRegistroPorArquivoSituacaoQuery(long importacaoArquivoId, SituacaoImportacaoArquivoRegistro situacaoVerificar)
        {
            ImportacaoArquivoId = importacaoArquivoId;
            SituacaoVerificar = situacaoVerificar;
        }

        public long ImportacaoArquivoId { get; }
        public SituacaoImportacaoArquivoRegistro SituacaoVerificar { get; }
    }

    public class PossuiRegistroPorArquivoSituacaoQueryValidator : AbstractValidator<PossuiRegistroPorArquivoSituacaoQuery>
    {
        public PossuiRegistroPorArquivoSituacaoQueryValidator()
        {
            RuleFor(x => x.ImportacaoArquivoId)
                .GreaterThan(0)
                .WithMessage("Informe o Id da importação arquivo para verificar se todos registros foram processados");
        }
    }
}
