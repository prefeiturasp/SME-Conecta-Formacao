using FluentValidation;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaPorGrupoDresCodigoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterAreaPromotoraListaPorGrupoDresCodigoQuery(Guid grupoId, IEnumerable<string> dresCodigo)
        {
            GrupoId = grupoId;
            DresCodigo = dresCodigo;
        }

        public IEnumerable<string> DresCodigo { get; set; }
        public Guid GrupoId { get; }
    }
    
    public class ObterAreaPromotoraListaPorGrupoDresCodigoQueryValidator : AbstractValidator<ObterAreaPromotoraListaPorGrupoDresCodigoQuery>
    {
        public ObterAreaPromotoraListaPorGrupoDresCodigoQueryValidator()
        {
            RuleFor(x => x.GrupoId)
                .NotEmpty()
                .WithMessage("É necessário informar o grupo do perfil para obter dados da área promotora");
        }
    }
}
