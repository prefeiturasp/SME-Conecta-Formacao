using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Base;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias
{
    public class CertificadoCursistaComRfStrategy(ITemplateService templateService) : CertificadoEstrategiaBase(templateService), ICertificadoCodafGeradorConteudo
    {
        public (string Titulo, string Corpo) GerarConteudoEmail(DadosProcessamentoCertificadoCodafDto dados, string urlAcesso)
        {
            var titulo = @$"PARABÉNS! SEU CERTIFICADO FOI EMITIDO | {dados.NomeFormacao}";

            var corpo = $@"
            <p>Olá <b>{dados.NomeCompleto}</b>! Parabéns!</p>
            <p>Você concluiu com sucesso sua participação como <b>cursista</b> na formação <b>{dados.NomeFormacao}</b>.</p>
            <p>Você pode visualizar seu novo certificado pela tela 'Meus certificados' na plataforma Conecta clicando <a href='{urlAcesso}' target='_blank'>aqui</a>.</p>
            <p>Te desejamos muito sucesso, e até a próxima!</p>";

            return (titulo, corpo);
        }

        public string GerarHtml(DadosEmissaoCertificadoCodafDto dados)
        {
            var layout = ObterLayoutBase(dados);
            var imgCabecalho = templateService.ObterImagemBase64("header.jpg");
            return layout.Replace("{{TEXTO_CERTIFICADO}}", GerarCorpoCertificado(dados))
                         .Replace("{{IMG_MOLDURA_LATERAL}}", imgCabecalho)
                         .MinificarHtml();
        }

        private static string GerarCorpoCertificado(DadosEmissaoCertificadoCodafDto dados)
        {
            return $@"Certificamos para os devidos fins que o(a) servidor(a), <b>{dados.NomeCompleto}</b>, 
                      RF: {dados.Documento}, participou do {dados.TipoFormacao} <b>{dados.NomeFormacao}</b> 
                      promovido pela {dados.DreCoordenadoria} da Secretaria Municipal de Educação,
                      no período de 
                      com carga horária de {dados.HorasTotais ?? dados.CargaHorariaTotalOutra.ConverterHoraMinutoParaInteiro():00} horas,
                      tendo obtido nota de aproveitamento {dados.ConceitoFinal} 
                      e frequência de {dados.PercentualFrequencia}%.";
        }
    }
}
