using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_cancelar_inscricao : TestePropostaBase
    {
        public Ao_cancelar_inscricao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Inscrição - Deve cancelar inscrição confirmada")]
        public async Task Deve_cancelar_inscricao_confirmada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, Dominio.Enumerados.SituacaoInscricao.Confirmada);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id, inscricao.Id);
            await InserirNaBase(vaga);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoCancelarInscricao>();

            // act
            await casoDeUso.Executar(inscricao.Id);

            // assert 
            var inscricaoDepois = ObterPorId<Dominio.Entidades.Inscricao, long>(inscricao.Id);

            inscricaoDepois.Situacao.ShouldBe(Dominio.Enumerados.SituacaoInscricao.Cancelada);

            var vagas = ObterTodos<PropostaTurmaVaga>();
            vagas.FirstOrDefault().InscricaoId.ShouldBeNull();
        }

        [Fact(DisplayName = "Inscrição - Deve cancelar inscrição não confirmada")]
        public async Task Deve_cancelar_inscricao_nao_confirmada()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.Sim);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, Dominio.Enumerados.SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoCancelarInscricao>();

            // act
            await casoDeUso.Executar(inscricao.Id);

            // assert 
            var inscricaoDepois = ObterPorId<Dominio.Entidades.Inscricao, long>(inscricao.Id);

            inscricaoDepois.Situacao.ShouldBe(Dominio.Enumerados.SituacaoInscricao.Cancelada);
        }
    }
}
