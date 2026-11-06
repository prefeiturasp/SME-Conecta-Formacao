namespace SME.ConectaFormacao.Dominio.Entidades
{
    public abstract class PropostaGrupoPeriodoBase<TTurmaVinculo> : EntidadeBaseAuditavel
        where TTurmaVinculo : PropostaGrupoPeriodoTurmaBase
    {
        public long PropostaId { get; set; }
        public string? Descricao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        protected readonly List<TTurmaVinculo> _turmasVinculadas = [];
        public IReadOnlyCollection<TTurmaVinculo> TurmasVinculadas => _turmasVinculadas.AsReadOnly();

        protected abstract TTurmaVinculo CriarVinculoTurma(long propostaTurmaId);

        public void AdicionarTurma(long propostaTurmaId)
        {
            var vinculoExistente = _turmasVinculadas.FirstOrDefault(t => t.PropostaTurmaId == propostaTurmaId);
            if (vinculoExistente != null)
            {
                if (vinculoExistente.Excluido)
                    vinculoExistente.Excluido = false;
            }
            else
            {
                _turmasVinculadas.Add(CriarVinculoTurma(propostaTurmaId));
            }
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
