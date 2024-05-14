using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
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
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnalisePeloParecerista);
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            var propostaPareceristaConsideracaoCadastroDto = PropostaSalvarMock.GerarPareceristaConsideracaoCadastro();
            propostaPareceristaConsideracaoCadastroDto.PropostaId = 1;
            
            // act
            var retorno = await useCase.Executar(propostaPareceristaConsideracaoCadastroDto);

            // assert
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var consideracaoParecista = ObterTodos<PropostaPareceristaConsideracao>().FirstOrDefault();
            consideracaoParecista.Id.ShouldBe(1);
            consideracaoParecista.Campo.ShouldBe(propostaPareceristaConsideracaoCadastroDto.Campo);
            consideracaoParecista.Descricao.ShouldBe(propostaPareceristaConsideracaoCadastroDto.Descricao);
            consideracaoParecista.PropostaPareceristaId.ShouldBe(1);
            consideracaoParecista.Excluido.ShouldBeFalse();
        }
        
        [Fact(DisplayName = "Proposta parecer - Alterar parecer")]
        public async Task Deve_alterar_proposta_parecerista_consideracao()
        {
            // arrange
            CriarClaimUsuario(Dominio.Constantes.Perfis.PARECERISTA.ToString());
            await InserirUsuario("1", "Parecerista1");
            
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaPareceristaConsideracao>();
            
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnalisePeloParecerista);
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1","Parecerista1"));
            
            var inserirConsideracaoDoParecista = PropostaPareceristaConsideracaoMock.GerarPropostasPareceristasConsideracoes(1);
            await InserirConsideracoesDosPareceristas(inserirConsideracaoDoParecista, "1",proposta.Id);
            
            var alterarConsideracaoDoParecista = PropostaSalvarMock.GerarPareceristaConsideracaoCadastro();
            alterarConsideracaoDoParecista.PropostaId = proposta.Id;
            alterarConsideracaoDoParecista.Id = inserirConsideracaoDoParecista.FirstOrDefault().Id;
            
            // act
            var retorno = await useCase.Executar(alterarConsideracaoDoParecista);

            // assert
            retorno.EntidadeId.ShouldBeGreaterThan(0);
            var pareceristaConsideracao = ObterTodos<PropostaPareceristaConsideracao>().FirstOrDefault();
            pareceristaConsideracao.Id.ShouldBe(1);
            pareceristaConsideracao.Descricao.ShouldBe(alterarConsideracaoDoParecista.Descricao);
            pareceristaConsideracao.PropostaPareceristaId.ShouldBe(1);
            pareceristaConsideracao.Excluido.ShouldBeFalse();
        }

        private async Task InserirConsideracoesDosPareceristas(IEnumerable<PropostaPareceristaConsideracao> inserirConsideracoesDosParecistas, string login, long propostaId, CampoConsideracao campoConsideracao = CampoConsideracao.FormacaoHomologada)
        {
            foreach (var consideracaoParecista in inserirConsideracoesDosParecistas)
            {
                consideracaoParecista.PropostaPareceristaId = propostaId;
                consideracaoParecista.CriadoLogin = login;
                consideracaoParecista.Campo = campoConsideracao;
                await InserirNaBase(consideracaoParecista);
            }
        }
    }
}