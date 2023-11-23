using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.ServicosFakes
{
    public class ObterGrupoPorIdQueryHandlerFaker : IRequestHandler<ObterGrupoPorIdQuery, GrupoDTO>
    {
        public async Task<GrupoDTO> Handle(ObterGrupoPorIdQuery request, CancellationToken cancellationToken)
        {
            return new GrupoDTO { VisaoId = 1 };
        }
    }
}
