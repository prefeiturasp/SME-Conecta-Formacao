using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria
{
    public class Ao_obter_situacao_usuario_parceria : TesteBase
    {
        public Ao_obter_situacao_usuario_parceria(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Usuário Rede Parceria - obter situações")]
        public async Task Deve_obter_situacoes_usuario_rede_parceria()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterSituacaoUsuarioRedeParceria>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)SituacaoUsuario.Ativo).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)SituacaoUsuario.Inativo).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)SituacaoUsuario.AguardandoValidacaoEmail).ShouldBeFalse();
        }
    }
}
