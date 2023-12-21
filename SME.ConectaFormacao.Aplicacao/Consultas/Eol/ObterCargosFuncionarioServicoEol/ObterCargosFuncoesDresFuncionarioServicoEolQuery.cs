using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargosFuncoesDresFuncionarioServicoEolQuery : IRequest<IEnumerable<CargoFuncionarioConectaDTO>>
    {
        public ObterCargosFuncoesDresFuncionarioServicoEolQuery(string registroFuncional)
        {
            RegistroFuncional = registroFuncional;
        }

        public string RegistroFuncional { get; }
    }

    public class ObterCargosFuncoesDresFuncionarioServicoEolQueryValidator : AbstractValidator<ObterCargosFuncoesDresFuncionarioServicoEolQuery>
    {
        public ObterCargosFuncoesDresFuncionarioServicoEolQueryValidator()
        {
            RuleFor(t => t.RegistroFuncional)
                .NotEmpty()
                .WithMessage("É necessário informar o Registro Funcional do usuário para consultar os cargos no EOL");

        }
    }
}
