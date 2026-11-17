using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodoInscricao
{
    public class SalvarPropostaGrupoPeriodoInscricaoCommandHandler(
        IRepositorioPropostaGrupoPeriodoInscricao repositorioPropostaGrupoPeriodoInscricao,
        IRepositorioProposta repositorioProposta)
        : IRequestHandler<SalvarPropostaGrupoPeriodoInscricaoCommand, Resultado>
    {
        public async Task<Resultado> Handle(SalvarPropostaGrupoPeriodoInscricaoCommand request, CancellationToken cancellationToken)
        {
            var gruposDto = request.PropostaDto.GruposPeriodosInscricao ?? [];

            AjustarDatasGruposInscricao(request.PropostaDto);

            if (!ValidarPeriodos(request.PropostaDto))
                return Erro.Validacao("Os períodos informados não são válidos.");

            var turmasDaProposta = await repositorioProposta.ObterTurmasPorId(request.PropostaId);
            var dicionarioTurmasValidas = turmasDaProposta.ToDictionary(t => t.Id, t => t.Nome);
            var validacaoTurmas = ValidarTurmas(request.PropostaDto, dicionarioTurmasValidas);
            if (!validacaoTurmas.Sucesso)
                return validacaoTurmas;

            var gruposDesejados = gruposDto.ToList();
            var gruposAtuais = await repositorioPropostaGrupoPeriodoInscricao.ObterPorPropostaIdAsync(request.PropostaId);

            var idsDesejados = gruposDesejados.Where(g => g.Id > 0).Select(g => g.Id).ToHashSet();
            var gruposParaRemover = gruposAtuais.Where(g => !idsDesejados.Contains(g.Id));

            var gruposParaInserir = gruposDesejados.Where(g => g.Id == 0);
            var gruposParaAtualizar = gruposDesejados.Where(g => g.Id > 0);

            await RemoverGruposAsync(gruposParaRemover);
            await InserirGruposAsync(gruposParaInserir, request.PropostaId);
            await AtualizarGruposAsync(gruposParaAtualizar, gruposAtuais);
            return Resultado.DeSucesso();
        }

        private static DateTime? ObterDataInscricaoFimEfetiva(DateTime? dataInscricaoFim)
        {
            if (!dataInscricaoFim.HasValue)
                return null;

            if (dataInscricaoFim.Value.TimeOfDay == TimeSpan.Zero)
                return dataInscricaoFim.Value.Date.AddDays(1).AddMilliseconds(-1);

            return dataInscricaoFim.Value;
        }

        private static void AjustarDatasGruposInscricao(PropostaDTO propostaDto)
        {
            if (propostaDto.GruposPeriodosInscricao is null || !propostaDto.GruposPeriodosInscricao.Any())
                return;

            var dataInscricaoFimEfetivo = ObterDataInscricaoFimEfetiva(propostaDto.DataInscricaoFim);
            var horaInicioGeral = propostaDto.DataInscricaoInicio?.TimeOfDay ?? TimeSpan.Zero;
            var horaFimGeral = dataInscricaoFimEfetivo?.TimeOfDay ?? new TimeSpan(23, 59, 59);

            foreach (var gp in propostaDto.GruposPeriodosInscricao)
            {
                gp.DataInicio = gp.DataInicio.Date.Add(horaInicioGeral);
                gp.DataFim = gp.DataFim.Date.Add(horaFimGeral);

                if (gp.DataInicio.Date == gp.DataFim.Date && gp.DataFim < gp.DataInicio)
                {
                    gp.DataFim = gp.DataInicio;
                }
            }
        }

        private static bool ValidarPeriodos(PropostaDTO propostaDto)
        {
            if (propostaDto.GruposPeriodosInscricao is null || !propostaDto.GruposPeriodosInscricao.Any())
                return true;

            var dataInscricaoInicio = propostaDto.DataInscricaoInicio;
            var dataInscricaoFim = ObterDataInscricaoFimEfetiva(propostaDto.DataInscricaoFim);

            return propostaDto.GruposPeriodosInscricao.All(gp =>
                gp.DataInicio <= gp.DataFim &&
                (!dataInscricaoInicio.HasValue || (gp.DataInicio >= dataInscricaoInicio.Value && gp.DataFim >= dataInscricaoInicio.Value)) &&
                (!dataInscricaoFim.HasValue || ((gp.DataInicio <= dataInscricaoFim.Value && gp.DataFim <= dataInscricaoFim.Value) || gp.DataFim == gp.DataInicio)));
        }

        private static Resultado ValidarTurmas(PropostaDTO propostaDto, Dictionary<long, string> dicionarioTurmasValidas)
        {
            if (propostaDto.GruposPeriodosInscricao is null || !propostaDto.GruposPeriodosInscricao.Any())
                return Resultado.DeSucesso();

            var todasTurmasUsadas = new HashSet<long>();
            var grupos = propostaDto.GruposPeriodosInscricao.ToList();

            for (var i = 0; i < grupos.Count; i++)
            {
                var resultadoGrupo = ValidarTurmasDoGrupo(grupos[i], i + 1, dicionarioTurmasValidas, todasTurmasUsadas);
                if (!resultadoGrupo.Sucesso)
                    return resultadoGrupo;
            }

            return Resultado.DeSucesso();
        }

        private static Resultado ValidarTurmasDoGrupo(
            PropostaGrupoPeriodoInscricaoDto gp,
            int posicao,
            Dictionary<long, string> dicionarioTurmasValidas,
            HashSet<long> todasTurmasUsadas)
        {
            var identificacaoGrupo = $"posição {posicao}";

            if (gp.PropostaTurmasIds is null || !gp.PropostaTurmasIds.Any())
                return Erro.Validacao($"O grupo de período na {identificacaoGrupo} deve conter pelo menos uma turma vinculada.");

            var turmasDoGrupo = new HashSet<long>();

            foreach (var turmaId in gp.PropostaTurmasIds)
            {
                var resultadoTurma = ValidarTurmaIndividual(turmaId, identificacaoGrupo, dicionarioTurmasValidas, turmasDoGrupo, todasTurmasUsadas);
                if (!resultadoTurma.Sucesso)
                    return resultadoTurma;
            }

            return Resultado.DeSucesso();
        }

        private static Resultado ValidarTurmaIndividual(
            long turmaId,
            string identificacaoGrupo,
            Dictionary<long, string> dicionarioTurmasValidas,
            HashSet<long> turmasDoGrupo,
            HashSet<long> todasTurmasUsadas)
        {
            if (turmaId <= 0)
                return Erro.Validacao($"Foi informada uma turma com identificador inválido no grupo {identificacaoGrupo}.");

            if (!dicionarioTurmasValidas.TryGetValue(turmaId, out var nomeTurma))
                return Erro.Validacao($"Uma turma não reconhecida (Código interno: {turmaId}) foi informada no grupo {identificacaoGrupo}. Certifique-se de que ela pertence a esta proposta.");

            if (!turmasDoGrupo.Add(turmaId))
                return Erro.Validacao($"A turma '{nomeTurma}' foi inserida mais de uma vez no grupo {identificacaoGrupo}.");

            if (!todasTurmasUsadas.Add(turmaId))
                return Erro.Validacao($"A turma '{nomeTurma}' já está vinculada a outro período de inscrição customizado.");

            return Resultado.DeSucesso();
        }

        private async Task InserirGruposAsync(IEnumerable<PropostaGrupoPeriodoInscricaoDto> gruposDto, long propostaId)
        {
            foreach (var item in gruposDto)
            {
                var entidade = new PropostaGrupoPeriodoInscricao
                {
                    PropostaId = propostaId,
                    DataInicio = item.DataInicio,
                    DataFim = item.DataFim
                };

                foreach (var turmaId in item.PropostaTurmasIds)
                {
                    entidade.AdicionarTurma(turmaId);
                }

                item.Id = await repositorioPropostaGrupoPeriodoInscricao.Inserir(entidade);
            }
        }

        private async Task AtualizarGruposAsync(
            IEnumerable<PropostaGrupoPeriodoInscricaoDto> gruposDto,
            IEnumerable<PropostaGrupoPeriodoInscricao> gruposAtuais)
        {
            foreach (var item in gruposDto)
            {
                var entidade = gruposAtuais.FirstOrDefault(g => g.Id == item.Id);
                if (entidade is null) continue;
                entidade.DataInicio = item.DataInicio;
                entidade.DataFim = item.DataFim;

                entidade.SincronizarTurmas(item.PropostaTurmasIds);
                await repositorioPropostaGrupoPeriodoInscricao.Atualizar(entidade);
            }
        }

        private async Task RemoverGruposAsync(IEnumerable<PropostaGrupoPeriodoInscricao> gruposParaRemover)
        {
            foreach (var entidade in gruposParaRemover)
            {
                entidade.Excluir();
                await repositorioPropostaGrupoPeriodoInscricao.Atualizar(entidade);
            }
        }
    }
}
