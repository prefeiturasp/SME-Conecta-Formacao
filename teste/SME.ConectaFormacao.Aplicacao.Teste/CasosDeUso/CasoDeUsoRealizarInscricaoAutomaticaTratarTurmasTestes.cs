using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratarTurmasTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoRealizarInscricaoAutomaticaTratarTurmas _sut;

        public CasoDeUsoRealizarInscricaoAutomaticaTratarTurmasTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoRealizarInscricaoAutomaticaTratarTurmas>();
        }

        [Fact]
        public async Task DadoMensagemValidaComDres_QuandoExecutar_EntaoDeveAgruparEAssociarCursistasEPublicarNaFila()
        {
            // Arrange
            var dto = new InscricaoAutomaticaTratarTurmasDTO
            {
                PropostaInscricaoAutomatica = new PropostaInscricaoAutomatica
                {
                    PropostaId = 1,
                    QuantidadeVagasTurmas = 2,
                    PropostasTurmas = new List<PropostaInscricaoAutomaticaTurma>
                    {
                        new PropostaInscricaoAutomaticaTurma { Id = 10, CodigoDre = "DRE1" }
                    },
                    TiposInscricao = new List<TipoInscricao> { TipoInscricao.Automatica }
                },
                CursistasEOL = new List<CursistaServicoEol>
                {
                    new CursistaServicoEol { Rf = "123", FuncaoDreCodigo = "DRE1", Associado = false },
                    new CursistaServicoEol { Rf = "456", CargoDreCodigo = "DRE1", Associado = false },
                    new CursistaServicoEol { Rf = "789", FuncaoDreCodigo = "DRE1", Associado = false } // Vai gerar turma nova
                }
            };

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<InserirPropostaTurmaAdicionalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(11);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<InserirPropostaTurmaAdicionalCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.RealizarInscricaoAutomaticaTratarCursistas), It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(5));
        }

        [Fact]
        public async Task DadoMensagemValidaSemDres_QuandoExecutar_EntaoDeveAgruparEAssociarCursistasEPublicarNaFila()
        {
            // Arrange
            var dto = new InscricaoAutomaticaTratarTurmasDTO
            {
                PropostaInscricaoAutomatica = new PropostaInscricaoAutomatica
                {
                    PropostaId = 2,
                    QuantidadeVagasTurmas = 1,
                    PropostasTurmas = new List<PropostaInscricaoAutomaticaTurma>
                    {
                        new PropostaInscricaoAutomaticaTurma { Id = 20, CodigoDre = "" }
                    },
                    TiposInscricao = new List<TipoInscricao> { TipoInscricao.Automatica }
                },
                CursistasEOL = new List<CursistaServicoEol>
                {
                    new CursistaServicoEol { Rf = "111", Associado = false },
                    new CursistaServicoEol { Rf = "222", Associado = false } // Vai gerar turma nova
                }
            };

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<InserirPropostaTurmaAdicionalCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(21);

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.IsAny<InserirPropostaTurmaAdicionalCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            _mocker.GetMock<IMediator>().Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c => c.Rota == RotasRabbit.RealizarInscricaoAutomaticaTratarCursistas), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
