using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Modalidade;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Modalidade
{
    public class Ao_obter_modalidade : TesteBase
    {
        public Ao_obter_modalidade(CollectionFixture collectionFixture, bool limparBanco = false) : base(collectionFixture, limparBanco)
        {
        }

        [Fact(DisplayName = "Modalidade - Deve obter modalidades")]
        public async Task Deve_obter_modalidades()
        {
            // arrange 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterModalidade>();

            // act 
            var retorno = await casoDeUso.Executar();

            // assert 
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.EJA).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.CIEJA).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.EducacaoInfantil).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.Fundamental).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.Medio).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.MOVA).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.CELP).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.ETEC).ShouldBeTrue();
            retorno.Any(t => t.Id == (long)Dominio.Enumerados.Modalidade.CMCT).ShouldBeTrue();
        }
    }
}
