using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Aplicacao
{

    [ExcludeFromCodeCoverage]
    public class InserirArquivoCommand : IRequest<long>
    {
        public InserirArquivoCommand(ArquivoDTO arquivo)
        {
            Arquivo = arquivo;
        }

        public ArquivoDTO Arquivo { get; }
    }


    [ExcludeFromCodeCoverage]
    public class InserirArquivoCommandValidator : AbstractValidator<InserirArquivoCommand>
    {
        public InserirArquivoCommandValidator()
        {
            RuleFor(x => x.Arquivo.Nome)
                .NotEmpty()
                .WithMessage("É necessário informar o nome do arquivo para inserir");

            RuleFor(x => x.Arquivo.TipoConteudo)
                .NotEmpty()
                .WithMessage("É necessário informar o tipo de conteudo do arquivo para inserir");

            RuleFor(x => x.Arquivo.FormFile)
                .NotEmpty()
                .WithMessage("É necessário informar o arquivo para inserir");
        }
    }
}
