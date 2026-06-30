using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using System.Data;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios
{
    public interface IRepositorioLog : IRepositorioBase<Log>
    {
        Task<long> Inserir(IDbTransaction transacao, Log log);     
    }
}
