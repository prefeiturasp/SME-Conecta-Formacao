using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterArquivosInscricaoImportadosTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterArquivosInscricaoImportados _sut;
        private readonly Faker _faker;

        public CasoDeUsoObterArquivosInscricaoImportadosTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroPagina")).Returns("1");
            _contextoAplicacaoMock.Setup(c => c.ObterVariavel<string>("NumeroRegistros")).Returns("10");

            _sut = mocker.CreateInstance<CasoDeUsoObterArquivosInscricaoImportados>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoPropostaIdValido_QuandoExecutar_EntaoDeveRetornarArquivosImportados()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 100);
            var resultadoPaginado = new PaginacaoResultadoDto<ArquivoInscricaoImportadoDTO>(new List<ArquivoInscricaoImportadoDTO>(), 0, 10);

            _mediatorMock.Setup(m => m.Send(It.Is<ObterArquivosInscricaoImportadosQuery>(q => 
                q.QuantidadeRegistrosIgnorados == _sut.QuantidadeRegistrosIgnorados &&
                q.NumeroRegistros == _sut.NumeroRegistros &&
                q.PropostaId == propostaId), default))
                .ReturnsAsync(resultadoPaginado);

            // Act
            var resultado = await _sut.Executar(propostaId);

            // Assert
            resultado.Should().BeEquivalentTo(resultadoPaginado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterArquivosInscricaoImportadosQuery>(), default), Times.Once);
        }
    }
}
