using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Proposta.ObterPropostaPaginada;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterPropostaPaginadaQueryHandlerTestes
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositorioProposta> _repositorioPropostaMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly ObterPropostaPaginadaQueryHandler _handler;
        private readonly Faker _faker;

        public ObterPropostaPaginadaQueryHandlerTestes()
        {
            var mocker = new AutoMocker();

            _mapperMock = mocker.GetMock<IMapper>();
            _repositorioPropostaMock = mocker.GetMock<IRepositorioProposta>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _handler = mocker.CreateInstance<ObterPropostaPaginadaQueryHandler>();

            _faker = new();
        }

        [Fact]
        public async Task DadoFiltroValido_QuandoExecutarHandle_EntaoDeveRetornarPaginacaoResultadoDto()
        {
            // Arrange
            var query = new ObterPropostaPaginadaQuery(
                numeroPagina: 1,
                numeroRegistros: 10,
                areaPromotoraIdUsuarioLogado: null,
                propostaFiltrosDTO: new()
                {
                    Id = null,
                    AreaPromotoraId = null,
                    Formato = null,
                    PublicoAlvoIds = null,
                    NomeFormacao = null,
                    NumeroHomologacao = null,
                    PeriodoRealizacaoInicio = null,
                    PeriodoRealizacaoFim = null,
                    Situacao = null,
                    FormacaoHomologada = null,
                    Revalidacao = null
                });
            _contextoAplicacaoMock.Setup(c => c.UsuarioLogado).Returns(_faker.Person.Cpf());
            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Guid.NewGuid());
            _repositorioPropostaMock.Setup(r => r.ObterPropostaPorFiltroAsync(It.IsAny<FiltroListagemPropostaDto>()))
                .ReturnsAsync(new ResultadoPaginado<Proposta>
                {
                    Itens = [new(), new(), new(), new(), new()],
                    TotalRegistros = 5,
                    TamanhoPagina = 10
                });
            _mapperMock.Setup(m => m.Map<List<PropostaPaginadaDTO>>(It.IsAny<IEnumerable<Proposta>>()))
                .Returns([new (), new(), new(), new(), new()]);

            // Act
            var resultado = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeOfType<PaginacaoResultadoDto<PropostaPaginadaDTO>>();
            resultado.Items.Should().HaveCount(5);
            resultado.TotalRegistros.Should().Be(5);
            resultado.TotalPaginas.Should().Be(1);
        }
    }
}
