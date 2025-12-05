using Shouldly;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_confirmar_inscricoes : TestePropostaBase
    {
        public Ao_confirmar_inscricoes(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            MensagemQueueSpy.Limpar();
        }

        [Fact(DisplayName = "Inscrição - Deve confirmar inscrições com sucesso e enviar email")]
        public async Task Deve_confirmar_inscricao_com_sucesso_e_validar_email()
        {
            // arrange
            var nomeFormacao = "O futuro da IA - Parte 1";
            var nomeUsuario = "João Cursista da Silva Neto";
            var usuario = UsuarioMock.GerarUsuario(nome: nomeUsuario);
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.Sim, nomeFormacao: nomeFormacao);

            var propostaTurma = proposta.Turmas.First();

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id);
            await InserirNaBase(vaga);

            CriarClaimUsuario(Perfis.ADMIN_DF.ToString()); // Simular usuário logado
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoConfirmarInscricoes>();

            // act
            var retorno = await casoDeUso.Executar([inscricao.Id]);

            // assert 

            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICOES_CONFIRMADAS_COM_SUCESSO);
            MensagemQueueSpy.MensagensEnviadas.Count.ShouldBe(1);

            var commandMensagem = MensagemQueueSpy.MensagensEnviadas.FirstOrDefault()?.ShouldBeOfType<PublicarNaFilaRabbitCommand>();
            commandMensagem.ShouldNotBeNull();
            var emailDto = commandMensagem!.Filtros.ShouldBeOfType<EnviarEmailDto>();
            emailDto.ShouldNotBeNull();
            emailDto.Titulo.ShouldContain("Inscrição confirmada | ");
            emailDto.Titulo.ShouldContain(nomeFormacao);
            emailDto.Texto.ShouldNotContain("{{NOME_DESTINATARIO}}"); // Verifica que o placeholder foi substituído
            emailDto.Texto.ShouldNotContain("{{NOME_FORMACAO}}");      // Verifica que o placeholder foi substituído

            emailDto.Texto.ShouldContain(nomeUsuario); // Verifica o nome do cursista
            emailDto.Texto.ShouldContain(nomeFormacao); // Verifica o nome da formação
            emailDto.Texto.ShouldContain("Inscrição Confirmada"); // Verifica o conteúdo do email
            emailDto.Texto.ShouldContain("clicando <a href=\"https://conectaformacao.sme.prefeitura.sp.gov.br/area-logada/minhas-inscricoes\""); // Verifica o link de acompanhamento
        }

        [Fact(DisplayName = "Inscrição - Deve confirmar inscrições com inconsistencias")]
        public async Task Deve_confirmar_inscricao_com_inconsistencias()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.Sim);

            var propostaTurma = proposta.Turmas.First();

            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma.Id, SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id);
            await InserirNaBase(vaga);

            var usuarioSemVaga = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuarioSemVaga);

            var propostaTurmaSemVaga = proposta.Turmas.Last();

            var inscricaoSemVaga = InscricaoMock.GerarInscricao(usuarioSemVaga.Id, propostaTurmaSemVaga.Id, SituacaoInscricao.AguardandoAnalise);
            await InserirNaBase(inscricaoSemVaga);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoConfirmarInscricoes>();

            // act
            var retorno = await casoDeUso.Executar([inscricao.Id, inscricaoSemVaga.Id]);

            // assert 
            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICOES_NAO_CONFIRMADAS_POR_FALTA_DE_VAGA);
        }
    }
}
