using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirImportacaoArquivoCommand : IRequest<long>
    {
        public InserirImportacaoArquivoCommand(ImportacaoArquivoDTO importacaoArquivo)
        {
            ImportacaoArquivo = importacaoArquivo;
        }

        public ImportacaoArquivoDTO ImportacaoArquivo { get; }
    }

    public class InserirImportacaoArquivoInscricaoCursistaCommandValidator : AbstractValidator<InserirImportacaoArquivoCommand>
    {
        public InserirImportacaoArquivoInscricaoCursistaCommandValidator()
        {
            RuleFor(x => x.ImportacaoArquivo.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar o nome do arquivo para inserir importação de arquivo");

            RuleFor(x => x.ImportacaoArquivo.Tipo)
                .NotEmpty()
                .WithMessage("É necessário informar o tipo de conteudo do arquivo para inserir importação de arquivo");

            RuleFor(x => x.ImportacaoArquivo)
                .NotEmpty()
                .WithMessage("É necessário informar o arquivo para inserir importação de arquivo");
            
            RuleFor(x => x.ImportacaoArquivo.PropostaId)
                .NotEmpty()
                .WithMessage("É necessário informar o número da proposta para inserir importação de arquivo");
        }
    }
}
