using System;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoSalvarInscricaoTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSalvarInscricao _sut;
        private readonly Faker _faker;

        public CasoDeUsoSalvarInscricaoTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _sut = mocker.CreateInstance<CasoDeUsoSalvarInscricao>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoInscricaoDtoValido_QuandoExecutar_EntaoDeveEnviarSalvarInscricaoCommandERetornarRetornoDto()
        {
            // Arrange
            var inscricaoDto = new InscricaoDto
            {
                PropostaTurmaId = _faker.Random.Long(1, 1000),
                VagaRemanescente = false
            };
            var retornoEsperado = RetornoDTO.RetornarSucesso("Inscrição salva com sucesso", _faker.Random.Long(1, 1000));

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarInscricaoCommand>(c => c.InscricaoDto == inscricaoDto), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            // Act
            var resultado = await _sut.Executar(inscricaoDto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeSameAs(retornoEsperado);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarInscricaoCommand>(c => c.InscricaoDto == inscricaoDto), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoExcecaoNoMediator_QuandoExecutar_EntaoDevePropagarExcecao()
        {
            // Arrange
            var inscricaoDto = new InscricaoDto
            {
                PropostaTurmaId = _faker.Random.Long(1, 1000),
                VagaRemanescente = true
            };
            var mensagemErro = _faker.Lorem.Sentence();

            _mediatorMock
                .Setup(m => m.Send(It.Is<SalvarInscricaoCommand>(c => c.InscricaoDto == inscricaoDto), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException(mensagemErro));

            // Act
            var act = () => _sut.Executar(inscricaoDto);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(mensagemErro);
            _mediatorMock.Verify(m => m.Send(It.Is<SalvarInscricaoCommand>(c => c.InscricaoDto == inscricaoDto), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
