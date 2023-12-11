using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.CargoFuncao;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.CargoFuncao
{
    public class Ao_obter_cargo_funcao : TesteBase
    {
        public Ao_obter_cargo_funcao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Cargo Função - Deve obter os cargos")]
        public async Task Deve_obter_os_cargos()
        {
            // arrange 
            var cargosEFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosEFuncoes);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCargoFuncao>();

            // act 
            var retorno = await casoDeUso.Executar(Dominio.Enumerados.CargoFuncaoTipo.Cargo, false);

            // assert
            retorno.Count().ShouldBe(cargosEFuncoes.Count(c => c.Tipo == Dominio.Enumerados.CargoFuncaoTipo.Cargo));
        }

        [Fact(DisplayName = "Cargo Função - Deve obter as funções")]
        public async Task Deve_obter_as_funcoes()
        {
            // arrange 
            var cargosEFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosEFuncoes);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCargoFuncao>();

            // act 
            var retorno = await casoDeUso.Executar(Dominio.Enumerados.CargoFuncaoTipo.Funcao, false);

            // assert
            retorno.Count().ShouldBe(cargosEFuncoes.Count(c => c.Tipo == Dominio.Enumerados.CargoFuncaoTipo.Funcao));
        }

        [Fact(DisplayName = "Cargo Função - Deve obter todos os cargos e funções")]
        public async Task Deve_obter_todos_cargos_funcoes()
        {
            // arrange 
            var cargosEFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosEFuncoes);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCargoFuncao>();

            // act 
            var retorno = await casoDeUso.Executar(null, false);

            // assert
            retorno.Count().ShouldBe(cargosEFuncoes.Count());
        }

        [Fact(DisplayName = "Cargo Função - Deve obter todos os cargos e funções opção outros")]
        public async Task Deve_obter_todos_cargos_funcoes_opcao_outros()
        {
            // arrange 
            var cargosEFuncoes = CargoFuncaoMock.GerarCargoFuncao(10, true);
            await InserirNaBase(cargosEFuncoes);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterCargoFuncao>();

            // act 
            var retorno = await casoDeUso.Executar(null, true);

            // assert
            retorno.Count().ShouldBe(cargosEFuncoes.Count());
            retorno.Any(t => t.Outros).ShouldBeTrue();
        }
    }
}
