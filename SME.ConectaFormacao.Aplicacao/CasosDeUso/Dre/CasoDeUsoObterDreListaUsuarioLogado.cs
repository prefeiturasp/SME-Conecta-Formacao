using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Dre;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Dre
{
    public class CasoDeUsoObterDreListaUsuarioLogado(IRepositorioAreaPromotora repositorio, IContextoAplicacao contextoAplicacao) : ICasoDeUsoObterDreListaUsuarioLogado
    {
        public async Task<IEnumerable<RetornoListagemDTO>> ExecutarAsync()
        {
            if (contextoAplicacao.IdPerfilUsuario == null)
                throw new NegocioException("Usuário não possui perfil de acesso.");

            var dres = await repositorio.ObterDresPorGrupoIdAsync(contextoAplicacao.IdPerfilUsuario.Value);
            return dres.Select(dre => new RetornoListagemDTO
            {
                Id = dre.Id,
                Descricao = dre.Nome
            });
        }
    }
}