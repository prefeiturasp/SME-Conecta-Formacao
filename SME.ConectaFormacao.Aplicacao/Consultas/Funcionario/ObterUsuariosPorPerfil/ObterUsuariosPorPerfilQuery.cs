using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUsuariosPorPerfilQuery : IRequest<IEnumerable<RetornoUsuarioLoginNomeDTO>>
    {
        public ObterUsuariosPorPerfilQuery(params Guid[] perfis)
        {
            Perfis = perfis;
        }

        public Guid[] Perfis { get; set; }
    }
    
    public class ObterUsuariosPorPerfilQueryValidator : AbstractValidator<ObterUsuariosPorPerfilQuery>
    {
        public ObterUsuariosPorPerfilQueryValidator()
        {
            RuleFor(x => x.Perfis)
                .NotNull()
                .WithMessage("Informe o(s) perfil(s) para obter os usuarios");
        }
    }
}