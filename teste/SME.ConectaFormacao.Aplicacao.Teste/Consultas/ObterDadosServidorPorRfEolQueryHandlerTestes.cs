using Moq;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;
using System.Runtime.CompilerServices;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterDadosServidorPorRfEolQueryHandlerTestes
    {
        private readonly Mock<IServicoEol> servicoEol;
        private readonly ObterDadosServidorPorRfEolQueryHandler handler;

        public ObterDadosServidorPorRfEolQueryHandlerTestes()
        {
            servicoEol = new Mock<IServicoEol>(MockBehavior.Strict);
            handler = new ObterDadosServidorPorRfEolQueryHandler(servicoEol.Object);
        }

        [Fact]
        public async Task Handle_Deve_encaminhar_rf_e_retornar_dados_do_servidor()
        {
            const string rfServidor = "1234567";
            var request = new ObterDadosServidorPorRfEolQuery(rfServidor);
            var usuarioEsperado = CriarUsuarioEolDto();

            servicoEol
                .Setup(x => x.ObterDadosServidorPorRfEol(rfServidor))
                .ReturnsAsync(usuarioEsperado);

            var resultado = await handler.Handle(request, CancellationToken.None);

            Assert.Same(usuarioEsperado, resultado);
            servicoEol.Verify(x => x.ObterDadosServidorPorRfEol(rfServidor), Times.Once);
            servicoEol.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_servico_nao_encontrar_servidor_Deve_retornar_nulo()
        {
            const string rfServidor = "7654321";
            var request = new ObterDadosServidorPorRfEolQuery(rfServidor);

            servicoEol
                .Setup(x => x.ObterDadosServidorPorRfEol(rfServidor))
                .ReturnsAsync((UsuarioEolDto?)null);

            var resultado = await handler.Handle(request, CancellationToken.None);

            Assert.Null(resultado);
            servicoEol.Verify(x => x.ObterDadosServidorPorRfEol(rfServidor), Times.Once);
            servicoEol.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_servico_lancar_excecao_Deve_propagar_excecao()
        {
            const string rfServidor = "1234567";
            var request = new ObterDadosServidorPorRfEolQuery(rfServidor);
            var excecaoEsperada = new InvalidOperationException("Falha ao consultar o EOL");

            servicoEol
                .Setup(x => x.ObterDadosServidorPorRfEol(rfServidor))
                .ThrowsAsync(excecaoEsperada);

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(
                () => handler.Handle(request, CancellationToken.None));

            Assert.Same(excecaoEsperada, excecao);
            servicoEol.Verify(x => x.ObterDadosServidorPorRfEol(rfServidor), Times.Once);
            servicoEol.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_Quando_token_estiver_cancelado_Deve_consultar_servico_normalmente()
        {
            const string rfServidor = "1234567";
            var request = new ObterDadosServidorPorRfEolQuery(rfServidor);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            servicoEol
                .Setup(x => x.ObterDadosServidorPorRfEol(rfServidor))
                .ReturnsAsync((UsuarioEolDto?)null);

            var resultado = await handler.Handle(request, cancellationTokenSource.Token);

            Assert.Null(resultado);
            servicoEol.Verify(x => x.ObterDadosServidorPorRfEol(rfServidor), Times.Once);
            servicoEol.VerifyNoOtherCalls();
        }

        private static UsuarioEolDto CriarUsuarioEolDto()
        {
            return (UsuarioEolDto)RuntimeHelpers.GetUninitializedObject(typeof(UsuarioEolDto));
        }
    }
}
