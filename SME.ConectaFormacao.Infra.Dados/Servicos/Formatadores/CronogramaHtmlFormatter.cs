using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using System.Text;

namespace SME.ConectaFormacao.Infra.Dados.Servicos.Formatadores
{
    public static class CronogramaHtmlFormatter
    {
        public static string Formatar(PropostaLaudaCompletaDto dados)
        {
            var descricao = new StringBuilder();

            var periodoInicio = dados.DataRealizacaoInicio.HasValue ? dados.DataRealizacaoInicio.Value.ToString("dd/MM/yyyy") : "";
            var periodoFim = dados.DataRealizacaoFim.HasValue ? dados.DataRealizacaoFim.Value.ToString("dd/MM/yyyy") : "";
            descricao.AppendLine($"<p>PERÍODO DE REALIZAÇÃO: {periodoInicio} ATÉ {periodoFim}</p>");
            descricao.AppendLine("<p>DATAS E HORÁRIOS DOS ENCONTROS:</p>");

            var turmas = dados.CronogramaTurmas.Select(t =>
            {
                if (string.IsNullOrEmpty(t.Local)) t.Local = "A DEFINIR";
                return t;
            }).ToList();

            var locaisAgrupados = turmas
                .GroupBy(e => e.Local)
                .OrderBy(g => g.Key);

            if (locaisAgrupados.Count() > 1)
                descricao.AppendLine("<hr>");

            var listaDescricaoCronogramaLocais = locaisAgrupados
                .Select(ProcessarLocalEncontro)
                .ToList();

            descricao.AppendLine(string.Join("<hr>", listaDescricaoCronogramaLocais));

            return descricao.ToString();
        }

        private static string ProcessarLocalEncontro(IGrouping<string, TurmaLaudaDto> localEncontro)
        {
            var descricaoCronogramaLocais = new StringBuilder();
            var comparer = StringComparer.Create(new System.Globalization.CultureInfo("pt-BR"), System.Globalization.CompareOptions.NumericOrdering);

            var turmasAgrupadas = localEncontro
                .GroupBy(e => e.Identificacao)
                .OrderBy(g => g.Key, comparer);

            foreach (var turma in turmasAgrupadas)
            {
                ProcessarTurma(descricaoCronogramaLocais, turma);
            }

            if (!string.IsNullOrEmpty(localEncontro.Key))
            {
                descricaoCronogramaLocais.AppendLine($"<p>LOCAL: {localEncontro.Key.ToUpper()}</p>");
            }

            return descricaoCronogramaLocais.ToString();
        }

        private static void ProcessarTurma(StringBuilder builder, IGrouping<string, TurmaLaudaDto> turma)
        {
            builder.Append($"<p><strong>{turma.Key.ToUpper()}:</strong> ");

            var horariosAgrupados = turma
                .GroupBy(e => new { e.HoraInicio, e.HoraFim })
                .OrderBy(h => h.Key.HoraInicio);

            bool primeiraLinha = true;

            foreach (var grupoHorario in horariosAgrupados)
            {
                var datas = grupoHorario
                    .Select(e => e.DataInicio.HasValue ? e.DataInicio.Value.ToString("dd/MM") : "")
                    .Where(d => !string.IsNullOrEmpty(d))
                    .Distinct()
                    .OrderBy(d => d);

                var datasFormatadas = string.Join("; ", datas);
                var horarioFormatado = grupoHorario.First().HorariosFormatados;

                var linha = string.IsNullOrEmpty(horarioFormatado)
                    ? $"{datasFormatadas}"
                    : $"{datasFormatadas} - {horarioFormatado}";

                if (!primeiraLinha)
                    builder.Append("<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");

                builder.Append(linha);
                primeiraLinha = false;
            }
            builder.Append("</p>");
        }
    }
}
