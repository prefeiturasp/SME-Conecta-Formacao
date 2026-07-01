using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Aplicacao.Interfaces.Inscricoes
{
    public interface ICasoDeUsoPesquisarCursistaPorPropostaTurmaId
    {
        Task<Resultado<PaginacaoResultadoDto<DadosInscricaoCursistaRetornoDto>>> ExecutarAsync(long propostaTurmaId, string termoBusca, int numeroPagina = 1, int numeroRegistros = 10);
    }
}
