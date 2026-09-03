using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Servicos.Formatadores;
using System.Security;

namespace SME.ConectaFormacao.Infra.Dados.Servicos
{
    public class GeradorLaudaDocxService : IGeradorLaudaDocxService
    {
        private const string NOME_TEMPLATE = "SME.ConectaFormacao.Infra.Dados.Templates.Template_Lauda_Completa.docx";
        private const string FONTE_PADRAO = "Courier New";
        private const string TAMANHO_FONTE_PADRAO = "21"; // 10.5pt
        private const string COR_FONTE_PADRAO = "42474A";
        private const string TEXTO_OUTROS = "OUTROS";

        public async Task<byte[]> GerarArquivoLaudaCompletaAsync(PropostaLaudaCompletaDto dados)
        {
            var assembly = typeof(GeradorLaudaDocxService).Assembly;
            using var templateStream = assembly.GetManifestResourceStream(NOME_TEMPLATE)
                ?? throw new NegocioException("O template da Lauda Completa não foi encontrado no sistema.");

            using var memoryStream = new MemoryStream();
            await templateStream.CopyToAsync(memoryStream);

            ProcessarDocumento(memoryStream, dados);

            return memoryStream.ToArray();
        }

        private static void ProcessarDocumento(MemoryStream memoryStream, PropostaLaudaCompletaDto dados)
        {
            using var wordDoc = WordprocessingDocument.Open(memoryStream, true);
            var mainPart = wordDoc.MainDocumentPart ?? throw new NegocioException("MainDocumentPart não encontrado no template.");
            var body = mainPart.Document?.Body ?? throw new NegocioException("Body não encontrado no template.");

            RemoverSecoesCondicionais(body, dados);

            var substituicoes = ObterDicionarioDeSubstituicoes(dados);
            substituicoes.Add("{{CRONOGRAMA}}", CronogramaHtmlFormatter.Formatar(dados));

            ProcessarTagsHtml(mainPart, substituicoes);
            ProcessarTagsTexto(body, substituicoes);

            mainPart.Document.Save();
        }

        private static void RemoverSecoesCondicionais(Body body, PropostaLaudaCompletaDto dados)
        {
            if (ObterMinutos(dados.CargaHorariaPresencial) <= 0) RemoverTabelaPorTag(body, "{{CH_PRESENCIAL}}");
            if (ObterMinutos(dados.CargaHorariaSincrona) <= 0) RemoverTabelaPorTag(body, "{{CH_NAO_PRESENCIAL}}");
            if (ObterMinutos(dados.CargaHorariaDistancia) <= 0) RemoverTabelaPorTag(body, "{{CH_DISTANCIA}}");
            if (string.IsNullOrWhiteSpace(dados.DescricaoAtividade)) RemoverTabelaPorTag(body, "{{ATIVIDADE_OBRIGATORIA}}");
            if (dados.VagasRemanecentes == null || !dados.VagasRemanecentes.Any()) RemoverTabelaPorTag(body, "{{VAGAS_REMANESCENTES}}");
        }

        private static void ProcessarTagsHtml(MainDocumentPart mainPart, Dictionary<string, string> substituicoes)
        {
            var tagsHtml = new[] { "{{JUSTIFICATIVA}}", "{{OBJETIVOS}}", "{{CONTEUDO_PROGRAMATICO}}", "{{PROCEDIMENTOS}}", "{{ATIVIDADE_OBRIGATÓRIA}}", "{{CRONOGRAMA}}", "{{BIBLIOGRAFIA}}" };
            foreach (var tag in tagsHtml.Where(substituicoes.ContainsKey))
            {
                if (substituicoes.TryGetValue(tag, out var valor))
                {
                    SubstituirTagPorHtml(mainPart, tag, valor);
                    substituicoes.Remove(tag);
                }
            }
        }

        private static void ProcessarTagsTexto(Body body, Dictionary<string, string> substituicoes)
        {
            string textoXmlDocumento = body.InnerXml;

            foreach (var item in substituicoes)
            {
                var valorSeguro = SecurityElement.Escape(item.Value ?? string.Empty);
                // Para quebra de linha ser válida no MS Word, ela deve sair do <w:t> atual,
                // inserir o <w:br/> no <w:r> pai, e reabrir o <w:t>
                valorSeguro = valorSeguro.Replace("&lt;br&gt;", "</w:t><w:br/><w:t>");
                textoXmlDocumento = textoXmlDocumento.Replace(item.Key, valorSeguro);
            }

            body.InnerXml = textoXmlDocumento;
        }

