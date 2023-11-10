using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.Mock;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.ServicoFake
{
    public class ObterCodigosDresQueryFake : IRequestHandler<ObterCodigosDresQuery, IEnumerable<DreNomeAbreviacaoDTO>>
    {
        public async Task<IEnumerable<DreNomeAbreviacaoDTO>> Handle(ObterCodigosDresQuery request, CancellationToken cancellationToken)
        {
            return SincronizarDreMock.GerarListaDreEol();
        }
    }
}
