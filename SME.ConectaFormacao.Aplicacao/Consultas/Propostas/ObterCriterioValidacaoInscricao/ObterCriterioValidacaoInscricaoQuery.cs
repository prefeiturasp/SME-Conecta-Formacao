using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCriterioValidacaoInscricaoQuery : IRequest<IEnumerable<CriterioValidacaoInscricaoDTO>>
    {
        public bool ExibirOutros { get; }

        public ObterCriterioValidacaoInscricaoQuery(bool exibirOutros)
        {
            ExibirOutros = exibirOutros;
        }
    }
}
