using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces
{
    public interface ICertificadoCodafGeradorConteudo
    {
        string GerarHtml(DadosEmissaoCertificadoCodafDto dados);
        (string Titulo, string Corpo) GerarConteudoEmail(DadosProcessamentoCertificadoCodafDto dados, string urlAcesso);
    }
}