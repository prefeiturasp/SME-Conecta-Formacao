using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Dominio.Entidades
{
    public class AtribuicaoServidorEol
    {
        public Guid Id { get; private set; }
        public Modalidade CdModalidade { get; private set; }
        public string AnoSerie { get; private set; }
        public int CdComponenteCurricular { get; private set; }
        public string CdRegistroFuncional { get; private set; }
        public string CodigoUe { get; private set; }
        public DateTime DataAtualizacao { get; private set; }
        public string ChaveNegocio { get; private set; }

        public AtribuicaoServidorEol(Modalidade cdModalidade, string anoSerie, int cdComponenteCurricular, string cdRegistroFuncional, string codigoUe, string chaveNegocio)
        {
            Id = Guid.NewGuid();
            CdModalidade = cdModalidade;
            AnoSerie = anoSerie;
            CdComponenteCurricular = cdComponenteCurricular;
            CdRegistroFuncional = cdRegistroFuncional;
            CodigoUe = codigoUe;
            ChaveNegocio = chaveNegocio;
            DataAtualizacao = DateTime.UtcNow;
        }

        protected AtribuicaoServidorEol()  // EF Core
        {
            AnoSerie = null!;
            CdRegistroFuncional = null!;
            CodigoUe = null!;
            ChaveNegocio = null!;
        }
    }
}
