using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_obter_notificacao_categoria : TesteBase
    {
        public Ao_obter_notificacao_categoria(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Notificacao - Deve retornar as categorias da notificação")]
        public async Task Deve_retornar_categorias_notificacao()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCategoriaNotificacao>();

            // act
            var retorno = await casoDeUso.Executar();

            // assert
            retorno.Any(t => t.Id == (long)NotificacaoCategoria.Alerta).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)NotificacaoCategoria.Workflow_Aprovacao).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)NotificacaoCategoria.Aviso).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)NotificacaoCategoria.Informe).ShouldBeTrue();
        }
    }
}
