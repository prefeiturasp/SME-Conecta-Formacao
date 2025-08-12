namespace SME.ConectaFormacao.Dominio.Enumerados
{
    public enum PropostaIntegrarNoSGA
    {
        NAO = 0,
        SIM = 1
    }
    public static class PropostaIntegrarNoSGAExtensions
    {
        public static bool ToBool(this PropostaIntegrarNoSGA valor)
        {
            return valor == PropostaIntegrarNoSGA.SIM;
        }
    }

}
