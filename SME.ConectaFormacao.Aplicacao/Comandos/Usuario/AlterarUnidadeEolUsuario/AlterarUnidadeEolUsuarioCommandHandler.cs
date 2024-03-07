using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarUnidadeEolUsuarioCommandHandler : IRequestHandler<AlterarUnidadeEolUsuarioCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public AlterarUnidadeEolUsuarioCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(AlterarUnidadeEolUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _repositorioUsuario.ObterPorLogin(request.Login) ?? 
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            usuario.CodigoEolUnidade = request.CodigoEolUnidade;
            await _repositorioUsuario.Atualizar(usuario);

            return true;
        }
    }
}
