using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta;
using SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarNumeroHomologacaoProposta;
using SME.ConectaFormacao.Aplicacao.Consultas.Propostas.ObterSePropostaPossuiCodaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoSalvarNumeroHomologacaoPropostaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSalvarNumeroHomologacaoProposta _sut;

        public CasoDeUsoSalvarNumeroHomologacaoPropostaTestes()
        {
            var autoMocker = new AutoMocker();
            _mediatorMock = autoMocker.GetMock<IMediator>();
            _sut = autoMocker.CreateInstance<CasoDeUsoSalvarNumeroHomologacaoProposta>();
        }

        [Fact]
        public async Task DadoPropostaInexistente_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            const long propostaId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Proposta)null!);

            // Act
            var act = async () => await _sut.Executar(propostaId, new PropostaNumeroHomologacaoDto { NumeroHomologacao = 12345 });

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoPropostaExcluida_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            const long propostaId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Proposta { Id = propostaId, Excluido = true });

            // Act
            var act = async () => await _sut.Executar(propostaId, new PropostaNumeroHomologacaoDto { NumeroHomologacao = 12345 });

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);
        }

        [Fact]
        public async Task DadoUsuarioNaoEhAdmin_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            const long propostaId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Proposta { Id = propostaId, Excluido = false });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Guid.NewGuid()); // Perfil diferente de ADMIN_DF

            // Act
            var act = async () => await _sut.Executar(propostaId, new PropostaNumeroHomologacaoDto { NumeroHomologacao = 12345 });

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.USUARIO_SEM_PERMISSAO_PARA_ALTERAR_NUMERO_HOMOLOGACAO);
        }

        [Fact]
        public async Task DadoPropostaPossuiCodaf_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            const long propostaId = 1;
            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Proposta { Id = propostaId, Excluido = false });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Perfis.ADMIN_DF);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterSePropostaPossuiCodafQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _sut.Executar(propostaId, new PropostaNumeroHomologacaoDto { NumeroHomologacao = 12345 });

            // Assert
            var excecao = await act.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.PROPOSTA_POSSUI_CODAF_NAO_PERMITE_ALTERAR_NUMERO_HOMOLOGACAO);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarNumeroHomologacaoPropostaCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoCriteriosValidos_QuandoExecutar_EntaoDeveSalvarNumeroHomologacaoERetornarTrue()
        {
            // Arrange
            const long propostaId = 1;
            const long numeroHomologacao = 98765;

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Proposta { Id = propostaId, Excluido = false });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGrupoUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Perfis.ADMIN_DF);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterSePropostaPossuiCodafQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarNumeroHomologacaoPropostaCommand>(c => c.PropostaId == propostaId && c.NumeroHomologacao == numeroHomologacao), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(propostaId, new PropostaNumeroHomologacaoDto { NumeroHomologacao = numeroHomologacao });

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarNumeroHomologacaoPropostaCommand>(c => c.PropostaId == propostaId && c.NumeroHomologacao == numeroHomologacao), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
