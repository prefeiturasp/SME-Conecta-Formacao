using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_parecer_proposta : TestePropostaBase
    {
        public Ao_salvar_parecer_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta parecer - Cadastrar um parecer")]
        public async Task Deve_cadastrar_proposta_parecer()
        {
            // arrange
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var propostaParecerDto = PropostaSalvarMock.GerarParecer();
            propostaParecerDto.PropostaId = proposta.Id;
            
            // act
            var retorno = await useCase.Executar(propostaParecerDto);

            // assert
            retorno.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaParecer>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Campo.ShouldBe(propostaParecerDto.Campo);
            propostaParecer.Descricao.ShouldBe(propostaParecerDto.Descricao);
            propostaParecer.PropostaId.ShouldBe(proposta.Id);
            propostaParecer.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta parecer - Alterar parecer")]
        public async Task Deve_alterar_proposta_parecer()
        {
            // arrange
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var inserirPropostaParecer = PropostaParecerMock.GerarPropostaParecer();
            inserirPropostaParecer.PropostaId = proposta.Id;
            await InserirNaBase(inserirPropostaParecer);
            
            var alterarPropostaParecer = PropostaSalvarMock.GerarParecer();
            alterarPropostaParecer.PropostaId = proposta.Id;
            alterarPropostaParecer.Id = inserirPropostaParecer.Id;
            
            // act
            var retorno = await useCase.Executar(alterarPropostaParecer);

            // assert
            retorno.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaParecer>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Descricao.ShouldBe(alterarPropostaParecer.Descricao);
            propostaParecer.PropostaId.ShouldBe(proposta.Id);
            propostaParecer.Excluido.ShouldBeFalse();
        }
    }
}