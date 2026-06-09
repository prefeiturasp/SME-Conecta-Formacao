using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaGrupoPeriodo
{
    public class SalvarPropostaGrupoPeriodoCommandHandler(
        IRepositorioPropostaGrupoPeriodo repositorioPropostaGrupoPeriodo,
        IRepositorioProposta repositorioProposta)
        : IRequestHandler<SalvarPropostaGrupoPeriodoCommand, Resultado>
    {
        public async Task<Resultado> Handle(SalvarPropostaGrupoPeriodoCommand request, CancellationToken cancellationToken)
        {
            if (!ValidarPeriodos(request.PropostaDto))
                return Erro.Validacao("Os períodos informados não são válidos.");

            var turmasDaProposta = await repositorioProposta.ObterTurmasPorId(request.PropostaId);
            var dicionarioTurmasValidas = turmasDaProposta.ToDictionary(t => t.Id, t => t.Nome);
            var validacaoTurmas = ValidarTurmas(request.PropostaDto, dicionarioTurmasValidas);
            if (!validacaoTurmas.Sucesso)
                return validacaoTurmas;

            var gruposDto = request.PropostaDto.GruposPeriodos;
            var gruposDesejados = gruposDto.ToList();
            var gruposAtuais = await repositorioPropostaGrupoPeriodo.ObterPorPropostaIdAsync(request.PropostaId);

            var idsDesejados = gruposDesejados.Where(g => g.Id > 0).Select(g => g.Id).ToHashSet();
            var gruposParaRemover = gruposAtuais.Where(g => !idsDesejados.Contains(g.Id));

            var gruposParaInserir = gruposDto.Where(g => g.Id == 0);
            var gruposParaAtualizar = gruposDto.Where(g => g.Id > 0);

            await RemoverGruposAsync(gruposParaRemover);
            await InserirGruposAsync(gruposParaInserir, request.PropostaId);
            await AtualizarGruposAsync(gruposParaAtualizar, gruposAtuais);
            return Resultado.DeSucesso();

        }

        private static bool ValidarPeriodos(PropostaDTO propostaDto) =>
            propostaDto.GruposPeriodos.All(gp => gp.DataInicio <= gp.DataFim &&
            gp.DataInicio <= propostaDto.DataRealizacaoFim && gp.DataInicio >= propostaDto.DataRealizacaoInicio &&
            gp.DataFim >= propostaDto.DataRealizacaoInicio && gp.DataFim <= propostaDto.DataRealizacaoFim);

        private static Resultado ValidarTurmas(PropostaDTO propostaDto, Dictionary<long, string> dicionarioTurmasValidas)
        {
            if (propostaDto.GruposPeriodos is null) return Resultado.DeSucesso();

            var erroEncontrado = propostaDto.GruposPeriodos
                .Select((gp, index) => ValidarGrupo(gp, index + 1, dicionarioTurmasValidas))
                .FirstOrDefault(resultado => !resultado.Sucesso);

            return erroEncontrado ?? Resultado.DeSucesso();
        }

        private static Resultado ValidarGrupo(PropostaGrupoPeriodoDto gp, int posicao, Dictionary<long, string> dicionarioTurmasValidas)
        {
            var identificacaoGrupo = $"posição {posicao}";

            if (gp.PropostaTurmasIds is null || !gp.PropostaTurmasIds.Any())
                return Erro.Validacao($"O grupo de período na {identificacaoGrupo} deve conter pelo menos uma turma vinculada.");

            var hashSetIds = new HashSet<long>();

            foreach (var turmaId in gp.PropostaTurmasIds)
            {
                if (turmaId <= 0)
                    return Erro.Validacao($"Foi informada uma turma com identificador inválido no grupo {identificacaoGrupo}.");

                if (!dicionarioTurmasValidas.TryGetValue(turmaId, out var nomeTurma)) 
                    return Erro.Validacao($"Uma turma não reconhecida (Código interno: {turmaId}) foi informada no grupo {identificacaoGrupo}. Certifique-se de que ela pertence a esta proposta.");

                if (!hashSetIds.Add(turmaId))
                    return Erro.Validacao($"A turma '{nomeTurma}' foi inserida mais de uma vez no grupo {identificacaoGrupo}.");                 
            }

            return Resultado.DeSucesso();
        }

        private async Task InserirGruposAsync(IEnumerable<PropostaGrupoPeriodoDto> gruposDto, long propostaId)
        {
            foreach (var item in gruposDto)
            {
                var entidade = new PropostaGrupoPeriodo
                {
                    PropostaId = propostaId,
                    DataInicio = item.DataInicio,
                    DataFim = item.DataFim
                };

                foreach (var turmaId in item.PropostaTurmasIds)
                {
                    entidade.AdicionarTurma(turmaId);
                }

                item.Id = await repositorioPropostaGrupoPeriodo.Inserir(entidade);
            }
        }

        private async Task AtualizarGruposAsync(IEnumerable<PropostaGrupoPeriodoDto> gruposDto, IEnumerable<PropostaGrupoPeriodo> gruposAtuais)
        {
            foreach (var item in gruposDto)
            {
                var entidade = gruposAtuais.FirstOrDefault(g => g.Id == item.Id);
                if (entidade is null) continue;
                entidade.DataInicio = item.DataInicio;
                entidade.DataFim = item.DataFim;

                entidade.SincronizarTurmas(item.PropostaTurmasIds);
                await repositorioPropostaGrupoPeriodo.Atualizar(entidade);
            }
        }

        private async Task RemoverGruposAsync(IEnumerable<PropostaGrupoPeriodo> gruposParaRemover)
        {
            foreach (var entidade in gruposParaRemover)
            {
                entidade.Excluir();
                await repositorioPropostaGrupoPeriodo.Atualizar(entidade);
            }
        }
    }
}