using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_obter_turmas_com_vaga : TestePropostaBase
    {
        public Ao_obter_turmas_com_vaga(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Inscrição - Deve obter Turmas com vaga proposta não homologada")]
        public async Task Deve_obter_turmas_com_vaga_proposta_nao_homologada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTurmasInscricao>();

            // act
            var turmas = await casoDeUso.Executar(proposta.Id);

            // assert 
            turmas.ShouldNotBeEmpty();

            turmas.Count().ShouldBe(proposta.Turmas.Count());
        }

        [Fact(DisplayName = "Inscrição - Deve retornar excecao Turmas sem vaga proposta não homologada")]
        public async Task Deve_retornar_excecao_turma_sem_vaga_proposta_nao_homologada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTurmasInscricao>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(proposta.Id));

            // assert 
            excecao.Mensagens.Contains(MensagemNegocio.NENHUMA_TURMA_COM_VAGA_DISPONIVEL).ShouldBeTrue();
        }

        [Fact(DisplayName = "Inscrição - Deve obter Turmas proposta homologada")]
        public async Task Deve_obter_turmas_proposta_homologada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.Sim);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterTurmasInscricao>();

            // act
            var turmas = await casoDeUso.Executar(proposta.Id);

            // assert 
            turmas.ShouldNotBeEmpty();

            turmas.Count().ShouldBe(proposta.Turmas.Count());
        }
    }
}
