using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public interface IGeradorRelatorioCodafExcelService
    {
        byte[] GerarRelatorio(DadosPrincipaisRelatorioCodafDto dadosBrutos, bool ehCodafSuplementar);
    }
}
