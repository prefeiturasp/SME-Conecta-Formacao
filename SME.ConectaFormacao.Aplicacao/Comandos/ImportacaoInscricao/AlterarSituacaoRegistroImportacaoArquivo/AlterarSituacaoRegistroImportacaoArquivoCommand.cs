using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoRegistroImportacaoArquivoCommand : IRequest<bool>
    {
        public AlterarSituacaoRegistroImportacaoArquivoCommand(long registroImportacaoId, SituacaoImportacaoArquivoRegistro situacao)
        {
            RegistroImportacaoId = registroImportacaoId;
            Situacao = situacao;
        }

        public long RegistroImportacaoId { get; }
        public SituacaoImportacaoArquivoRegistro Situacao { get; }
    }

    public class AlterarSituacaoRegistroImportacaoArquivoCommandValidator : AbstractValidator<AlterarSituacaoRegistroImportacaoArquivoCommand>
    {
        public AlterarSituacaoRegistroImportacaoArquivoCommandValidator()
        {
            RuleFor(x => x.RegistroImportacaoId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador do registro de importação arquivo para alterar a situação");

            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("É necessário informar a situação do registro de importação arquivo para alterar a situação");
        }
    }
}
