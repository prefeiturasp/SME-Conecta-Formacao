using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.ComponenteCurricular;
using SME.ConectaFormacao.Aplicacao.Interfaces.ComponenteCurricular;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ComponenteCurricular
{
    public class Ao_obter_lista_componente_curricular : TesteBase
    {
        public Ao_obter_lista_componente_curricular(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Componente Curricular - Deve obter lista dos componentes curriculares com opção todos")]
        public async Task Deve_obter_lista_dos_componentes_curriculares_com_opcao_todos()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurma);

            var anoTurma = anosTurma.FirstOrDefault(t => !t.Todos);
            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9, anoTurma.Id);
            await InserirNaBase(componentesCurriculares);

            var filtro = new FiltroListaComponenteCurricularDTO
            {
                AnoTurmaId = new long[] { anoTurma.Id },
                ExibirOpcaoTodos = true
            };
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaComponentesCurriculares>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Count(t => !t.Todos).ShouldBe(componentesCurriculares.Count(c => c.AnoTurmaId == filtro.AnoTurmaId.FirstOrDefault()));
            retorno.Count(t => t.Todos).ShouldBe(1);
        }

        [Fact(DisplayName = "Componente Curricular - Deve obter lista dos componentes curriculares sem opção todos")]
        public async Task Deve_obter_lista_dos_componentes_curriculares_sem_opcao_todos()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurma);

            var anoTurma = anosTurma.FirstOrDefault(t => !t.Todos);
            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9, anoTurma.Id);
            await InserirNaBase(componentesCurriculares);

            var filtro = new FiltroListaComponenteCurricularDTO
            {
                AnoTurmaId = new long[] { anoTurma.Id },
                ExibirOpcaoTodos = false
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaComponentesCurriculares>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Count().ShouldBe(componentesCurriculares.Count(c => c.AnoTurmaId == filtro.AnoTurmaId.FirstOrDefault()));
            retorno.Count(t => t.Todos).ShouldBe(0);
        }

        [Fact(DisplayName = "Componente Curricular - Deve obter lista dos componentes curriculares sem todos")]
        public async Task Deve_obter_lista_dos_componentes_curriculares_todos()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurma);

            var anoTurma = anosTurma.FirstOrDefault(t => !t.Todos);
            var anoTurmaTodos = anosTurma.FirstOrDefault(t => t.Todos);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(9, anoTurma.Id);
            await InserirNaBase(componentesCurriculares);

            var filtro = new FiltroListaComponenteCurricularDTO
            {
                AnoTurmaId = new long[] { anoTurmaTodos.Id },
                ExibirOpcaoTodos = true
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaComponentesCurriculares>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Count().ShouldBe(componentesCurriculares.Count());
        }
    }
}
