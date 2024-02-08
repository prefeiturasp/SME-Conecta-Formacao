using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.ObterDominioDeEmailPermitido
{
    public class ObterDominioDeEmailPermitidoQueryHandler : IRequestHandler<ObterDominioDeEmailPermitidoQuery, IEnumerable<string>>
    {
        private readonly IRepositorioParametroSistema repositorioParametroSistema;

        public ObterDominioDeEmailPermitidoQueryHandler(IRepositorioParametroSistema repositorioParametroSistema)
        {
            this.repositorioParametroSistema = repositorioParametroSistema ?? throw new ArgumentNullException(nameof(repositorioParametroSistema));
        }

        public async Task<IEnumerable<string>> Handle(ObterDominioDeEmailPermitidoQuery request, CancellationToken cancellationToken)
        {
            return await repositorioParametroSistema.ObterDominiosPermitidosParaUesParceiras();
        }
    }
}