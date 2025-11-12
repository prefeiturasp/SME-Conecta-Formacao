using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Ano;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AnoTurma
{
    public class Ao_obter_lista_ano_turma : TesteBase
    {
        public Ao_obter_lista_ano_turma(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Ano Turma - Deve obter a lista de anos das turmas com opção todos")]
        public async Task Deve_obter_lista_anos_das_turmas_com_opcao_todos()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(9);
            await InserirNaBase(anosTurma);

            var filtro = new Aplicacao.Dtos.Ano.FiltroAnoTurmaDTO
            {
                AnoLetivo = anosTurma.FirstOrDefault()?.AnoLetivo,
                Modalidade = new Dominio.Enumerados.Modalidade[] { anosTurma.FirstOrDefault().Modalidade.GetValueOrDefault() },
                ExibirOpcaoTodos = true
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaAnoTurma>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Count(t => !t.Todos).ShouldBe(anosTurma.Count(c => c.Modalidade == filtro.Modalidade.FirstOrDefault()));
            retorno.Count(t => t.Todos).ShouldBe(1);
        }

        [Fact(DisplayName = "Ano Turma - Deve obter a lista de anos das turmas")]
        public async Task Deve_obter_lista_anos_das_turmas_sem_opcao_todos()
        {
            // arrange 
            var anosTurma = AnoTurmaMock.GerarAnoTurma(9);
            await InserirNaBase(anosTurma);

            var filtro = new Aplicacao.Dtos.Ano.FiltroAnoTurmaDTO
            {
                AnoLetivo = anosTurma.FirstOrDefault()?.AnoLetivo,
                Modalidade = new Dominio.Enumerados.Modalidade[] { anosTurma.FirstOrDefault().Modalidade.GetValueOrDefault() },
                ExibirOpcaoTodos = false
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListaAnoTurma>();

            // act 
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Count().ShouldBe(anosTurma.Count(c => c.Modalidade == filtro.Modalidade.FirstOrDefault()));
            retorno.Count(t => t.Todos).ShouldBe(0);
        }
    }
}
