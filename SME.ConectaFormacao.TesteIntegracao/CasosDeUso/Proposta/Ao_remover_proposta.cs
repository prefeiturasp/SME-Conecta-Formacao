using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_remover_proposta : TestePropostaBase
    {
        public Ao_remover_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve remover por id válido")]
        public async Task Deve_remover_por_id_valido()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverProposta>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert 
            retorno.ShouldBeTrue();

            var propostaBase = ObterTodos<Dominio.Entidades.Proposta>().First();
            propostaBase.Excluido.ShouldBeTrue();

            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>();
            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
                criterioValidacaoInscricao.Excluido.ShouldBeTrue();

            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>();
            foreach (var vagaRemanecente in vagasRemanecentes)
                vagaRemanecente.Excluido.ShouldBeTrue();

            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>();
            foreach (var funcaoEspecifica in funcoesEspecificas)
                funcaoEspecifica.Excluido.ShouldBeTrue();

            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>();
            foreach (var publicoAlvo in publicosAlvo)
                publicoAlvo.Excluido.ShouldBeTrue();

            var encontros = ObterTodos<PropostaEncontro>();
            foreach (var encontro in encontros)
                encontro.Excluido.ShouldBeTrue();

            var encontroTurmas = ObterTodos<PropostaEncontroTurma>();
            foreach (var turma in encontroTurmas)
                turma.Excluido.ShouldBeTrue();

            var encontroDatas = ObterTodos<PropostaEncontroData>();
            foreach (var data in encontroDatas)
                data.Excluido.ShouldBeTrue();

            var propostaPalavraChaves = ObterTodos<PropostaPalavraChave>();
            foreach (var propostaPalavraChave in propostaPalavraChaves)
                propostaPalavraChave.Excluido.ShouldBeTrue();
            
            var propostaModalidades = ObterTodos<PropostaModalidade>();
            foreach (var propostaModalidade in propostaModalidades)
                propostaModalidade.Excluido.ShouldBeTrue();
            
            var propostaAnosTurmas = ObterTodos<PropostaAnoTurma>();
            foreach (var propostaAnoTurma in propostaAnosTurmas)
                propostaAnoTurma.Excluido.ShouldBeTrue();
            
            var propostaComponentesCurriculares = ObterTodos<PropostaComponenteCurricular>();
            foreach (var propostaComponenteCurricular in propostaComponentesCurriculares)
                propostaComponenteCurricular.Excluido.ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção ao remover por id inválido")]
        public async Task Deve_retornar_excecao_ao_remover_por_id_invalido()
        {
            // arrange 
            var idAleatorio = PropostaSalvarMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverProposta>();

            // act 
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(idAleatorio));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }
    }
}
