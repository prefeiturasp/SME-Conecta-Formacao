using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCriterioValidacaoInscricaoQuery : IRequest<IEnumerable<CriterioValidacaoInscricaoDTO>>
    {
        private static ObterCriterioValidacaoInscricaoQuery _instancia;
        public static ObterCriterioValidacaoInscricaoQuery Instancia => _instancia ??= new();
    }
}
