using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks
{
    public static class AoObterParecerPropostaMock
    {
        public static PropostaMovimentacaoDTO PropostaMovimentacaoDto { get; set; }

        public static void Montar()
        {
            var faker = new Faker<PropostaMovimentacaoDTO>("pt_BR");
            faker.RuleFor(x => x.Parecer, f => f.Lorem.Text());
            faker.RuleFor(x => x.Situacao, f => SituacaoProposta.AguardandoAnaliseDf);
            PropostaMovimentacaoDto = faker.Generate();
        }
    }
}
