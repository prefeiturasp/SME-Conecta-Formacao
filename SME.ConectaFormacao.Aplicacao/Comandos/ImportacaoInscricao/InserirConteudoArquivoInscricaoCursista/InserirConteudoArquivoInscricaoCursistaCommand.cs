using FluentValidation;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirConteudoArquivoInscricaoCursistaCommand : IRequest<bool>
    {
        public InserirConteudoArquivoInscricaoCursistaCommand(long importacaoArquivoId, Stream streamArquivo)
        {
            ImportacaoArquivoId = importacaoArquivoId;
            StreamArquivo = streamArquivo;
        }

        public long ImportacaoArquivoId { get; set; }
        public Stream StreamArquivo { get; set; }
    }


    [ExcludeFromCodeCoverage]
    public class InserirConteudoArquivoInscricaoCursistaCommandValidator : AbstractValidator<InserirConteudoArquivoInscricaoCursistaCommand>
    {
        public InserirConteudoArquivoInscricaoCursistaCommandValidator()
        {
            RuleFor(r => r.ImportacaoArquivoId)
                .NotEmpty()
                .WithMessage("É necessário informar o identificador do arquivo para obter e inserir o conteúdo da planilha de inscrição cursista");

            RuleFor(r => r.StreamArquivo)
                .NotEmpty()
                .WithMessage("É necessário informar o stream do arquivo para obter e inserir o conteúdo da planilha de inscrição cursista");
        }
    }
}