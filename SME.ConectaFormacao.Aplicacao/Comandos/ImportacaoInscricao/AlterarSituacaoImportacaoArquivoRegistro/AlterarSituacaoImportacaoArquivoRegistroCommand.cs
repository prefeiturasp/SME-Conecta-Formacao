using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoImportacaoArquivoRegistroCommand : IRequest<bool>
    {
        public AlterarSituacaoImportacaoArquivoRegistroCommand(long id, SituacaoImportacaoArquivoRegistro situacao, string erro = "")
        {
            Id = id;
            Situacao = situacao;
            Erro = erro;
        }

        public long Id { get; }
        public SituacaoImportacaoArquivoRegistro Situacao { get; }
        public string Erro { get; }
    }

    public class AlterarSituacaoImportacaoArquivoRegistroCommandValidator : AbstractValidator<AlterarSituacaoImportacaoArquivoRegistroCommand>
    {
        public AlterarSituacaoImportacaoArquivoRegistroCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador da importação arquivo registro para alterar a situação");

            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("É necessário informar a situação da importação arquivo registro para alterar a situação");
        }
    }
}
