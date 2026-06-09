namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaGrupoPeriodo : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        private readonly List<PropostaGrupoPeriodoTurma> _turmasVinculadas = [];
        public IReadOnlyCollection<PropostaGrupoPeriodoTurma> TurmasVinculadas => _turmasVinculadas.AsReadOnly();

        public void AdicionarTurma(long propostaTurmaId)
        {
            var vinculoExistente = _turmasVinculadas.FirstOrDefault(t => t.PropostaTurmaId == propostaTurmaId);
            if (vinculoExistente != null)
            {
                if (vinculoExistente.Excluido)
                    vinculoExistente.Excluido = false;
            }
            else
                _turmasVinculadas.Add(new() { GrupoPeriodoId = Id, PropostaTurmaId = propostaTurmaId });
        }

        public void Excluir()
        {
            Excluido = true;
            ExcluirTodasAsTurmas();
        }

        private void ExcluirTodasAsTurmas()
        {
            foreach (var turma in _turmasVinculadas)
            {
                turma.Excluido = true;
            }
        }

        public void SincronizarTurmas(IEnumerable<long> propostaTurmasIds)
        {
            var idsDesejados = propostaTurmasIds?.ToList() ?? [];
            var turmasParaRemover = _turmasVinculadas.Where(t => !t.Excluido && !idsDesejados.Contains(t.PropostaTurmaId)).ToList();
            foreach (var turmaId in idsDesejados)
            {
                AdicionarTurma(turmaId);
            }
            foreach (var turma in turmasParaRemover)
            {
                turma.Excluido = true;
            }
        }
    }
}