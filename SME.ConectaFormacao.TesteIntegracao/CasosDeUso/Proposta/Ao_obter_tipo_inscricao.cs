using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_tipo_inscricao : TesteBase
    {
        public Ao_obter_tipo_inscricao(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
        }

        [Fact(DisplayName = "Proposta - obter tipos de inscrição")]
        public async Task Deve_obter_tipos_inscricao()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTipoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)TipoInscricao.Automatica).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)TipoInscricao.Optativa).ShouldBeTrue();
        }
    }
}
