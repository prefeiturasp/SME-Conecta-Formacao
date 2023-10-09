using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterParametroSistemaPorTipoEAnoQueryHandler : IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, ParametroSistema>
    {
        private readonly IRepositorioParametroSistema repositorioParametroSistema;

        public ObterParametroSistemaPorTipoEAnoQueryHandler(IRepositorioParametroSistema repositorioParametroSistema)
        {
            this.repositorioParametroSistema = repositorioParametroSistema ?? throw new ArgumentNullException(nameof(repositorioParametroSistema));
        }

        public async Task<ParametroSistema> Handle(ObterParametroSistemaPorTipoEAnoQuery request, CancellationToken cancellationToken)
            => await repositorioParametroSistema.ObterParametroPorTipoEAno(request.TipoParametroSistema, request.Ano);
    }
}
