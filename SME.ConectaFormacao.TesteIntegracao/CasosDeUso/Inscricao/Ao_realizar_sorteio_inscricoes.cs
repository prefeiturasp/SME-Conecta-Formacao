using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_realizar_sorteio_inscricoes : TestePropostaBase
    {
        public Ao_realizar_sorteio_inscricoes(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Inscrição - Deve sortear inscrição")]
        public async Task Deve_sortear_inscricao()
        {
            // arrange
            var propostas = await InserirNaBaseProposta(1, criterioValidacaoInscricaopermiteSorteio: true);
            var proposta = propostas.FirstOrDefault();

            var usuarios = UsuarioMock.GerarUsuarios(quantidade: 35);
            await InserirNaBase(usuarios);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricoes(usuarios, propostaTurma.Id, Dominio.Enumerados.SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vaga);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSortearInscricoes>();

            // act
            await casoDeUso.Executar(propostaTurma.Id);

            // assert 
            var inscricoesDepois = ObterTodos<Dominio.Entidades.Inscricao>();

            inscricoesDepois.Count(c => c.PropostaTurmaId == propostaTurma.Id && c.Situacao.EhConfirmada()).ShouldBe(proposta.QuantidadeVagasTurma.GetValueOrDefault());

            var canceladas = usuarios.Count() - proposta.QuantidadeVagasTurma.GetValueOrDefault();
            inscricoesDepois.Count(c => c.PropostaTurmaId == propostaTurma.Id && c.Situacao.EhCancelada()).ShouldBe(canceladas);
        }
    }
}
