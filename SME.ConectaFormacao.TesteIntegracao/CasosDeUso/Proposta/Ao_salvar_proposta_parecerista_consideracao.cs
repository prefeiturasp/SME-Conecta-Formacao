using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_proposta_parecerista_consideracao : TestePropostaBase
    {
        public Ao_salvar_proposta_parecerista_consideracao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta parecerista consideração - Cadastrar uma consideração")]
        public async Task Deve_cadastrar_proposta_parecerista_consideracao()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString());
            await InserirUsuario("1", "Parecerista1");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaPareceristaConsideracao>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerista);
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            var propostaParecerDto = PropostaSalvarMock.GerarPareceristaConsideracaoCadastro();
            propostaParecerDto.PropostaPareceristaId = 1;
            
            // act
            var retorno = await useCase.Executar(propostaParecerDto);

            // assert
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaPareceristaConsideracao>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Campo.ShouldBe(propostaParecerDto.Campo);
            propostaParecer.Descricao.ShouldBe(propostaParecerDto.Descricao);
            propostaParecer.PropostaPareceristaId.ShouldBe(1);
            propostaParecer.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta parecer - Alterar parecer")]
        public async Task Deve_alterar_proposta_parecerista_consideracao()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString());
            await InserirUsuario("1", "Parecerista1");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaPareceristaConsideracao>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerista);
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            var inserirPropostaParecer = PropostaPareceristaConsideracaoMock.GerarPropostasPareceristasConsideracoes(1);
            await InserirConsideracoesDosPareceristas(inserirPropostaParecer, "1",proposta.Id);
            
            var alterarPropostaParecer = PropostaSalvarMock.GerarPareceristaConsideracaoCadastro();
            alterarPropostaParecer.PropostaPareceristaId = proposta.Id;
            alterarPropostaParecer.Id = inserirPropostaParecer.FirstOrDefault().Id;
            
            // act
            var retorno = await useCase.Executar(alterarPropostaParecer);

            // assert
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var propostaParecer = ObterTodos<PropostaPareceristaConsideracao>().FirstOrDefault();
            propostaParecer.Id.ShouldBe(1);
            propostaParecer.Descricao.ShouldBe(alterarPropostaParecer.Descricao);
            propostaParecer.PropostaPareceristaId.ShouldBe(1);
            propostaParecer.Excluido.ShouldBeFalse();
        }

        private async Task InserirConsideracoesDosPareceristas(IEnumerable<PropostaPareceristaConsideracao> inserirConsideracoesDosParecistas, string login, long propostaId, CampoParecer campoParecer = CampoParecer.FormacaoHomologada)
        {
            foreach (var consideracaoParecista in inserirConsideracoesDosParecistas)
            {
                consideracaoParecista.PropostaPareceristaId = propostaId;
                consideracaoParecista.CriadoLogin = login;
                consideracaoParecista.Campo = campoParecer;
                await InserirNaBase(consideracaoParecista);
            }
        }
    }
}