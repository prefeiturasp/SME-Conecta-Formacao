using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_tipo_encontro : TesteBase
    {
        public Ao_obter_tipo_encontro(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Proposta - obter tipos de encontro")]
        public async Task Deve_obter_tipos_encontro()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTipoEncontro>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)TipoEncontro.Presencial).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)TipoEncontro.Assincrono).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)TipoEncontro.Sincrono).ShouldBeTrue();
        }
    }
}
