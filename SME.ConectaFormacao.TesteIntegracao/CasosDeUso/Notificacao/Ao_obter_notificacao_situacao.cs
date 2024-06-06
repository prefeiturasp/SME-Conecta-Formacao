using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Notificacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Notificacao
{
    public class Ao_obter_notificacao_situacao : TesteBase
    {
        public Ao_obter_notificacao_situacao(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Notificacao - Deve retornar as situações da notificação")]
        public async Task Deve_retornar_situacao_notificacao()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterSituacaoNotificacao>();

            // act
            var retorno = await casoDeUso.Executar();

            // assert
            retorno.Any(t => t.Id == (long)NotificacaoUsuarioSituacao.Lida).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)NotificacaoUsuarioSituacao.NaoLida).ShouldBeTrue();
        }
    }
}
