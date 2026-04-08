using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Infra.Dados.Dtos;

namespace SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces
{
    public interface IRepositorioPropostaEncontro : IRepositorioBaseAuditavel<PropostaEncontro>
    {
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorPropostaAsync(long propostaId);
        Task<ResultadoPaginado<PropostaEncontro>> ObterEncontrosPorPropostaAsync(long propostaId, int numeroPagina, int numeroRegistros);
        Task<IEnumerable<PropostaEncontroData>> ObterEncontroDatasPorEncontroIdAsync(params long[] encontroId);
        Task<IEnumerable<PropostaEncontroTurma>> ObterEncontroTurmasPorEncontroIdAsync(params long[] encontroId);
        Task<PropostaEncontro?> ObterEncontroPorIdAsync(long encontroId); 
        Task<IEnumerable<PropostaEncontro>> ObterEncontrosPorPropostaTurmaAsync(long turmaId);
        Task<int> ObterQuantidadeDeTurmasComEncontroAsync(long propostaId);
        Task<int> ObterTotalEncontrosAsync(long propostaId);
        Task InserirEncontroAsync(long propostaId, PropostaEncontro encontro);
        Task InserirEncontroTurmasAsync(long propostaEncontroId, IEnumerable<PropostaEncontroTurma> turmas);
        Task InserirEncontroDatasAsync(long propostaEncontroId, IEnumerable<PropostaEncontroData> datas);
        Task RemoverEncontrosAsync(IEnumerable<PropostaEncontro> encontros);
        Task AtualizarEncontroAsync(PropostaEncontro encontro);
        Task RemoverEncontroTurmasAsync(IEnumerable<PropostaEncontroTurma> turmas);
        Task RemoverEncontroDatasAsync(IEnumerable<PropostaEncontroData> datas);
        Task AtualizarEncontroDataAsync(PropostaEncontroData data);
    }
}
