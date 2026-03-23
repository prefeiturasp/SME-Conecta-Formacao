using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaEncontro
{
    public class SalvarPropostaEncontroCommandHandler(
        IMapper mapper,
        IRepositorioPropostaEncontro repositorioPropostaEncontro,
        ITransacao transacao,
        ICacheDistribuido cacheDistribuido) : IRequestHandler<SalvarPropostaEncontroCommand, long>
    {
        private readonly ITransacao _transacao = transacao;

        public async Task<long> Handle(SalvarPropostaEncontroCommand request, CancellationToken cancellationToken)
        {
            var encontroAntes = await repositorioPropostaEncontro.ObterEncontroPorIdAsync(request.EncontroDto.Id);
            var encontroDepois = mapper.Map<PropostaEncontro>(request.EncontroDto);
            encontroDepois.PropostaId = request.PropostaId;

            using var transacaoAtiva = _transacao.Iniciar();
            try
            {
                await ProcessarEncontroAsync(encontroAntes, encontroDepois);

                await ProcessarTurmasAsync(encontroDepois.Id, encontroDepois.Turmas);

                await ProcessarDatasAsync(encontroDepois.Id, encontroDepois.Datas);

                transacaoAtiva.Commit();

                await LimparCacheAsync(encontroDepois.Id, request.PropostaId);

                return encontroDepois.Id;
            }
            catch
            {
                transacaoAtiva.Rollback();
                throw;
            }
        }

        private async Task ProcessarEncontroAsync(PropostaEncontro? encontroAntes, PropostaEncontro encontroDepois)
        {
            if (encontroAntes == null)
            {
                await repositorioPropostaEncontro.InserirEncontroAsync(encontroDepois.PropostaId, encontroDepois);
                return;
            }

            if (encontroAntes.HoraInicio != encontroDepois.HoraInicio ||
                encontroAntes.HoraFim != encontroDepois.HoraFim ||
                encontroAntes.Local != encontroDepois.Local)
            {
                encontroDepois.ManterCriador(encontroAntes);
                await repositorioPropostaEncontro.AtualizarEncontroAsync(encontroDepois);
            }
        }

        private async Task ProcessarTurmasAsync(long encontroId, IEnumerable<PropostaEncontroTurma> turmasDepois)
        {
            var turmasAntes = await repositorioPropostaEncontro.ObterEncontroTurmasPorEncontroIdAsync(encontroId);

            var turmasInserir = turmasDepois.Where(w => !turmasAntes.Any(a => a.TurmaId == w.TurmaId));
            var turmasExcluir = turmasAntes.Where(w => !turmasDepois.Any(a => a.TurmaId == w.TurmaId));

            if (turmasInserir.Any())
                await repositorioPropostaEncontro.InserirEncontroTurmasAsync(encontroId, turmasInserir);

            if (turmasExcluir.Any())
                await repositorioPropostaEncontro.RemoverEncontroTurmasAsync(turmasExcluir);
        }

        private async Task ProcessarDatasAsync(long encontroId, IEnumerable<PropostaEncontroData> datasDepois)
        {
            var datasAntes = await repositorioPropostaEncontro.ObterEncontroDatasPorEncontroIdAsync(encontroId);

            var datasInserir = datasDepois.Where(w => !datasAntes.Any(a => a.Id == w.Id));
            var datasExcluir = datasAntes.Where(w => !datasDepois.Any(a => a.Id == w.Id));
            var datasAlterar = datasDepois.Where(w => datasAntes.Any(a => a.Id == w.Id));

            if (datasInserir.Any())
                await repositorioPropostaEncontro.InserirEncontroDatasAsync(encontroId, datasInserir);

            if (datasExcluir.Any())
                await repositorioPropostaEncontro.RemoverEncontroDatasAsync(datasExcluir);

            foreach (var dataAlterar in datasAlterar)
            {
                var dataAntes = datasAntes.First(t => t.Id == dataAlterar.Id);

                if (dataAlterar.DataInicio != dataAntes.DataInicio ||
                    dataAlterar.DataFim != dataAntes.DataFim)
                {
                    dataAlterar.PropostaEncontroId = encontroId;
                    dataAlterar.ManterCriador(dataAntes);
                    await repositorioPropostaEncontro.AtualizarEncontroDataAsync(dataAlterar);
                }
            }
        }

        private async Task LimparCacheAsync(long encontroId, long propostaId)
        {
            var turmasAntes = await repositorioPropostaEncontro.ObterEncontroTurmasPorEncontroIdAsync(encontroId);

            foreach (var turma in turmasAntes)
            {
                await cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.PropostaTurmaEncontro.Parametros(turma.TurmaId));
            }

            await cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.FormacaoDetalhada.Parametros(propostaId));
        }
    }
}
