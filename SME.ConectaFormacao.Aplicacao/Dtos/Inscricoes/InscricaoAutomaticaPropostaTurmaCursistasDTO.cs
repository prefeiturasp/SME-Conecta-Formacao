using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes
{
    public class InscricaoAutomaticaPropostaTurmaCursistasDTO
    {
        public long Id { get; set; }
        public IEnumerable<CursistaServicoEol> Cursistas { get; set; }
    }
}
