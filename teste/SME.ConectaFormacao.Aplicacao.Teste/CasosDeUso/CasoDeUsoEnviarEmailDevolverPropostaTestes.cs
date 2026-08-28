using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Email;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Email
{
    public class CasoDeUsoEnviarEmailDevolverPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoEnviarEmailDevolverProposta _sut;

        public CasoDeUsoEnviarEmailDevolverPropostaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _sut = mocker.CreateInstance<CasoDeUsoEnviarEmailDevolverProposta>();

            // Garantir que o arquivo esperado exista para evitar FileNotFoundException
            var diretorioWwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ModelosEmail");
            if (!Directory.Exists(diretorioWwwroot))
                Directory.CreateDirectory(diretorioWwwroot);

            var caminhoArquivo = Path.Combine(diretorioWwwroot, "DevolverProposta.txt");
            if (!File.Exists(caminhoArquivo))
                File.WriteAllText(caminhoArquivo, "Texto: #TEXTO, Motivo: #MOTIVO");
        }

        [Fact]
        public async Task DadoMensagemRabbitNulaOuInvalida_QuandoExecutar_EntaoLancaNegocioException()
        {
            // Arrange
            var mensagemRabbit = new MensagemRabbit("null");

            // Act
            var acao = async () => await _sut.Executar(mensagemRabbit);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.WithMessage(MensagemNegocio.DADOS_ENVIO_EMAIL_NAO_LOCALIZADO);
        }

        [Fact]
        public async Task DadoEmailSemArroba_QuandoExecutar_EntaoNaoEnviaEmailERetornaVerdadeiro()
        {
            // Arrange
            var dto = new
            {
                EmailDestinatario = "emailinvalido"
            };
            var mensagemRabbit = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoEmailNuloOuVazio_QuandoExecutar_EntaoNaoEnviaEmailERetornaVerdadeiro()
        {
            // Arrange
            var dto = new
            {
                EmailDestinatario = ""
            };
            var mensagemRabbit = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoEmailValido_QuandoExecutar_EntaoDeveEnviarEmailERetornaVerdadeiro()
        {
            // Arrange
            var dto = new
            {
                NomeDestinatario = "Destinatario Teste",
                EmailDestinatario = "teste@teste.com",
                Titulo = "Titulo Teste",
                Texto = "TextoTeste",
                Motivo = "MotivoTeste"
            };
            var mensagemRabbit = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagemRabbit);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<EnviarEmailCommand>(c =>
                c.NomeDestinatario == "Destinatario Teste" &&
                c.EmailDestinatario == "teste@teste.com" &&
                c.Assunto == "Titulo Teste" &&
                c.MensagemHtml == "Texto: TextoTeste, Motivo: MotivoTeste"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
