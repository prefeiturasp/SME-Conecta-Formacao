using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Base;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias
{
    public class CertificadoRegenteStrategy(ITemplateService templateService) : CertificadoEstrategiaBase(templateService), ICertificadoCodafGeradorConteudo
    {
        public string GerarHtml(DadosEmissaoCertificadoCodafDto dados)
        {
            var layout = ObterLayoutBase(dados);
            var imgMolduraLateral = templateService.ObterImagemBase64("barra_lateral_padrao_certificado_codaf.png");
            return layout.Replace("{{TEXTO_CERTIFICADO}}", GerarCorpoCertificado(dados))
                         .Replace("{{IMG_MOLDURA_LATERAL}}", imgMolduraLateral)
                         .MinificarHtml();
        }

        private static string GerarCorpoCertificado(DadosEmissaoCertificadoCodafDto dados)
        {
            return $@"
            <p>Certificamos para os devidos fins que o(a) servidor(a), <b>{dados.NomeCompleto}</b>, 
            R.F. {dados.Documento}, ministrou o Evento <b>{dados.NomeFormacao}</b> 
            promovido pelo(a) SME em {dados.DataRealizacao:dd/MM/yyyy}, 
            com carga horária de {dados.HorasTotais ?? dados.CargaHorariaTotalOutra.ConverterHoraMinutoParaInteiro():00} horas.</p>";
        }

        public (string Titulo, string Corpo) GerarConteudoEmail(DadosProcessamentoCertificadoCodafDto dados, string urlAcesso)
        {
            var titulo = @$"PARABÉNS! SEU CERTIFICADO FOI EMITIDO | {dados.NomeFormacao}";

            var corpo = $@"
            <p>Olá <b>{dados.NomeCompleto}</b>! Parabéns!</p>
            <p>Você concluiu com sucesso sua participação como <b>regente</b> na formação <b>{dados.NomeFormacao}</b>.</p>
            <p>Você pode visualizar seu novo certificado pela tela 'Meus certificados' na plataforma Conecta clicando <a href='{urlAcesso}' target='_blank'>aqui</a>.</p>
            <p>Te desejamos muito sucesso, e até a próxima!</p>";

            return (titulo, corpo);
        }
    }
}
