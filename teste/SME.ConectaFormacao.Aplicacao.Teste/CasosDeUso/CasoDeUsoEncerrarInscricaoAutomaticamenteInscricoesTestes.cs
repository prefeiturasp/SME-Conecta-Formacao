using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoesTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes _sut;
        private readonly Faker _faker;

        public CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoesTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();
            
            _sut = mocker.CreateInstance<CasoDeUsoEncerrarInscricaoAutomaticamenteInscricoes>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoMensagemNula_QuandoChamarExecutar_EntaoDeveRetornarTrueENaoConsultarInscricoes()
        {
            // Arrange
            var param = new MensagemRabbit { Mensagem = null };

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _repositorioInscricaoMock.Verify(
                r => r.ObterInscricoesUsuariosInternosPorPropostasTurmasId(It.IsAny<long[]>(), It.IsAny<SituacaoInscricao?[]>()), 
                Times.Never);
        }

        [Fact]
        public async Task DadoMensagemVazia_QuandoChamarExecutar_EntaoDeveRetornarTrueENaoConsultarInscricoes()
        {
            // Arrange
            var param = new MensagemRabbit(" ");

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _repositorioInscricaoMock.Verify(
                r => r.ObterInscricoesUsuariosInternosPorPropostasTurmasId(It.IsAny<long[]>(), It.IsAny<SituacaoInscricao?[]>()), 
                Times.Never);
        }

        [Fact]
        public async Task DadoNenhumaInscricaoEncontrada_QuandoChamarExecutar_EntaoDeveRetornarTrueENaoPublicarNaFila()
        {
            // Arrange
            var turmaId = _faker.Random.Long(1, 1000);
            var param = new MensagemRabbit(turmaId.ObjetoParaJson());

            _repositorioInscricaoMock.Setup(r => r.ObterInscricoesUsuariosInternosPorPropostasTurmasId(
                    It.Is<long[]>(ids => ids.Contains(turmaId)),
                    SituacaoInscricao.Confirmada, SituacaoInscricao.AguardandoAnalise, SituacaoInscricao.Enviada, SituacaoInscricao.EmEspera))
                .ReturnsAsync(new List<InscricaoUsuarioInternoDto>());

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _repositorioInscricaoMock.Verify(r => r.ObterInscricoesUsuariosInternosPorPropostasTurmasId(It.IsAny<long[]>(), It.IsAny<SituacaoInscricao?[]>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoInscricoesEncontradas_QuandoChamarExecutar_EntaoDevePublicarNaFilaERetornarTrue()
        {
            // Arrange
            var turmaId = _faker.Random.Long(1, 1000);
            var param = new MensagemRabbit(turmaId.ObjetoParaJson());
            var inscricoes = new List<InscricaoUsuarioInternoDto>
            {
                new InscricaoUsuarioInternoDto { InscricaoId = _faker.Random.Long(1, 1000), UsuarioId = _faker.Random.Long(1, 1000) }
            };

            _repositorioInscricaoMock.Setup(r => r.ObterInscricoesUsuariosInternosPorPropostasTurmasId(
                    It.Is<long[]>(ids => ids.Contains(turmaId)),
                    SituacaoInscricao.Confirmada, SituacaoInscricao.AguardandoAnalise, SituacaoInscricao.Enviada, SituacaoInscricao.EmEspera))
                .ReturnsAsync(inscricoes);

            _mediatorMock.Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(param);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => 
                c.Rota == RotasRabbit.EncerrarInscricaoAutomaticamenteUsuarios &&
                c.Filtros == inscricoes &&
                c.Usuario.Login == "Sistema" &&
                c.Usuario.Nome == "Sistema"
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
