using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoPorCodigoEolQuery : IRequest<IEnumerable<CargoFuncao>>
    {
        public ObterCargoFuncaoPorCodigoEolQuery(IEnumerable<long> codigosCargosEol, IEnumerable<long> codigosFuncoesEol)
        {
            CodigosCargosEol = codigosCargosEol;
            CodigosFuncoesEol = codigosFuncoesEol;
        }

        public IEnumerable<long> CodigosCargosEol { get; }
        public IEnumerable<long> CodigosFuncoesEol { get; }
    }
}
