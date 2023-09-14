using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Arquivo;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirArquivoCommand : IRequest<long>
    {
        public InserirArquivoCommand(ArquivoDTO arquivo)
        {
            Arquivo = arquivo;
        }

        public ArquivoDTO Arquivo { get; }
    }

    public class InserirArquivoCommandValidator : AbstractValidator<InserirArquivoCommand>
    {
        public InserirArquivoCommandValidator()
        {
            RuleFor(x => x.Arquivo.Nome)
                .NotEmpty()
                .WithMessage("É nescessário informar o nome do arquivo para inserir");

            RuleFor(x => x.Arquivo.TipoConteudo)
                .NotEmpty()
                .WithMessage("É nescessário informar o tipo de conteudo do arquivo para inserir");

            RuleFor(x => x.Arquivo.FormFile)
                .NotEmpty()
                .WithMessage("É nescessário informar o arquivo para inserir");
        }
    }
}
