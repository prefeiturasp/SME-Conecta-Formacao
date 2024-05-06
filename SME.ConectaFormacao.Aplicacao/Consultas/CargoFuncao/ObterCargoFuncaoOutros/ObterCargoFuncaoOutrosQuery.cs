using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterCargoFuncaoOutrosQuery : IRequest<CargoFuncao>
    {
        private static ObterCargoFuncaoOutrosQuery? _instancia;

        public static ObterCargoFuncaoOutrosQuery Instancia() => _instancia ??= new();
    }
}
