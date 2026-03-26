using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.PropostaEncontros;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaEncontroPaginacao(
        IMediator mediator,
        IContextoAplicacao contextoAplicacao,
        IRepositorioPropostaEncontro repositorioPropostaEncontro) :
        CasoDeUsoAbstratoPaginado(mediator, contextoAplicacao), ICasoDeUsoObterPropostaEncontroPaginacao
    {
        public async Task<PaginacaoResultadoDto<CronogramaEncontroDto>> ExecutarAsync(long id)
        {
            if (id == 0) return new PaginacaoResultadoDto<CronogramaEncontroDto>([], 0, 0);

            var encontros = await repositorioPropostaEncontro.ObterEncontrosPorPropostaAsync(id, NumeroPagina, NumeroRegistros);

            if (encontros.Itens == null || !encontros.Itens.Any())
                return new PaginacaoResultadoDto<CronogramaEncontroDto>([], encontros.TotalRegistros, encontros.TamanhoPagina);

            var cronogramaEncontros = encontros.Itens.Select(e => new CronogramaEncontroDto
            {
                Id = e.Id,
                QuantidadeDiasEncontro = e.Datas.Count(),
                Tipo = e.Tipo,
                Local = e.Local,
                Turmas = [.. e.Turmas.Select(t => new PropostaEncontroTurmaDto { TurmaId = t.TurmaId, Nome = t.Turma.Nome })],
                CronogramaDatas = ExpandirDatasEncontro(e)
            });

            return new PaginacaoResultadoDto<CronogramaEncontroDto>(cronogramaEncontros, encontros.TotalRegistros, encontros.TamanhoPagina);
        }

        private static List<CronogramaDataEncontroDto> ExpandirDatasEncontro(PropostaEncontro encontro)
        {
            encontro.MigrarHorariosLegadoParaDatas();

            var datas = encontro.Datas;
            if (datas == null || !datas.Any())
                return [];
            var cronogramaDatas = new List<CronogramaDataEncontroDto>();

            foreach (var data in datas)
            {
                if (data.SemDataFim())
                {
                    cronogramaDatas.Add(new (data.Id, data.DataInicio!.Value.Date, data.HoraInicio, data.HoraFim));
                    continue;
                }

                var dataInicio = data.DataInicio!.Value.Date;
                var dataFim = data.DataFim!.Value.Date;
                for (var dataAtual = dataInicio; dataAtual <= dataFim; dataAtual = dataAtual.AddDays(1))
                {
                    if (dataAtual.DayOfWeek == DayOfWeek.Saturday || dataAtual.DayOfWeek == DayOfWeek.Sunday)
                        continue;
                    cronogramaDatas.Add(new (data.Id, dataAtual, data.HoraInicio, data.HoraFim));
                }
            }
            return [.. cronogramaDatas.Distinct().OrderBy(d => d.Data)];
        }
    }
}
