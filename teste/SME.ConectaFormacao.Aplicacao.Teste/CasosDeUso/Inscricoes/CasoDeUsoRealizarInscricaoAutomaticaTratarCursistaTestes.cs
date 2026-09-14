using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratarCursistaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoRealizarInscricaoAutomaticaTratarCursista _sut;
        private readonly Faker _faker;

        public CasoDeUsoRealizarInscricaoAutomaticaTratarCursistaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoRealizarInscricaoAutomaticaTratarCursista>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoTurmasECursistas_QuandoExecutar_EntaoPublicaRealizarInscricaoAutomaticaIncreverCursistaParaCadaCursistaERetornaTrue()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var turma1Id = _faker.Random.Long(1, 1000);
            var turma2Id = _faker.Random.Long(1001, 2000);

            var cursista1 = new CursistaServicoEol
            {
                Rf = "1111111",
                Nome = "Cursista Um",
                Cpf = "12345678901",
                CargoCodigo = "100",
                CargoDreCodigo = "DRE1",
                CargoUeCodigo = "UE1",
                FuncaoCodigo = "200",
                FuncaoDreCodigo = "DRE1",
                FuncaoUeCodigo = "UE1",
                TipoVinculo = 1
            };

            var cursista2 = new CursistaServicoEol
            {
                Rf = "2222222",
                Nome = "Cursista Dois",
                Cpf = "98765432100",
                CargoCodigo = "101",
                CargoDreCodigo = "DRE2",
                CargoUeCodigo = "UE2",
                FuncaoCodigo = "201",
                FuncaoDreCodigo = "DRE2",
                FuncaoUeCodigo = "UE2",
                TipoVinculo = 2
            };

            var cursista3 = new CursistaServicoEol
            {
                Rf = "3333333",
                Nome = "Cursista Tres",
                Cpf = "55555555555",
                CargoCodigo = "102",
                CargoDreCodigo = "DRE3",
                CargoUeCodigo = "UE3",
                FuncaoCodigo = "202",
                FuncaoDreCodigo = "DRE3",
                FuncaoUeCodigo = "UE3",
                TipoVinculo = 1
            };

            var dto = new InserirInscricaoDTO
            {
                PropostaId = propostaId,
                InscricaoAutomaticaPropostaTurmaCursistasDTO = new List<InscricaoAutomaticaPropostaTurmaCursistasDTO>
                {
                    new InscricaoAutomaticaPropostaTurmaCursistasDTO
                    {
                        Id = turma1Id,
                        Cursistas = new List<CursistaServicoEol> { cursista1, cursista2 }
                    },
                    new InscricaoAutomaticaPropostaTurmaCursistasDTO
                    {
                        Id = turma2Id,
                        Cursistas = new List<CursistaServicoEol> { cursista3 }
                    }
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista &&
                c.Filtros is InscricaoAutomaticaDTO &&
                ((InscricaoAutomaticaDTO)c.Filtros).UsuarioRf == cursista1.Rf &&
                ((InscricaoAutomaticaDTO)c.Filtros).UsuarioNome == cursista1.Nome &&
                ((InscricaoAutomaticaDTO)c.Filtros).UsuarioCpf == cursista1.Cpf &&
                ((InscricaoAutomaticaDTO)c.Filtros).PropostaId == propostaId &&
                ((InscricaoAutomaticaDTO)c.Filtros).PropostaTurmaId == turma1Id &&
                ((InscricaoAutomaticaDTO)c.Filtros).CargoCodigo == cursista1.CargoCodigo &&
                ((InscricaoAutomaticaDTO)c.Filtros).TipoVinculo == cursista1.TipoVinculo
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista &&
                c.Filtros is InscricaoAutomaticaDTO &&
                ((InscricaoAutomaticaDTO)c.Filtros).UsuarioRf == cursista2.Rf &&
                ((InscricaoAutomaticaDTO)c.Filtros).PropostaTurmaId == turma1Id
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista &&
                c.Filtros is InscricaoAutomaticaDTO &&
                ((InscricaoAutomaticaDTO)c.Filtros).UsuarioRf == cursista3.Rf &&
                ((InscricaoAutomaticaDTO)c.Filtros).PropostaTurmaId == turma2Id
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.Is<PublicarNaFilaRabbitCommand>(c =>
                c.Rota == RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista
            ), It.IsAny<CancellationToken>()), Times.Exactly(3));
        }

        [Fact]
        public async Task DadoCursistasVazio_QuandoExecutar_EntaoNaoPublicaNaFilaERetornaTrue()
        {
            // Arrange
            var dto = new InserirInscricaoDTO
            {
                PropostaId = _faker.Random.Long(1, 1000),
                InscricaoAutomaticaPropostaTurmaCursistasDTO = new List<InscricaoAutomaticaPropostaTurmaCursistasDTO>
                {
                    new InscricaoAutomaticaPropostaTurmaCursistasDTO
                    {
                        Id = _faker.Random.Long(1, 1000),
                        Cursistas = new List<CursistaServicoEol>()
                    }
                }
            };

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoInscricaoSemTurmas_QuandoExecutar_EntaoNaoPublicaNaFilaERetornaTrue()
        {
            // Arrange
            var dto = new InserirInscricaoDTO
            {
                PropostaId = _faker.Random.Long(1, 1000),
                InscricaoAutomaticaPropostaTurmaCursistasDTO = new List<InscricaoAutomaticaPropostaTurmaCursistasDTO>()
            };

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();
            _mediatorMock.Verify(m => m.Send(It.IsAny<PublicarNaFilaRabbitCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
