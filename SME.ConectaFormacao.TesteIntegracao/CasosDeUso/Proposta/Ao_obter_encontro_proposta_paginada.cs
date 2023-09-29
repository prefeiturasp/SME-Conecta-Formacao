using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_encontro_proposta_paginada : TestePropostaBase
    {
        public Ao_obter_encontro_proposta_paginada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve obter os encontros paginados ")]
        public async Task Deve_obter_encontro_paginado()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaEncontroPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalRegistros.ShouldBe(proposta.Encontros.Count());
        }
    }
}
