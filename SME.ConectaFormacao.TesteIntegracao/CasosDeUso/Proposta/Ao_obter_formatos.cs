using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_formatos : TesteBase
    {
        public Ao_obter_formatos(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Proposta - obter formatos tipo formacao curso")]
        public async Task Deve_obter_formatos_tipo_formacao_curso()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterFormatos>();

            // act 
            var retorno = await casoDeUso.Executar(TipoFormacao.Curso);

            // assert 
            retorno.Any(t => t.Id == (long)Formato.Distancia).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Formato.Presencial).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Formato.Hibrido).ShouldBeFalse();
        }

        [Fact(DisplayName = "Proposta - obter formatos tipo formacao evento")]
        public async Task Deve_obter_formatos_tipo_formacao_evento()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterFormatos>();

            // act 
            var retorno = await casoDeUso.Executar(TipoFormacao.Evento);

            // assert 
            retorno.Any(t => t.Id == (long)Formato.Distancia).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Formato.Presencial).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Formato.Hibrido).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - obter todos os formatos")]
        public async Task Deve_obter_todos_formatos()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTodosFormatos>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)Formato.Distancia).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Formato.Presencial).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Formato.Hibrido).ShouldBeTrue();
        }
    }
}
