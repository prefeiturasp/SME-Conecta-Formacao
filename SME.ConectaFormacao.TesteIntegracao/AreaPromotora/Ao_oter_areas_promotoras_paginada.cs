using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.TesteIntegracao.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.AreaPromotora
{
    public class Ao_oter_areas_promotoras_paginada : TesteBase
    {
        public Ao_oter_areas_promotoras_paginada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AreaPromotoraPaginacaoMock.Montar();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar registros consulta paginada com filtro")]
        public async Task Deve_retornar_registros_com_filtro()
        {
            // arrange
            await InserirNaBase(AreaPromotoraPaginacaoMock.AreasPromotoras);

            var filtro = AreaPromotoraPaginacaoMock.FiltrosAreaPromotoraDTOValido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraPaginada>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(1);
        }

        [Fact(DisplayName = "Área promotora - Deve retornar registros consulta paginada sem filtro")]
        public async Task Deve_retornar_registros_sem_filtros()
        {
            // arrange
            await InserirNaBase(AreaPromotoraPaginacaoMock.AreasPromotoras);

            var filtro = AreaPromotoraPaginacaoMock.FiltrosAreaPromotoraDTOVazio;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraPaginada>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(2);
        }

        [Fact(DisplayName = "Área promotora - Não deve retornar registros consulta paginada filtros invalidos")]
        public async Task Nao_deve_retornar_registros_filtros_invalidos()
        {
            // arrange
            await InserirNaBase(AreaPromotoraPaginacaoMock.AreasPromotoras);

            var filtro = AreaPromotoraPaginacaoMock.FiltrosAreaPromotoraDTOInvalido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterAreaPromotoraPaginada>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeFalse();
        }
    }
}
