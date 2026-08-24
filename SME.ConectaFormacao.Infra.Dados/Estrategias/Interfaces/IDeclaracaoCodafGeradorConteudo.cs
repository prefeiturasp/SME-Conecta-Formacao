using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCertificados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafDeclaracoes;

namespace SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces
{
    public interface IDeclaracaoCodafGeradorConteudo
    {
        string GerarHtml(DadosEmissaoDeclaracaoCodafDto dados);
        (string Titulo, string Corpo) GerarConteudoEmail(DadosProcessamentoCodafDto dados, string urlAcesso);
    }
}