using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class PropostaEncontro : EntidadeBaseAuditavel
    {
        public long PropostaId { get; set; }
        public string? HoraInicio { get; set; }
        public string? HoraFim { get; set; }
        public TipoEncontro? Tipo { get; set; }
        public string Local { get; set; } = null!;

        public IEnumerable<PropostaEncontroTurma> Turmas { get; set; } = [];
        public IEnumerable<PropostaEncontroData> Datas { get; set; } = [];

        public bool HouveAlteracao(PropostaEncontro outraEntidade)
        {
            return HoraInicio != outraEntidade.HoraInicio ||
                   HoraFim != outraEntidade.HoraFim ||
                   Tipo != outraEntidade.Tipo ||
                   Local != outraEntidade.Local;
        }

        /// <summary>
        /// Indica se o encontro possui horários na raiz (padrão legado) em vez de nas datas.
        /// </summary>
        /// <remarks>
        /// MÉTODO PROVISÓRIO (Retrocompatibilidade): Criado para suportar a alteração migração dos dados de março de 2026 para baixo, 
        /// onde os horários ficavam no Encontro e não nas Datas.
        /// US (144347) - Sprint 009 - 2026/03/25 - Equipe de Desenvolvimento
        /// </remarks>
        public bool PossuiHorarioLegado => !string.IsNullOrWhiteSpace(HoraInicio) ||
                                           !string.IsNullOrWhiteSpace(HoraFim);

        /// <summary>
        /// Preenche os horários das datas filhas com base no horário do encontro.
        /// </summary>
        /// <remarks>
        /// MÉTODO PROVISÓRIO (Retrocompatibilidade): Criado para suportar a alteração migração dos dados de março de 2026 para baixo, 
        /// onde os horários ficavam no Encontro e não nas Datas.
        /// US (144347) - Sprint 009 - 2026/03/25 - Equipe de Desenvolvimento
        /// </remarks>
        public void MigrarHorariosLegadoParaDatas()
        {
            if (!PossuiHorarioLegado)
                return;

            if (Datas != null && Datas.Any())
            {
                var datasParaPreencher = Datas.Where(w => string.IsNullOrWhiteSpace(w.HoraInicio) || string.IsNullOrWhiteSpace(w.HoraFim));
                foreach (var data in datasParaPreencher)
                {
                    data.HoraInicio = HoraInicio;
                    data.HoraFim = HoraFim;
                }
            }

            HoraInicio = null;
            HoraFim = null;
        }
    }
}
