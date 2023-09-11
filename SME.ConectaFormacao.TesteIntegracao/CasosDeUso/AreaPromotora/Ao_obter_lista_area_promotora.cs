using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora
{
    public class Ao_obter_lista_area_promotora : TesteBase
    {
        public Ao_obter_lista_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Área promotora - Deve obter lista")]
        public async Task Deve_obter_lista()
        {
            // arrange 
            var areasPromotoras = AreaPromotoraMock.GerarAreaPromotora(10);
            await InserirNaBase(areasPromotoras);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraLista>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(areasPromotoras.Count());

            foreach (var item in retorno)
            {
                var areaPromotora = areasPromotoras.FirstOrDefault(t => t.Id == item.Id);

                areaPromotora.ShouldNotBeNull();
                item.Descricao.ShouldBe(areaPromotora.Nome);
            }
        }
    }
}