        private static void SubstituirTagPorHtml(MainDocumentPart mainPart, string tag, string html)
        {
            if (string.IsNullOrWhiteSpace(html)) html = string.Empty;

            var textos = mainPart.Document?.Body?.Descendants<Text>().Where(t => t.Text.Contains(tag)).ToList();
            if (textos == null || textos.Count == 0) return;

            var converter = new HtmlToOpenXml.HtmlConverter(mainPart);
            var elementosHtml = converter.Parse(html);

            foreach (var texto in textos)
            {
                var paragrafoOrigem = texto.Ancestors<Paragraph>().FirstOrDefault();
                if (paragrafoOrigem != null && paragrafoOrigem.Parent != null)
                {
                    var parent = paragrafoOrigem.Parent;
                    OpenXmlElement nodeReferencia = paragrafoOrigem;

                    foreach (var elem in elementosHtml)
                    {
                        var clone = elem.CloneNode(true);
                        AplicarFormatacaoPadrao(clone);

                        // Insere DEPOIS do parágrafo de origem, para manter a label "JUSTIFICATIVA:" no topo
                        parent.InsertAfter(clone, nodeReferencia);
                        nodeReferencia = clone;
                    }

                    texto.Text = texto.Text.Replace(tag, string.Empty);

                    if (string.IsNullOrWhiteSpace(paragrafoOrigem.InnerText))
                    {
                        paragrafoOrigem.Remove();
                    }
                }
            }
        }

        private static void AplicarFormatacaoPadrao(OpenXmlElement root)
        {
            foreach (var run in root.Descendants<Run>())
            {
                run.RunProperties ??= new RunProperties();

                run.RunProperties.RunFonts ??= new RunFonts()
                {
                    Ascii = FONTE_PADRAO,
                    HighAnsi = FONTE_PADRAO,
                    ComplexScript = FONTE_PADRAO,
                    EastAsia = FONTE_PADRAO
                };

                run.RunProperties.FontSize ??= new FontSize() { Val = TAMANHO_FONTE_PADRAO };

                run.RunProperties.FontSizeComplexScript ??= new FontSizeComplexScript() { Val = TAMANHO_FONTE_PADRAO };

                run.RunProperties.Color ??= new Color() { Val = COR_FONTE_PADRAO };
            }
        }

        private static void RemoverTabelaPorTag(Body body, string tag)
        {
            var textos = body.Descendants<Text>().Where(t => t.Text.Contains(tag)).ToList();
            foreach (var texto in textos)
            {
                var tabela = texto.Ancestors<Table>().FirstOrDefault();
                tabela?.Remove();
            }
        }

        private static string ObterInscricoes(PropostaLaudaCompletaDto dados)
        {
            var criteriosValidacao = dados.CriteriosValidacao.Select(c =>
                string.Equals(c.Nome, TEXTO_OUTROS, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dados.CriteriosValidacao_Outros)
                    ? $"{c.Nome}: {dados.CriteriosValidacao_Outros}"
                    : c.Nome).ToList();

            var periodo = $"DE {dados.DataInscricaoInicio:dd/MM/yyyy} ATÉ {dados.DataInscricaoFim:dd/MM/yyyy}";
            var linkText = string.IsNullOrEmpty(dados.LinkInscricaoExterna) ? "https://conectaformacao.sme.prefeitura.sp.gov.br/area-publica" : dados.LinkInscricaoExterna;
            var link = $"<br>PELO LINK: {linkText}";
            var criteriosStr = criteriosValidacao.Count > 0 ? $"<br>{string.Join(", ", criteriosValidacao)}" : "";

            return $"{periodo}{link}{criteriosStr}";
        }

        private static string ObterCriteriosCertificacao(PropostaLaudaCompletaDto dados)
        {
            var list = dados.CriteriosCertificacao.Select(c =>
                string.Equals(c.Nome, TEXTO_OUTROS, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dados.Criterios_Outros)
                    ? $"{c.Nome}: {dados.Criterios_Outros}"
                    : c.Nome).ToList();
            return string.Join(", ", list);
        }

        private static string ObterPublicoAlvo(PropostaLaudaCompletaDto dados)
        {
            var list = dados.PublicosAlvo.Select(c =>
                string.Equals(c.Nome, TEXTO_OUTROS, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dados.PublicoAlvo_Outros)
                    ? $"{c.Nome}: {dados.PublicoAlvo_Outros}"
                    : c.Nome).ToList();
            return string.Join(", ", list);
        }

