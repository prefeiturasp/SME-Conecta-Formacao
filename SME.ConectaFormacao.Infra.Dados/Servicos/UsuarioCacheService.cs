
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class UsuarioCacheService(
        IContextoAplicacao contextoAplicacao,
        ICacheDistribuido cacheDistribuido,
        IRepositorioUsuario repositorioUsuario) : IUsuarioCacheService
    {
        public async Task AtualizarTelefoneEInvalidarCacheAsync(Usuario usuario, string telefone)
        {
            usuario.Telefone = telefone;
            await repositorioUsuario.AtualizarTelefone(usuario.Id, telefone);
            await cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.Usuario.Parametros(contextoAplicacao.UsuarioLogado));
            await cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.UsuarioLogado.Parametros(contextoAplicacao.UsuarioLogado));
        }
    }
}
