using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora
{
    public class Ao_obter_lista_area_promotora_rede_parceria : TesteBase
    {
        public Ao_obter_lista_area_promotora_rede_parceria(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Área promotora - Deve obter lista rede parceria")]
        public async Task Deve_obter_lista()
        {
            // arrange 
            var areasPromotoras = AreaPromotoraMock.GerarAreaPromotora(20);
            await InserirNaBase(areasPromotoras);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraListaRedeParceria>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(areasPromotoras.Where(t => t.Tipo.EhRedeParceria()).Count());

            foreach (var item in retorno)
            {
                var areaPromotora = areasPromotoras.FirstOrDefault(t => t.Id == item.Id);

                areaPromotora.ShouldNotBeNull();
                item.Descricao.ShouldBe(areaPromotora.Nome);
            }
        }
    }
}
