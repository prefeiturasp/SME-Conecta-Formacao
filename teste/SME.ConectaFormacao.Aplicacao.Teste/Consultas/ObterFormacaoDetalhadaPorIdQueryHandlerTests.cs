using AutoMapper;
using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterFormacaoDetalhadaPorIdQueryHandlerTestes
    {
        private readonly Mock<ICacheDistribuido> _mockCacheDistribuido;
        private readonly Mock<IRepositorioProposta> _mockRepositorioProposta;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IRepositorioUsuarioAcessibilidade> _mockRepositorioUsuarioAcessibilidade;
        private readonly ObterFormacaoDetalhadaPorIdQueryHandler _handler;
        private readonly Faker _faker;

        public ObterFormacaoDetalhadaPorIdQueryHandlerTestes()
        {
            var mocker = new AutoMocker();
            _mockCacheDistribuido = mocker.GetMock<ICacheDistribuido>();
            _mockRepositorioProposta = mocker.GetMock<IRepositorioProposta>();
            _mockMapper = mocker.GetMock<IMapper>();
            _mockRepositorioUsuarioAcessibilidade = mocker.GetMock<IRepositorioUsuarioAcessibilidade>();
            _handler = mocker.CreateInstance<ObterFormacaoDetalhadaPorIdQueryHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoCacheVazioEFormacaoNaoEncontrada_QuandoObter_EntaoDeveLancarExcecao()
        {
            // Arrange
            var query = new ObterFormacaoDetalhadaPorIdQuery(_faker.Random.Long(1, 100));

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task DadoCacheVazioEFormacaoEncontrada_QuandoObter_EntaoDeveRetornarFormacaoDetalhada()
        {
            // Arrange
            var query = new ObterFormacaoDetalhadaPorIdQuery(_faker.Random.Long(1, 100));
            var formacaoDetalhada = new FormacaoDetalhada
            {
                NomeFormacao = _faker.Lorem.Sentence(),
                SobreEsteCurso = _faker.Lorem.Paragraph(),
                DataInscricaoInicio = DateTime.Now.AddDays(-10),
                DataInscricaoFim = DateTime.Now.AddDays(10),
                FormacaoHomologada = FormacaoHomologada.Sim,
                ArquivoImagemDivulgacao = null
            };

            var acessibilidade = new UsuarioAcessibilidade
            {
                Id = _faker.Random.Long(1),
                UsuarioId = _faker.Random.Long(1),
                PossuiDeficiencia = _faker.Random.Bool()
            };

            var acessibilidadeDto = new UsuarioAcessibilidadeDto
            {
                PossuiDeficiencia = acessibilidade.PossuiDeficiencia
            };

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = formacaoDetalhada.NomeFormacao,
                SobreEsteCurso = formacaoDetalhada.SobreEsteCurso,
                FormacaoHomologada = formacaoDetalhada.FormacaoHomologada,
                UsuarioAcessibilidade = acessibilidadeDto
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>()))
                .ReturnsAsync((RetornoFormacaoDetalhadaDTO)null);

            _mockRepositorioProposta
                .Setup(r => r.ObterFormacaoDetalhadaPorIdAsync(query.Id))
                .ReturnsAsync(formacaoDetalhada);

            _mockRepositorioUsuarioAcessibilidade
                .Setup(u => u.ObterAcessibilidadeAtualDoUsuarioAsync())
                .ReturnsAsync(acessibilidade);

            _mockMapper
                .Setup(m => m.Map<RetornoFormacaoDetalhadaDTO>(It.IsAny<FormacaoDetalhada>()))
                .Returns(formacaoDetalhadaDto);

            _mockMapper
                .Setup(m => m.Map<UsuarioAcessibilidadeDto>(It.IsAny<UsuarioAcessibilidade>()))
                .Returns(acessibilidadeDto);

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Should().BeEquivalentTo(formacaoDetalhadaDto);
            _mockCacheDistribuido
                .Verify(c => c.SalvarAsync(It.IsAny<string>(), It.IsAny<RetornoFormacaoDetalhadaDTO>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            _mockRepositorioProposta.Verify(p => p.ObterTurmasComVagaPorId(It.IsAny<long>(), null), Times.Never);
        }

        [Fact]
        public async Task DadoFormacaoNaoHomologadaNoCache_QuandoObter_EntaoDeveValidarVagasEAtualizarTurmas()
        {
            // Arrange
            var query = new ObterFormacaoDetalhadaPorIdQuery(_faker.Random.Long(1, 100));
            var formacaoDetalhada = new FormacaoDetalhada
            {
                NomeFormacao = _faker.Lorem.Sentence(),
                SobreEsteCurso = _faker.Lorem.Paragraph(),
                DataInscricaoInicio = DateTime.Now.AddDays(-10),
                DataInscricaoFim = DateTime.Now.AddDays(10),
                FormacaoHomologada = FormacaoHomologada.NaoCursosPorIN,
                ArquivoImagemDivulgacao = new() { Nome = _faker.System.FilePath() }
            };

            var formacaoDetalhadaDto = new RetornoFormacaoDetalhadaDTO
            {
                Titulo = formacaoDetalhada.NomeFormacao,
                SobreEsteCurso = formacaoDetalhada.SobreEsteCurso,
                FormacaoHomologada = formacaoDetalhada.FormacaoHomologada,
                Turmas =
                [
                    new()
                    {
                        Id = _faker.Random.Long(1),
                        Nome = _faker.Lorem.Word(),
                        InscricaoEncerrada = false
                    }
                ]
            };

            _mockCacheDistribuido
                .Setup(c => c.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(It.IsAny<string>()))
                .ReturnsAsync(formacaoDetalhadaDto);

            _mockRepositorioProposta
                .Setup(r => r.ObterTurmasComVagaPorId(query.Id, null))
                .ReturnsAsync([new() { Id = _faker.Random.Long(1) }]);

            // Act
            var resposta = await _handler.Handle(query, CancellationToken.None);

            // Assert
            resposta.Turmas.Should().NotBeEmpty();
            resposta.Turmas.Should().HaveCount(1);
            resposta.Turmas.First().Id.Should().Be(formacaoDetalhadaDto.Turmas.First().Id);
            resposta.Turmas.First().InscricaoEncerrada.Should().BeTrue();
            _mockCacheDistribuido
                .Verify(c => c.SalvarAsync(It.IsAny<string>(), It.IsAny<RetornoFormacaoDetalhadaDTO>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
            _mockRepositorioProposta.Verify(p => p.ObterTurmasComVagaPorId(It.IsAny<long>(), null), Times.Once);
        }
    }
}