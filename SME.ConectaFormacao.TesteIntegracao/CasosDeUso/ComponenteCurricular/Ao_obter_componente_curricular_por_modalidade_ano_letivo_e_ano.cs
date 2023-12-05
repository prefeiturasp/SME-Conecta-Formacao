using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.CargoFuncao
{
    public class Ao_obter_componente_curricular_por_modalidade_ano_letivo_e_ano : TesteBase
    {
        public Ao_obter_componente_curricular_por_modalidade_ano_letivo_e_ano(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Componente Curricular - Deve obter os componentes curriculares por modalidade, ano letivo atual e ano")]
        public async Task Deve_obter_os_componentes_curriculares_por_modalidade_ano_letivo_e_ano()
        {
            // arrange 
            var anos = AnoMock.GerarAno(9);
            await InserirNaBase(anos);
            
            foreach (var ano in anos)
            {
               var componenteCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9);
               
               foreach (var componenteCurricular in componenteCurriculares)
               {
                   componenteCurricular.AnoId = ano.Id;
                   await InserirNaBase(componenteCurricular);    
               }
            }
            
            AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoMock.Montar(Modalidade.Fundamental);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno>();

            // act 
            var filtro = AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoMock.ComponenteCurricularFiltrosDto;
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            var componentesCurricularesInseridos = ObterTodos<ComponenteCurricular>();
            retorno.Count().ShouldBe(componentesCurricularesInseridos.Count(c => c.AnoId == filtro.AnoId));
        }
        
        [Fact(DisplayName = "Componente Curricular - Deve obter os componentes curriculares de todas as modalidades, ano letivo atual e todos os anos")]
        public async Task Deve_obter_os_componentes_curriculares_por_todas_as_modalidades_ano_letivo_e_todos_os_anos()
        {
            // arrange 
            var anos = AnoMock.GerarAno(9);
            await InserirNaBase(anos);
            
            foreach (var ano in anos)
            {
                var componenteCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9);
               
                foreach (var componenteCurricular in componenteCurriculares)
                {
                    componenteCurricular.AnoId = ano.Id;
                    await InserirNaBase(componenteCurricular);    
                }
            }
            
            AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoMock.Montar(Modalidade.Fundamental);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterComponentesCurricularesPorModalidadeAnoLetivoAno>();

            // act 
            var filtro = AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoMock.ComponenteCurricularFiltrosDto;
            filtro.AnoId = 999;
            filtro.Modalidade = Modalidade.TODAS;
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            var componentesCurricularesInseridos = ObterTodos<ComponenteCurricular>();
            retorno.Count().ShouldBe(componentesCurricularesInseridos.Count());
        }
    }
}
