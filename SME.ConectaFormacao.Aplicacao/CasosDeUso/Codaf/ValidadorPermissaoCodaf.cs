using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class ValidadorPermissaoCodaf(
        IRepositorioCodafListaPresenca repositorioCodafListaPresenca,
        IContextoAplicacao contextoAplicacao) : IValidadorPermissaoCodaf
    {
        private readonly IRepositorioCodafListaPresenca _repositorioCodafListaPresenca = repositorioCodafListaPresenca ??
                throw new ArgumentNullException(nameof(repositorioCodafListaPresenca));      
        private readonly IContextoAplicacao _contextoAplicacao = contextoAplicacao ?? throw new ArgumentNullException(nameof(contextoAplicacao));

        public async Task<bool> ValidarSeUsuarioEhCriador(Usuario usuarioLogado, long codafListaPresencaId)
        {
            var codaf = await _repositorioCodafListaPresenca.ObterPorIdDetalhadoAsync(codafListaPresencaId);
            
            if (codaf == null)
                return false;

            return codaf.CriadoLogin == usuarioLogado.Login;
        }

        public async Task<bool> UsuarioPossuiPerfilAdminOuEMFORPEF(Guid usuarioPerfil)
        {
            if (usuarioPerfil == Guid.Empty)
                return false;

            var temPerfilAdmin = usuarioPerfil == Perfis.ADMIN_DF || usuarioPerfil == Perfis.EMFORPEF;  

            return await Task.FromResult(temPerfilAdmin);
        }

        public Task<Guid> BuscarPerfilUsuario()
        {
            var usuarioPerfil = _contextoAplicacao.IdPerfilUsuario ?? throw new NegocioException(
                    "Não foi possível identificar os perfis do usuário logado. Por favor, faça login novamente.");

            return Task.FromResult(usuarioPerfil);
        }
    }
}