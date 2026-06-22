using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Interfaces.Codaf;
using SME.ConectaFormacao.Aplicacao.Servicos.Interfaces;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf
{
    public class CasoDeUsoObterPropostaTurmaComCodaf(
        IRepositorioProposta repositorioProposta,
        IServicoPeriodoEncontroProposta servicoPeriodoEncontroProposta) :
        ICasoDeUsoObterPropostaTurmaComCodaf
    {
        public async Task<Resultado<IEnumerable<PropostaTurmaComCodafDto>>> ExecutarAsync(long propostaId)
        {
            var turmas = await repositorioProposta.ObterTurmasComCodafAsync(propostaId);
            var turmasComCodaf = new List<PropostaTurmaComCodafDto>();

            foreach (var turma in turmas)
            {
                turmasComCodaf.Add(new()
                {
                    Id = turma.Id,
                    Descricao = turma.Nome + await servicoPeriodoEncontroProposta.ObterPeriodoEncontrosTurmaAsync(turma.Id),
                    CodafId = turma.CodafListaPresenca?.Id ?? 0
                });
            }
            return turmasComCodaf;
        }
    }
}
