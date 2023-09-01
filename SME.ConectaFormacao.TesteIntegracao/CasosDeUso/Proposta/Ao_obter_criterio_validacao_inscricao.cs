using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
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

        [Fact(DisplayName = "Proposta - obter critérios de validação da inscrição")]
        public async Task Deve_obter_ultimo_roteiro_proposta_formativa_ativo()
        {
            // arrange 
            var criterios = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(3);
            await InserirNaBase(criterios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCriterioValidacaoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar(false);

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(criterios.Count());
        }

        [Fact(DisplayName = "Proposta - obter critérios de validação da inscrição com opção outros")]
        public async Task Deve_obter_ultimo_roteiro_proposta_formativa_ativo_opcap_outros()
        {
            // arrange 
            var criterios = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(3);
            await InserirNaBase(criterios);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCriterioValidacaoInscricao>();

            // act 
            var retorno = await casoDeUso.Executar(true);

            // assert 
            retorno.Any().ShouldBeTrue();
            retorno.Count().ShouldBe(criterios.Count() + 1);
            retorno.Any(t => t.Id == (long)OpcaoListagem.Outros).ShouldBeTrue();
        }
    }
}
