using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosFuncionarioExterno
{
    public class ObterDadosFuncionarioExternoQuery : IRequest<IEnumerable<FuncionarioExternoServicoEol>>
    {
        public ObterDadosFuncionarioExternoQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }

    public class ObterDadosFuncionarioExternoQueryValidator : AbstractValidator<ObterDadosFuncionarioExternoQuery>
    {
        public ObterDadosFuncionarioExternoQueryValidator()
        {
            RuleFor(x => x.Cpf).NotNull().NotEmpty().WithMessage("É necessario informar o cpf para consultar os dados do usuário");
        }
    }
}