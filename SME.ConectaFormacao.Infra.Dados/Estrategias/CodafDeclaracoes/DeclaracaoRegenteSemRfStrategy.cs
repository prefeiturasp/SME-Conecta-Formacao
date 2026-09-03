using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Base;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Templates;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.CodafDeclaracoes
{
    public class DeclaracaoRegenteSemRfStrategy(ITemplateService templateService) : DeclaracaoEstrategiaBase(templateService), IDeclaracaoCodafGeradorConteudo
    {
        public string GerarHtml(DadosEmissaoDeclaracaoCodafDto dados)
        {
            var layout = ObterLayoutBase(dados);
            return layout
                .Replace("{{TEXTO_DECLARACAO}}", GerarCorpoDeclaracao(dados))
                .MinificarHtml();
        }

        private static string GerarCorpoDeclaracao(DadosEmissaoDeclaracaoCodafDto dados)
        {
            return $@"
            <p>Declaramos para os devidos fins que o(a) servidor(a), <b><i>{StringExtensao.FormatarNomePessoa(dados.NomeExibicao)}</i></b>, 
            CPF <b><i>{StringExtensao.AplicarMascaraCpf(dados.Documento)}</b></i>, ministrou o {dados.TipoFormacao} <b><i>{dados.NomeFormacao}</i></b> 
            promovido pelo(a) {ObterTextoEmissorCorpo(dados)} da Secretaria Municipal de Educação no período de {dados.DataInicio:dd/MM/yyyy} a {dados.DataFim:dd/MM/yyyy}, com carga horária 
            de {dados.DefinirCargaHoraria()} horas.</p>";
        }

        public (string Titulo, string Corpo) GerarConteudoEmail(DadosProcessamentoCodafDto dados, string urlAcesso)
        {
            var titulo = @$"PARABÉNS! SUA DECLARAÇÃO FOI EMITIDA | {dados.NomeFormacao}";

            var corpo = $@"
                <p>Olá <b>{dados.NomeCompleto}</b>! Parabéns!</p>
                <p>Você concluiu sua participação como <b>regente</b> na formação <b>{dados.NomeFormacao}</b>.</p>
                <p>A declaração pode ser visualizada na tela 'Meus certificados e declarações' na plataforma Conecta, clicando <a href='{urlAcesso}' target='_blank'>aqui</a>.</p>";

            return (titulo, corpo);
        }
    }
}
