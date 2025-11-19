using Shouldly;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_colocar_inscricao_em_espera : TestePropostaBase
    {
        public Ao_colocar_inscricao_em_espera(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            MensagemQueueSpy.Limpar();
        }

        [Fact(DisplayName = "Inscrição - Deve mover inscrições em espera com sucesso e enviar email")]
        public async Task Deve_mover_em_espera_inscricao_com_sucesso_e_validar_email()
        {
            // Arrange
            var nomeFormacao = "Formação Integrada de C#";
            var nomeUsuario = "João Cursista da Silva";
            var usuario = UsuarioMock.GerarUsuario(nome: nomeUsuario);
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.Sim, nomeFormacao: nomeFormacao);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, Dominio.Enumerados.SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id);
            await InserirNaBase(vaga);

            CriarClaimUsuario(Perfis.ADMIN_DF.ToString()); // Simular usuário logado
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEmEsperaInscricoes>();

            // Act
            var retorno = await casoDeUso.Executar([inscricao.Id]);

            // Assert 
            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICOES_EM_ESPERA_COM_SUCESSO);
            MensagemQueueSpy.MensagensEnviadas.Count.ShouldBe(1);

            var commandMensagem = MensagemQueueSpy.MensagensEnviadas.FirstOrDefault()?.ShouldBeOfType<PublicarNaFilaRabbitCommand>();
            commandMensagem.ShouldNotBeNull();
            var emailDto = commandMensagem!.Filtros.ShouldBeOfType<EnviarEmailDto>();
            emailDto.ShouldNotBeNull();
            emailDto.Titulo.ShouldContain("Inscrição em lista de espera");
            emailDto.Titulo.ShouldContain(nomeFormacao);
            emailDto.Texto.ShouldNotContain("{NOME_DESTINATARIO}"); // Verifica que o placeholder foi substituído
            emailDto.Texto.ShouldNotContain("{NOME_FORMACAO}");      // Verifica que o placeholder foi substituído

            emailDto.Texto.ShouldContain(nomeUsuario); // Verifica o nome do cursista
            emailDto.Texto.ShouldContain(nomeFormacao); // Verifica o nome da formação
            emailDto.Texto.ShouldContain("lista de espera"); // Verifica o texto específico do cenário
            emailDto.Texto.ShouldContain("clicando <a href=\"https://conectaformacao.sme.prefeitura.sp.gov.br/area-logada/minhas-inscricoes\""); // Verifica o link de acompanhamento
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
