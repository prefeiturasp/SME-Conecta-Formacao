using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_sugestao_parecer_pareceristas : TestePropostaBase
    {
        public Ao_obter_sugestao_parecer_pareceristas(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - deve obter sugestões de parecer dos pareceristas por situação")]
        public async Task Deve_obter_Sugestoes_Parecer_Pareceristas_por_situacao()
        {
            // arrange 
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerPelaDF, quantidadeParecerista: 100);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterSugestaoParecerPareceristas>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.Count.ShouldBe(retorno.Count());
        }
    }
}