        private static string ObterFuncaoEspecifica(PropostaLaudaCompletaDto dados)
        {
            var list = dados.FuncaoEspecifica.Select(c =>
                string.Equals(c.Nome, TEXTO_OUTROS, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(dados.FuncaoEspecifica_Outros)
                    ? $"{c.Nome}: {dados.FuncaoEspecifica_Outros}"
                    : c.Nome).ToList();
            return string.Join(", ", list);
        }

        private static string ObterCorpoDocente(PropostaLaudaCompletaDto dados)
        {
            var list = dados.Regentes.Select(r => r.ObterDescricaoCompleta()).ToList();
            return string.Join("", list);
        }

        private static Dictionary<string, string> ObterDicionarioDeSubstituicoes(PropostaLaudaCompletaDto dados)
        {
            var totalMinutos = ObterMinutos(dados.CargaHorariaPresencial) + ObterMinutos(dados.CargaHorariaDistancia) + ObterMinutos(dados.CargaHorariaSincrona);
            var cargaHorariaTotal = ObterHoraFormatada(totalMinutos);

            return new Dictionary<string, string>
            {
                { "{{NUMERO_DESPACHO}}", dados.NumeroHomologacao },
                { "{{NUMERO_PROPOSTA}}", dados.CodigoEventoSigpec == 0 ? "-" : dados.CodigoEventoSigpec.ToString() },
                { "{{TIPO_FORMACAO}}", dados.TipoFormacaoConecta },
                { "{{AREA_PROMOTORA}}", dados.NomeAreaPromotora },
                { "{{NOME_FORMACAO}}", dados.NomeFormacao },
                { "{{MODALIDADE}}", dados.Modalidade },
                { "{{CH_TOTAL}}", cargaHorariaTotal },
                { "{{CH_PRESENCIAL}}", ObterHoraFormatadaStr(dados.CargaHorariaPresencial) },
                { "{{CH_NAO_PRESENCIAL}}", ObterHoraFormatadaStr(dados.CargaHorariaSincrona) },
                { "{{CH_DISTANCIA}}", ObterHoraFormatadaStr(dados.CargaHorariaDistancia) },
                { "{{JUSTIFICATIVA}}", dados.Justificativa },
                { "{{OBJETIVOS}}", dados.Objetivos },
                { "{{CONTEUDO_PROGRAMATICO}}", dados.ConteudoProgramatico },
                { "{{PROCEDIMENTOS}}", dados.Procedimentos },
                { "{{ATIVIDADE_OBRIGATÓRIA}}", dados.DescricaoAtividade },
                { "{{CRITERIOS_AVALIACAO}}", ObterCriteriosCertificacao(dados) },
                { "{{BIBLIOGRAFIA}}", dados.Referencias },
                { "{{QTD_TURMAS}}", dados.QuantidadeTurmas.ToString() },
                { "{{VAGAS_POR_TURMA}}", dados.QuantidadeVagasTurmas.ToString() },
                { "{{TOTAL_VAGAS}}", (dados.QuantidadeTurmas * dados.QuantidadeVagasTurmas).ToString() },
                { "{{PUBLICO_ALVO}}", ObterPublicoAlvo(dados) },
                { "{{FUNCAO_ESPECIFICA}}", ObterFuncaoEspecifica(dados) },
                { "{{VAGAS_REMANESCENTES}}", string.Join(", ", dados.VagasRemanecentes.Select(c => c.Nome)) },
                { "{{CORPO_DOCENTE}}", ObterCorpoDocente(dados) },
                { "{{INSCRICOES_PROCEDIMENTOS}}", ObterInscricoes(dados) },
                { "{{CONTATO_AREA_RESPONSAVEL}}", string.Join(", ", dados.TelefonesAreaPromotora) },
                { "{{LOCAL}}", dados.CronogramaTurmas?.FirstOrDefault()?.Local ?? "A DEFINIR" }
            };
        }

        private static int ObterMinutos(string hora)
        {
            if (string.IsNullOrEmpty(hora)) return 0;
            var partes = hora.Split(":");
            if (partes.Length >= 2 && int.TryParse(partes[0], out int h) && int.TryParse(partes[1], out int m))
            {
                return h * 60 + m;
            }
            return 0;
        }

        private static string ObterHoraFormatada(int totalMinutos)
        {
            if (totalMinutos == 0) return string.Empty;
            var horas = totalMinutos / 60;
            var minutos = totalMinutos % 60;
            return $"{horas:00}:{minutos:00}";
        }

        private static string ObterHoraFormatadaStr(string hora)
        {
            var totalMinutos = ObterMinutos(hora);
            return totalMinutos > 0 ? ObterHoraFormatada(totalMinutos) : string.Empty;
        }
    }
}
