using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricao.ReativarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricao
{
    public class CasoDeUsoReativarInscricoesTeste
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoReativarInscricoes _casoDeUso;

        public CasoDeUsoReativarInscricoesTeste()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoReativarInscricoes(_mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_Retornar_Sucesso_Quando_Reativacao_Valida()
        {
            // Arrange
            var ids = new long[] { 1 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(true);

            // Act
            var result = await _casoDeUso.Executar(ids);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(MensagemNegocio.INSCRICOES_REATIVACAO_CONFIRMADAS_COM_SUCESSO, result.Mensagem);
        }

        [Fact]
        public async Task Executar_Deve_Lancar_Excecao_Quando_Ids_Nulos_Ou_Vazios()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(null));
            await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(Array.Empty<long>()));
        }

        [Fact]
        public async Task Executar_Deve_Lancar_Excecao_Com_Erros_Quando_Um_Item_Falhar()
        {
            // Arrange
            var ids = new long[] { 1 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA));

            // Act
            var ex = await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(ids));

            // Assert
            Assert.Contains(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, ex.Mensagens);
        }

        [Fact]
        public async Task Executar_Deve_Retornar_Inconsistencias_Quando_Alguns_Itens_Falharem()
        {
            // Arrange
            var ids = new long[] { 1, 2 };
            _mediatorMock.SetupSequence(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA))
                         .ReturnsAsync(true);

            // Act
            var result = await _casoDeUso.Executar(ids);

            // Assert
            Assert.True(result.Sucesso);
            Assert.Equal(MensagemNegocio.INSCRICOES_REATIVADAS_COM_INCONSISTENCIAS, result.Mensagem);
        }

        [Theory]
        [InlineData(MensagemNegocio.INSCRICAO_CARGO_NAO_PERMITIDO)]
        [InlineData(MensagemNegocio.INSCRICAO_DRE_NAO_PERMITIDA)]
        [InlineData(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA)]
        public async Task Executar_Deve_Propagar_Erros_De_Validacao(string mensagemErro)
        {
            // Arrange
            var ids = new long[] { 1 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(new NegocioException(mensagemErro));

            // Act
            var ex = await Assert.ThrowsAsync<NegocioException>(() => _casoDeUso.Executar(ids));

            // Assert
            Assert.Contains(mensagemErro, ex.Mensagens);
        }

        [Fact]
        public async Task Executar_Deve_Processar_Ids_Em_Ordem_Crescente()
        {
            // Arrange
            var ids = new long[] { 3, 1, 2 };
            var idsProcessados = new System.Collections.Concurrent.ConcurrentBag<long>();

            _mediatorMock.Setup(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                        .Returns<ReativarInscricaoCommand, CancellationToken>((cmd, _) =>
                        {
                            idsProcessados.Add(cmd.Id);
                            return Task.FromResult(true);
                        });

            // Act
            await _casoDeUso.Executar(ids);

            // Assert
            Assert.Equal(new long[] { 1, 2, 3 }, idsProcessados.OrderBy(x => x));
        }

        [Fact]
        public async Task Executar_Deve_Agregar_Todos_Erros_Quando_Multiplas_Inscricoes_Falharem()
        {
            // Arrange
            var ids = new long[] { 1, 2, 3 };
            var mensagensErro = new[]
            {
                MensagemNegocio.INSCRICAO_CARGO_NAO_PERMITIDO,
                MensagemNegocio.INSCRICAO_DRE_NAO_PERMITIDA
            };

            _mediatorMock.SetupSequence(m => m.Send(It.IsAny<ReativarInscricaoCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new NegocioException(mensagensErro[0]))
                         .ThrowsAsync(new NegocioException(mensagensErro[1]))
                         .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(ids);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.Equal(MensagemNegocio.INSCRICOES_REATIVADAS_COM_INCONSISTENCIAS, resultado.Mensagem);
        }
    }
}