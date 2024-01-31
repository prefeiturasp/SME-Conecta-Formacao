using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.Mock;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.ServicoFake
{
    public class ObterCodigosDresQueryFake : IRequestHandler<ObterCodigosDresEOLQuery, IEnumerable<DreServicoEol>>
    {
        public async Task<IEnumerable<DreServicoEol>> Handle(ObterCodigosDresEOLQuery request, CancellationToken cancellationToken)
        {
            return SincronizarDreMock.GerarListaDreEol();
        }
    }
}
