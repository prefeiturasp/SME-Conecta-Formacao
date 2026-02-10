using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafComentarioListaPresenca : IRepositorioBaseAuditavel<CodafComentarioListaPresenca>
    {
        Task<CodafComentarioDevolucaoDto?> ObterUltimoComentarioDevolucaoPorUsuarioAsync(long codafListaPresencaId,
        StatusCodafListaPresenca statusDevolucao,
        StatusCodafListaPresenca statusEnvio);
    }
}
