using AutoMapper;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using DominioUsuario = SME.ConectaFormacao.Dominio.Entidades.Usuario;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoRealizarInscricaoAutomaticaInscreverCursistaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista _sut;
        private readonly Faker _faker;

        public CasoDeUsoRealizarInscricaoAutomaticaInscreverCursistaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _mapperMock = mocker.GetMock<IMapper>();
            _sut = mocker.CreateInstance<CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista>();
            _faker = new Faker();
        }

        [Fact]
        public void DadoMapperNulo_QuandoInstanciar_EntaoDeveLancarArgumentNullException()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();

            // Act
            Action act = () => new CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista(mediatorMock.Object, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("mapper");
        }

        [Fact]
        public async Task DadoCursistaComUsuarioExistente_QuandoExecutar_EntaoNaoChamaMapperNemSalvarUsuarioCommandEEnviaSalvarInscricaoAutomaticaCommandERetornaTrue()
        {
            // Arrange
            var rf = _faker.Random.Number(1000000, 9999999).ToString();
            var usuarioIdExistente = _faker.Random.Long(1, 1000);
            var dto = new InscricaoAutomaticaDTO
            {
                UsuarioRf = rf,
                UsuarioNome = _faker.Person.FullName,
                PropostaId = _faker.Random.Long(1, 1000),
                PropostaTurmaId = _faker.Random.Long(1, 1000)
            };

            var usuarioExistente = new DominioUsuario
            {
                Id = usuarioIdExistente,
                Login = rf,
                Nome = dto.UsuarioNome
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == rf), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarInscricaoAutomaticaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1L);

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();
            _mapperMock.Verify(m => m.Map<DominioUsuario>(It.IsAny<object>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarInscricaoAutomaticaCommand>(c =>
                c.InscricaoAutomaticaDTO.UsuarioId == usuarioIdExistente &&
                c.InscricaoAutomaticaDTO.UsuarioRf == rf
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoCursistaSemUsuarioExistente_QuandoExecutar_EntaoMapeiaESalvaUsuarioEEnviaSalvarInscricaoAutomaticaCommandERetornaTrue()
        {
            // Arrange
            var rf = _faker.Random.Number(1000000, 9999999).ToString();
            var novoUsuarioId = _faker.Random.Long(1, 1000);
            var dto = new InscricaoAutomaticaDTO
            {
                UsuarioRf = rf,
                UsuarioNome = _faker.Person.FullName,
                PropostaId = _faker.Random.Long(1, 1000),
                PropostaTurmaId = _faker.Random.Long(1, 1000)
            };

            var usuarioMapeado = new DominioUsuario
            {
                Id = novoUsuarioId,
                Login = rf,
                Nome = dto.UsuarioNome
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == rf), It.IsAny<CancellationToken>()))
                .ReturnsAsync((DominioUsuario)null);

            _mapperMock
                .Setup(m => m.Map<DominioUsuario>(It.Is<InscricaoAutomaticaDTO>(d => d.UsuarioRf == rf)))
                .Returns(usuarioMapeado);

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarUsuarioCommand>(c => c.Usuario == usuarioMapeado), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarInscricaoAutomaticaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1L);

            var mensagem = new MensagemRabbit(JsonSerializer.Serialize(dto));

            // Act
            var resultado = await _sut.Executar(mensagem);

            // Assert
            resultado.Should().BeTrue();
            _mapperMock.Verify(m => m.Map<DominioUsuario>(It.Is<InscricaoAutomaticaDTO>(d => d.UsuarioRf == rf)), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarUsuarioCommand>(c => c.Usuario == usuarioMapeado), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarInscricaoAutomaticaCommand>(c =>
                c.InscricaoAutomaticaDTO.UsuarioId == novoUsuarioId &&
                c.InscricaoAutomaticaDTO.UsuarioRf == rf
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
