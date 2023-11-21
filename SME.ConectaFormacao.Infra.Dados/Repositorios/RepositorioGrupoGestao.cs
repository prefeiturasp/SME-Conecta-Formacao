using Dapper;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public class RepositorioGrupoGestao : RepositorioBase<GrupoGestao>, IRepositorioGrupoGestao
    {
        public RepositorioGrupoGestao(IContextoAplicacao contexto, IConectaFormacaoConexao conexao) : base(contexto, conexao)
        { }
    }
}