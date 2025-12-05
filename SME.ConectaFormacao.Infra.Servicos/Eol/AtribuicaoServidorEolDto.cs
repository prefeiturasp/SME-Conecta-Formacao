using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Infra.Servicos.Eol
{
    public class AtribuicaoServidorEolDto
    {
        public int CdEtapaEnsino { get; set; }
        public required string AnoSerie { get; set; }
        public int CdComponenteCurricular { get; set; }
        public required string CdRegistroFuncional { get; set; }
        public required string CodigoUe { get; set; }
        public bool Excluido { get; set; }
        public Modalidade Modalidade => ConverterModalidade(CdEtapaEnsino);
        public string ChaveNegocio => ObterChaveNegocio();

        public string ObterChaveNegocio()
        {
            return $"{CdRegistroFuncional}-{(short)Modalidade}-{AnoSerie}-{CdComponenteCurricular}-{CodigoUe}";
        }
        private static Modalidade ConverterModalidade(int etapaEnsino)
        {
            return etapaEnsino switch
            {
                1 or 10 => Modalidade.EducacaoInfantil,
                3 or 11 => Modalidade.EJA,
                2 => Modalidade.CIEJA,
                5 or 13 => Modalidade.Fundamental,
                6 or 8 or 9 or 14 or 17 or 18 or 19 or 20 or 21 or 22 or 23 or _ => Modalidade.Medio
            };
        }
    }
}
