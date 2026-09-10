using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricoes
{
    public class CasoDeUsoObterDadosPaginadosComFiltrosTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterDadosPaginadosComFiltros _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterDadosPaginadosComFiltrosTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterDadosPaginadosComFiltros>();
            _faker = new Faker();
        }

        [Fact]
        public async Task DadoFiltroEAreaPromotoraLogada_QuandoExecutar_EntaoDeveEnviarQueryComAreaPromotoraERetornarPaginacao()
        {
            // Arrange
            var filtro = new FiltroListagemInscricaoComTurmaDTO
            {
                CodigoFormacao = _faker.Random.Long(1, 1000),
                NomeFormacao = _faker.Random.Word(),
                NumeroHomologacao = _faker.Random.Long(1, 1000),
                ApenasSemCodaf = _faker.Random.Bool()
            };
            _casoDeUso.NumeroPagina = _faker.Random.Int(1, 10);
            _casoDeUso.NumeroRegistros = _faker.Random.Int(10, 50);

            var areaPromotora = new AreaPromotora { Id = _faker.Random.Int(1, 100) };
            
            var resultadoEsperado = new PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>(new List<DadosListagemFormacaoComTurmaDTO>(), 0, 10);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterDadosPaginadosComFiltrosQuery>(q => 
                    q.NumeroPagina == _casoDeUso.NumeroPagina &&
                    q.NumeroRegistros == _casoDeUso.NumeroRegistros &&
                    q.CodigoFormacao == filtro.CodigoFormacao &&
                    q.NomeFormacao == filtro.NomeFormacao &&
                    q.AreaPromotoraIdUsuarioLogado == areaPromotora.Id &&
                    q.NumeroHomologacao == filtro.NumeroHomologacao &&
                    q.ApenasSemCodaf == filtro.ApenasSemCodaf
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(filtro);

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterDadosPaginadosComFiltrosQuery>(q => 
                    q.NumeroPagina == _casoDeUso.NumeroPagina &&
                    q.NumeroRegistros == _casoDeUso.NumeroRegistros &&
                    q.CodigoFormacao == filtro.CodigoFormacao &&
                    q.NomeFormacao == filtro.NomeFormacao &&
                    q.AreaPromotoraIdUsuarioLogado == areaPromotora.Id &&
                    q.NumeroHomologacao == filtro.NumeroHomologacao &&
                    q.ApenasSemCodaf == filtro.ApenasSemCodaf
                ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoFiltroESemAreaPromotoraLogada_QuandoExecutar_EntaoDeveEnviarQueryComAreaPromotoraNulaERetornarPaginacao()
        {
            // Arrange
            var filtro = new FiltroListagemInscricaoComTurmaDTO();
            _casoDeUso.NumeroPagina = _faker.Random.Int(1, 10);
            _casoDeUso.NumeroRegistros = _faker.Random.Int(10, 50);

            var resultadoEsperado = new PaginacaoResultadoDto<DadosListagemFormacaoComTurmaDTO>(new List<DadosListagemFormacaoComTurmaDTO>(), 0, 10);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaPromotora?)null);

            _mediatorMock
                .Setup(m => m.Send(It.Is<ObterDadosPaginadosComFiltrosQuery>(q => 
                    q.NumeroPagina == _casoDeUso.NumeroPagina &&
                    q.NumeroRegistros == _casoDeUso.NumeroRegistros &&
                    q.AreaPromotoraIdUsuarioLogado == null
                ), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultadoEsperado);

            // Act
            var resultado = await _casoDeUso.Executar(filtro);

            // Assert
            resultado.Should().BeSameAs(resultadoEsperado);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterAreaPromotoraUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.Is<ObterDadosPaginadosComFiltrosQuery>(q => q.AreaPromotoraIdUsuarioLogado == null), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
