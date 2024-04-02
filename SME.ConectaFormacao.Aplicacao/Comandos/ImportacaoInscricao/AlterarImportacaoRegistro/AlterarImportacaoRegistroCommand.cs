using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarImportacaoRegistroCommand : IRequest<bool>
    {
        public AlterarImportacaoRegistroCommand(long id, SituacaoImportacaoArquivoRegistro situacao, string conteudo, string erro = "")
        {
            Id = id;
            Situacao = situacao;
            Conteudo = conteudo;
            Erro = erro;
        }

        public long Id { get; }
        public SituacaoImportacaoArquivoRegistro Situacao { get; }
        public string Conteudo { get; }
        public string Erro { get; }
    }

    public class AlterarImportacaoRegistroCommandValidator : AbstractValidator<AlterarImportacaoRegistroCommand>
    {
        public AlterarImportacaoRegistroCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador da importação arquivo registro para alterar a situação");

            RuleFor(x => x.Situacao)
                .NotEmpty()
                .WithMessage("É necessário informar a situação da importação arquivo registro para alterar a situação");

            RuleFor(x => x.Situacao)
                .NotEqual(SituacaoImportacaoArquivoRegistro.Erro)
                .WithMessage("Não é permitido alterar o registro para erro com o command AlterarImportacaoRegistroCommand");
        }
    }
}
