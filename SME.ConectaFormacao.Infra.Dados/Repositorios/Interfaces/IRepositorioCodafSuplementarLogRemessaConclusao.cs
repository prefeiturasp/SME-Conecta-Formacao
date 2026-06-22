using SME.ConectaFormacao.Dominio.Entidades;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioCodafSuplementarLogRemessaConclusao
    {
        Task InserirAsync(CodafSuplementarLogRemessaConclusao codafSuplementarLogRemessaConclusao);
    }
}
