using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ComponenteCurricular
{
    public class Ao_obter_componente_curricular_por_modalidade_ano_letivo_e_ano_turma : TesteBase
    {
        public Ao_obter_componente_curricular_por_modalidade_ano_letivo_e_ano_turma(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Componente Curricular - Deve obter os componentes curriculares por modalidade, ano letivo atual e ano turma")]
        public async Task Deve_obter_os_componentes_curriculares_por_modalidade_ano_letivo_e_ano_turma()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(9);
            await InserirNaBase(anosTurma);
            
            foreach (var anoTurma in anosTurma)
            {
               var componenteCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9);
               
               foreach (var componenteCurricular in componenteCurriculares)
               {
                   componenteCurricular.AnoTurmaId = anoTurma.Id;
                   await InserirNaBase(componenteCurricular);    
               }
            }
            
            AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoTurmaMock.Montar(Modalidade.Fundamental);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterComponentesCurricularesEAnosTurmaPorModalidadeAnoLetivoAnoTurma>();

            // act 
            var filtro = AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoTurmaMock.ComponenteCurricularEAnoTurmaFiltrosDto;
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            var componentesCurricularesInseridos = ObterTodos<Dominio.Entidades.ComponenteCurricular>();
            retorno.Count().ShouldBe(componentesCurricularesInseridos.Count(c => c.AnoTurmaId == filtro.AnoTurmaId));
        }
        
        [Fact(DisplayName = "Componente Curricular - Deve obter os componentes curriculares de todas as modalidades, ano letivo atual e todos os anos turma")]
        public async Task Deve_obter_os_componentes_curriculares_por_todas_as_modalidades_ano_letivo_e_todos_os_anos_turma()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(9);
            await InserirNaBase(anosTurma);
            
            foreach (var anoTurma in anosTurma)
            {
                var componenteCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9);
               
                foreach (var componenteCurricular in componenteCurriculares)
                {
                    componenteCurricular.AnoTurmaId = anoTurma.Id;
                    await InserirNaBase(componenteCurricular);    
                }
            }
            
            AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoTurmaMock.Montar(Modalidade.Fundamental);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterComponentesCurricularesEAnosTurmaPorModalidadeAnoLetivoAnoTurma>();

            // act 
            var filtro = AoObterComponentesCurricularesPorModalidadeAnoLetivoAnoTurmaMock.ComponenteCurricularEAnoTurmaFiltrosDto;
            filtro.AnoTurmaId = 999;
            filtro.Modalidade = Modalidade.TODAS;
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            var componentesCurricularesInseridos = ObterTodos<Dominio.Entidades.ComponenteCurricular>();
            retorno.Count().ShouldBe(componentesCurricularesInseridos.Count());
        }
    }
}
