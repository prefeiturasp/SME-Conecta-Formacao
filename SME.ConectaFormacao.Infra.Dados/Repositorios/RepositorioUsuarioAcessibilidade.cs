using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    [ExcludeFromCodeCoverage]
    public class RepositorioUsuarioAcessibilidade(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) :
        RepositorioBaseAuditavel<UsuarioAcessibilidade>(contexto, conexao), IRepositorioUsuarioAcessibilidade
    {
        public async Task<UsuarioAcessibilidade?> ObterPorUsuarioIdAsync(long usuarioId)
        {
            return await ObterPorExpressaoAsync(ua => ua.UsuarioId == usuarioId && !ua.Excluido);
        }
    }
}
