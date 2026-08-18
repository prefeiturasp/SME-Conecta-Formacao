using SME.ConectaFormacao.Infra.Dados.Dtos.CodafSuplementares;

namespace SME.ConectaFormacao.Infra.Dados.Relatorios
{
    public interface IGeradorRelatorioCodafSuplementarExcelService
    {
        byte[] GerarRelatorio(DadosPrincipaisRelatorioCodafDto dadosBrutos, bool ehCodafSuplementar);
    }
}
