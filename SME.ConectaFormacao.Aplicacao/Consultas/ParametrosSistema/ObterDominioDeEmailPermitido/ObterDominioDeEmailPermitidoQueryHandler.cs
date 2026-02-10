using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.ObterDominioDeEmailPermitido
{
    public class ObterDominioDeEmailPermitidoQueryHandler(IRepositorioParametroSistema repositorioParametroSistema) : 
        IRequestHandler<ObterDominioDeEmailPermitidoQuery, IEnumerable<string>>
    {
        public async Task<IEnumerable<string>> Handle(ObterDominioDeEmailPermitidoQuery request, CancellationToken cancellationToken)
        {
            return await repositorioParametroSistema.ObterDominiosPermitidosParaUesParceirasAsync();
        }
    }
}