using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes
{
    public class AtualizarUsuarioServicoAcessoCommandHandlerFaker : IRequestHandler<AtualizarUsuarioServicoAcessoCommand, bool>
    {
        public Task<bool> Handle(AtualizarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
