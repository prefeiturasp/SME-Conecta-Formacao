using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarUsuarioParcialCommandHandler : IRequestHandler<SalvarUsuarioParcialCommand, bool>
    {
        private readonly IRepositorioUsuario _repositorioUsuario;

        public SalvarUsuarioParcialCommandHandler(IRepositorioUsuario repositorioUsuario)
        {
            _repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
        }

        public async Task<bool> Handle(SalvarUsuarioParcialCommand request, CancellationToken cancellationToken)
        {
            if (request.Nome.NaoEstaPreenchido())
                throw new NegocioException(MensagemNegocio.NOME_USUARIO_NAO_PREENCHIDO);
            
            var usuario = await _repositorioUsuario.ObterPorLogin(request.Login);

            if (usuario.EhNulo())
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
            
            usuario.Nome = request.Nome;
            
            return (await _repositorioUsuario.Atualizar(usuario)).Id > 0;
        }
    }
}
