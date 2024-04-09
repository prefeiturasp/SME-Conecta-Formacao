using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosPorPerfisServicoEolQuery : IRequest<IEnumerable<UsuarioPerfilServicoEol>>
    {
        public ObterUsuariosPorPerfisServicoEolQuery(Guid[] perfis)
        {
            Perfis = perfis;
        }

        public Guid[] Perfis { get; }
    }

    public class ObterUsuariosPorPerfisServicoEolQueryValidator : AbstractValidator<ObterUsuariosPorPerfisServicoEolQuery>
    {
        public ObterUsuariosPorPerfisServicoEolQueryValidator()
        {
            RuleFor(c => c.Perfis)
                .NotNull()
                .WithMessage("Os perfis devem ser informados para obter os usuários.");
        }
    }
}