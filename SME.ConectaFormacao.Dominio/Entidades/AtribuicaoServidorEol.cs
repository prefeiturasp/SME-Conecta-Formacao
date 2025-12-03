using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class AtribuicaoServidorEol(Modalidade cdModalidade, string anoSerie, int cdComponenteCurricular, string cdRegistroFuncional, string codigoUe, string chaveNegocio)
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Modalidade CdModalidade { get; private set; } = cdModalidade;
        public string AnoSerie { get; private set; } = anoSerie;
        public int CdComponenteCurricular { get; private set; } = cdComponenteCurricular;
        public string CdRegistroFuncional { get; private set; } = cdRegistroFuncional;
        public string CodigoUe { get; private set; } = codigoUe;
        public DateTime DataAtualizacao { get; private set; } = DateTime.UtcNow;
        public string ChaveNegocio { get; private set; } = chaveNegocio;
        protected AtribuicaoServidorEol() : this(Modalidade.Fundamental, string.Empty, 0, string.Empty, string.Empty, string.Empty) { } // EF Core
    }
}
