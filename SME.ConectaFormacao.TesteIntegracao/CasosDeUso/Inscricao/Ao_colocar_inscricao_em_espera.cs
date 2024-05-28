using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_colocar_inscricao_em_espera : TestePropostaBase
    {
        public Ao_colocar_inscricao_em_espera(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Inscrição - Deve mover inscrições em espera com sucesso")]
        public async Task Deve_mover_em_espera_inscricao_com_sucesso()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.Sim);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, Dominio.Enumerados.SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id);
            await InserirNaBase(vaga);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEmEsperaInscricoes>();

            // act
            var retorno = await casoDeUso.Executar(new long[] { inscricao.Id });

            // assert 

            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_SUCESSO);
        }

        [Fact(DisplayName = "Inscrição - Deve mover em espera inscrições com inconsistencias")]
        public async Task Deve_mover_em_espera_inscricao_com_inconsistencias()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.Sim);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, Dominio.Enumerados.SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id);
            await InserirNaBase(vaga);

            var usuarioSemVaga = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioSemVaga);

            var propostaTurmaSemVaga = proposta.Turmas.LastOrDefault();

            var inscricaoSemVaga = InscricaoMock.GerarInscricao(usuarioSemVaga.Id, propostaTurmaSemVaga.Id, Dominio.Enumerados.SituacaoInscricao.Cancelada);
            await InserirNaBase(inscricaoSemVaga);
            
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEmEsperaInscricoes>();

            // act
            var retorno = await casoDeUso.Executar(new long[] { inscricao.Id, inscricaoSemVaga.Id });

            // assert 
            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_INCONSISTENCIAS);
        }
    }
}
