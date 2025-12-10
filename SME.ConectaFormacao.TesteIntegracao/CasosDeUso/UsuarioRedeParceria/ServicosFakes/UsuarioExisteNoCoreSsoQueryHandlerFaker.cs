using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuariosRedeParceria.ServicosFakes
{
    public class UsuarioExisteNoCoreSsoQueryHandlerFaker : IRequestHandler<UsuarioExisteNoCoreSsoQuery, bool>
    {
        public Task<bool> Handle(UsuarioExisteNoCoreSsoQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
