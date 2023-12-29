using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoPorCodigoEolQueryHandler : IRequestHandler<ObterCargoFuncaoPorCodigoEolQuery, IEnumerable<Dominio.Entidades.CargoFuncao>>
    {
        private readonly IRepositorioCargoFuncao _repositorioCargoFuncao;

        public ObterCargoFuncaoPorCodigoEolQueryHandler(IRepositorioCargoFuncao repositorioCargoFuncao)
        {
            _repositorioCargoFuncao = repositorioCargoFuncao ?? throw new ArgumentNullException(nameof(repositorioCargoFuncao));
        }

        public async Task<IEnumerable<Dominio.Entidades.CargoFuncao>> Handle(ObterCargoFuncaoPorCodigoEolQuery request, CancellationToken cancellationToken)
        {
            return await _repositorioCargoFuncao.ObterPorCodigoEol(
                request.CodigosCargosEol.ToArray(),
                request.CodigosFuncoesEol.ToArray());
        }
    }
}
