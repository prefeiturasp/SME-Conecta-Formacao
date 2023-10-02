using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_modalidades : TesteBase
    {
        public Ao_obter_modalidades(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Proposta - obter modalidades tipo formacao curso")]
        public async Task Deve_obter_modalidades_tipo_formacao_curso()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterModalidades>();

            // act 
            var retorno = await casoDeUso.Executar(TipoFormacao.Curso);

            // assert 
            retorno.Any(t => t.Id == (long)Modalidade.Distancia).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Modalidade.Presencial).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Modalidade.Hibrido).ShouldBeFalse();
        }

        [Fact(DisplayName = "Proposta - obter modalidades tipo formacao evento")]
        public async Task Deve_obter_modalidades_tipo_formacao_evento()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterModalidades>();

            // act 
            var retorno = await casoDeUso.Executar(TipoFormacao.Evento);

            // assert 
            retorno.Any(t => t.Id == (long)Modalidade.Distancia).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Modalidade.Presencial).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Modalidade.Hibrido).ShouldBeTrue();
        }
    }
}
