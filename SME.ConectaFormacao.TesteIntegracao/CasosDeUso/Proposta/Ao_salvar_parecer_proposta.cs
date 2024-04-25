using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
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
            retorno.EntidadeId.ShouldBeGreaterThan(0);
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
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaParecer>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Descricao.ShouldBe(alterarPropostaParecer.Descricao);
            propostaParecer.PropostaId.ShouldBe(proposta.Id);
            propostaParecer.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta parecer - Deve permitir ao cursista inserir parecer")]
        public async Task Deve_permitir_ao_cursista_inserir_parecer()
        {
            // arrange
            // CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA);
            var useCase = ObterCasoDeUso<ICasoDeUsoObterPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            // act
            var filtro = new PropostaParecerFiltroDTO()
            {
                Campo = CampoParecer.FormacaoHomologada,
                PropostaId = 1
            };
            
            var retorno = await useCase.Executar(filtro);

            // assert
            retorno.PodeInserir.ShouldBeTrue();
        }
    }
}