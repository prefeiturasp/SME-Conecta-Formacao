using MediatR;
using SME.ConectaFormacao.Aplicacao;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuariosRedeParceria.ServicosFakes
{
    internal class CadastrarUsuarioServicoAcessoCommandHandlerFaker : IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>
    {
        public Task<bool> Handle(CadastrarUsuarioServicoAcessoCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
