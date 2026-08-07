using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Base;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.CodafCertificados
{
    public class CertificadoCursistaComRfStrategy(ITemplateService templateService) : CertificadoEstrategiaBase(templateService), ICertificadoCodafGeradorConteudo
    {

        public (string Titulo, string Corpo) GerarConteudoEmail(DadosProcessamentoCodafDto dados, string urlAcesso)
        {
            var titulo = @$"PARABÉNS! SEU CERTIFICADO FOI EMITIDO | {dados.NomeFormacao}";

            var corpo = $@"
                <p>Olá <b>{dados.NomeCompleto}</b>! Parabéns!</p>
                <p>Você concluiu sua participação como <b>cursista</b> na formação <b>{dados.NomeFormacao}</b>.</p>
                <p>O certificado pode ser visualizado na tela 'Meus certificados' na plataforma Conecta, clicando <a href='{urlAcesso}' target='_blank'>aqui</a>.</p>";

            return (titulo, corpo);
        }

        public string GerarHtml(DadosEmissaoCertificadoCodafDto dados)
        {
            var layout = ObterLayoutBase(dados);

            return layout
                .Replace("{{TEXTO_CERTIFICADO}}", GerarCorpoCertificado(dados))
                .Replace("{{CLASSE_SELO}}", "")
                .MinificarHtml();
        }

        private static string GerarCorpoCertificado(DadosEmissaoCertificadoCodafDto dados)
        {
            return $@"Declaramos para os devidos fins que o(a) servidor(a), <b><i>{StringExtensao.FormatarNomePessoa(dados.NomeExibicao)}</i></b>,
                    RF: <b><i>{StringExtensao.AplicarMascaraRf(dados.Documento)}</i></b>, participou do {dados.TipoFormacao} <b><i>{dados.NomeFormacao}</i></b> 
                    promovido pela {ObterTextoEmissorCorpo(dados)} da Secretaria Municipal de Educação, no período de {dados.DataInicio:dd/MM/yyyy} a {dados.DataFim:dd/MM/yyyy}, 
                    com carga horária de {dados.DefinirCargaHoraria()} horas,
                    tendo obtido nota de aproveitamento {dados.ConceitoFinal} e frequência de {dados.PercentualFrequencia}%.";
        }
    }
}
