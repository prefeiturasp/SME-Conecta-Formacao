using System.Collections.Generic;
using System.Linq;
using System.Text;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;

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

            var turmas = dados.CronogramaTurmas.Select(t => {
                if (string.IsNullOrEmpty(t.Local)) t.Local = "A DEFINIR";
                return t;
            }).ToList();

            var locaisAgrupados = turmas
                .GroupBy(e => e.Local)
                .OrderBy(g => g.Key);

            if (locaisAgrupados.Count() > 1)
                descricao.AppendLine("<hr>");

            var listaDescricaoCronogramaLocais = new List<string>();

            foreach (var localEncontro in locaisAgrupados)
            {
                var descricaoCronogramaLocais = new StringBuilder();
                var turmasAgrupadas = localEncontro
                    .GroupBy(e => e.Identificacao)
                    .OrderBy(g => g.Key);

                foreach (var turma in turmasAgrupadas)
                {
                    descricaoCronogramaLocais.Append($"<p><strong>{turma.Key.ToUpper()}:</strong> ");

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
                            descricaoCronogramaLocais.Append("<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");

                        descricaoCronogramaLocais.Append(linha);
                        primeiraLinha = false;
                    }
                    descricaoCronogramaLocais.Append("</p>");
                }
                var local = localEncontro.Key;
                if (!string.IsNullOrEmpty(local))
                {
                    descricaoCronogramaLocais.AppendLine($"<p>LOCAL: {local.ToUpper()}</p>");
                }
                listaDescricaoCronogramaLocais.Add(descricaoCronogramaLocais.ToString());
            }
            descricao.AppendLine(string.Join("<hr>", listaDescricaoCronogramaLocais));

            return descricao.ToString();
        }
    }
}
