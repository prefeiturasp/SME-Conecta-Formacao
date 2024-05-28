using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_obter_notificacao_tipo : TesteBase
    {
        public Ao_obter_notificacao_tipo(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Notificacao - Deve retornar os tipos da notificação")]
        public async Task Deve_retornar_tipo_notificacao()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTipoNotificacao>();

            // act
            var retorno = await casoDeUso.Executar();

            // assert
            retorno.Any(t => t.Id == (long)NotificacaoTipo.Proposta).ShouldBeTrue();
        }
    }
}
