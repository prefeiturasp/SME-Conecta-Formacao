using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.ServicoFake
{
    public class ObterCodigosDresQueryFake : IRequestHandler<ObterCodigosDresQuery, IEnumerable<DreNomeAbreviacaoDTO>>
    {
        public async Task<IEnumerable<DreNomeAbreviacaoDTO>> Handle(ObterCodigosDresQuery request, CancellationToken cancellationToken)
        {
            var lista = new List<DreNomeAbreviacaoDTO>()
            {
                new DreNomeAbreviacaoDTO("1","Dre JT","Dre JT"),
                new DreNomeAbreviacaoDTO("1", "Dre JT", "Dre JT"),
                new DreNomeAbreviacaoDTO("1", "Dre JT", "Dre JT"),
            };

            return lista;
        }
    }
}
