using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class PropostaPaginacaoMock
    {
        public static PropostaFiltrosDTO GerarPropostaFiltrosDTOValido(Dominio.Entidades.AreaPromotora areaPromotora, IEnumerable<Dominio.Entidades.Proposta> propostas)
        {
            var propostaFiltro = propostas.FirstOrDefault();

            return new PropostaFiltrosDTO
            {
                AreaPromotoraId = areaPromotora.Id,
                Formato = propostaFiltro.Formato,
                NomeFormacao = propostaFiltro.NomeFormacao,
                PublicoAlvoIds = propostaFiltro.PublicosAlvo.Select(t => t.CargoFuncaoId).ToArray(),
                Situacao = propostaFiltro.Situacao,
                NumeroHomologacao = propostaFiltro.NumeroHomologacao
            };
        }

        public static PropostaFiltrosDTO GerarPropostaFiltrosDTOInvalido()
        {
            var filtroInvalidoFaker = new Faker<PropostaFiltrosDTO>();
            filtroInvalidoFaker.RuleFor(x => x.AreaPromotoraId, f => f.Random.Number(999));
            filtroInvalidoFaker.RuleFor(x => x.NomeFormacao, f => f.Lorem.Sentence(3));

            return filtroInvalidoFaker.Generate();
        }
    }
}
