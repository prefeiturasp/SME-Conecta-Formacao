using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCoordenadoria : IRepositorioBaseAuditavel<Coordenadoria>
    {
        Task<Coordenadoria?> ObterComAreaPromotoraAsync(long id);
        Task<ResultadoPaginado<Coordenadoria>> ObterCoordenadoriaPaginadoAsync(string? nome, string? sigla, int pagina, int tamanhoPagina);
    }
}
