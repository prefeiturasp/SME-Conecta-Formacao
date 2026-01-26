using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafLogRemessaConclusao
    {
        Task InserirAsync(CodafLogRemessaConclusao codafLogRemessaConclusao);
    }
}
