using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.Mock;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.ServicoFake
{
    public class ObterCodigosDresQueryFake : IRequestHandler<ObterCodigosDresEOLQuery, IEnumerable<DreNomeAbreviacaoDTO>>
    {
        public async Task<IEnumerable<DreNomeAbreviacaoDTO>> Handle(ObterCodigosDresEOLQuery request, CancellationToken cancellationToken)
        {
            return SincronizarDreMock.GerarListaDreEol();
        }
    }
}
