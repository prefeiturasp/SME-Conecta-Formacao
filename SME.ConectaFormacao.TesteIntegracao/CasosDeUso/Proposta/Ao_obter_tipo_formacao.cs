using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_tipo_formacao : TesteBase
    {
        public Ao_obter_tipo_formacao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - obter tipos de formacao")]
        public async Task Deve_obter_tipos_formacao()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTipoFormacao>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)TipoFormacao.Evento).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)TipoFormacao.Curso).ShouldBeTrue();
        }
    }
}
