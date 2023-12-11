using FluentValidation;
using MediatR;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterEnderecoArquivoServicoArmazenamentoQuery : IRequest<string>
    {
        public ObterEnderecoArquivoServicoArmazenamentoQuery(string nomeArquivoFisico, bool ehTemp)
        {
            NomeArquivoFisico = nomeArquivoFisico;
            EhTemp = ehTemp;
        }

        public string NomeArquivoFisico { get; }
        public bool EhTemp { get; }
    }

    public class ObterEnderecoArquivoServicoArmazenamentoQueryValidator : AbstractValidator<ObterEnderecoArquivoServicoArmazenamentoQuery>
    {
        public ObterEnderecoArquivoServicoArmazenamentoQueryValidator()
        {
            RuleFor(x => x.NomeArquivoFisico)
                .NotEmpty()
                .WithMessage("É necessário informar o nome do arquivo fisico para obter o endereço");
        }
    }
}
