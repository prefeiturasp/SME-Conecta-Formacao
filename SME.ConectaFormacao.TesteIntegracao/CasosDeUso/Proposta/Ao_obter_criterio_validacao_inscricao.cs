using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_criterio_validacao_inscricao : TesteBase
    {
        public Ao_obter_criterio_validacao_inscricao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - obter critérios de validação da inscrição com outros")]
        public async Task Deve_obter_criterio_validacao_inscricao_com_outros()
        {
            // arrange 
            var criterios = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(3, false, true);
            await InserirNaBase(criterios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCriterioValidacaoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar(true);

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(criterios.Count());
            retorno.Any(t => t.Outros).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - obter critérios de validação da inscrição sem outros")]
        public async Task Deve_obter_criterio_validacao_inscricao_sem_outros()
        {
            // arrange 
            var criterios = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(3, false, false);
            await InserirNaBase(criterios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCriterioValidacaoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar(true);

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(criterios.Count());
            retorno.Any(t => t.Outros).ShouldBeFalse();
        }

        [Fact(DisplayName = "Proposta - obter critérios de validação da inscrição com unico")]
        public async Task Deve_obter_criterio_validacao_inscricao_com_unico()
        {
            // arrange 
            var criterios = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(3, true, false);
            await InserirNaBase(criterios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCriterioValidacaoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar(true);

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(criterios.Count());
            retorno.Any(t => t.Unico).ShouldBeTrue();
        }
    }
}
