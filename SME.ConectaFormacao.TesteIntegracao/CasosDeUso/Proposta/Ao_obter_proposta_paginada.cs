using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta_paginada : TestePropostaBase
    {
        public Ao_obter_proposta_paginada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada com filtro")]
        public async Task Deve_retornar_registros_com_filtro()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var propostas = await InserirNaBaseProposta(15, areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var filtro = PropostaPaginacaoMock.GerarPropostaFiltrosDTOValido(areaPromotora, propostas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(1);
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada sem filtro")]
        public async Task Deve_retornar_registros_sem_filtros()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            await InserirNaBaseProposta(15, areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var filtro = new PropostaFiltrosDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(2);
        }

        [Fact(DisplayName = "Proposta - Não deve retornar registros consulta paginada filtros invalidos")]
        public async Task Nao_deve_retornar_registros_filtros_invalidos()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var propostas = await InserirNaBaseProposta(15, areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var filtro = PropostaPaginacaoMock.GerarPropostaFiltrosDTOInvalido();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeFalse();
        }
    }
}
