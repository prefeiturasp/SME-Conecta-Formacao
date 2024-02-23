using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_obter_inscricao_tipo : TesteBase
    {
        public Ao_obter_inscricao_tipo(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }


        [Fact(DisplayName = "Inscrição - Deve obter uma lista com todos os tipos")]
        public async Task Deve_obter_lista_com_todos_os_tipos_de_inscricao()
        {
            //Arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoTipo>();
            var tipos = Enum.GetValues(typeof(TipoInscricao)).Cast<TipoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert
            retorno.ShouldNotBeNull();
            retorno.Count().ShouldBeEquivalentTo(tipos.Count());

            foreach (var tipo in tipos)
                retorno.Count(x => x.Descricao == tipo.Nome()).ShouldBeEquivalentTo(1);
        }
    }
}