using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Collections.Generic;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCodafListaPresencaPorIdTests
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioCodafComentarioListaPresenca> _repositorioCodafComentarioListaPresencaMock;
        private readonly Mock<IRepositorioCodafInscritosListaPresenca> _repositorioCodafInscritosMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CasoDeUsoObterCodafListaPresencaPorId _casoDeUsoObterCodafListaPresencaPorId;
        private readonly Faker _faker;

        public CasoDeUsoObterCodafListaPresencaPorIdTests()
        {
            var mocker = new Moq.AutoMock.AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafComentarioListaPresencaMock = mocker.GetMock<IRepositorioCodafComentarioListaPresenca>();
            _repositorioCodafInscritosMock = mocker.GetMock<IRepositorioCodafInscritosListaPresenca>();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _mapperMock = mocker.GetMock<IMapper>();
            _casoDeUsoObterCodafListaPresencaPorId = mocker.CreateInstance<CasoDeUsoObterCodafListaPresencaPorId>();
            _faker = new();
        }

        [Fact]
        public async Task DadoIdValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                new(DataPublicacao: DateTime.Now,
                DataPublicacaoDom: DateTime.Now,
                NumeroComunicado: 123,
                PaginaComunicadoDom: 12,
                CodigoCursoEol: 1,
                CodigoNivel: 2,
                Observacao: "Observação teste"),
                Perfis.ADMIN_DF);
            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = 1
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);

            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Id.Should().Be(listaPresencaId);
        }

        [Fact]
        public async Task DadoIdInvalido_QuandoChamarExecutar_DeveRetornarErroNaoEncontrado()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync((CodafListaPresenca?)null);
            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);
            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.MensagensErro.Should().NotBeNull();
            resultado.MensagensErro.Should().Contain("Lista de presença não encontrada.");
        }

        [Fact]
        public async Task DadoListaPresencaComAnexos_QuandoChamarExecutar_DeveObterUrlDownloadDosAnexos()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var anexoCodigo = _faker.Random.Guid();
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                new(DataPublicacao: DateTime.Now,
                DataPublicacaoDom: DateTime.Now,
                NumeroComunicado: 123,
                PaginaComunicadoDom: 12,
                CodigoCursoEol: 1,
                CodigoNivel: 2,
                Observacao: "Observação teste"),
                Perfis.ADMIN_DF);

            var anexoDto = new CodafAnexoDto
            {
                ArquivoCodigo = anexoCodigo,
                NomeArquivo = _faker.System.FileName(),
                Extensao = _faker.System.FileExt()
            };
            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = 1,
                Anexos = [anexoDto]
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);
            var urlDownloadEsperada = "http://url-de-download.com/arquivo";
            _servicoArmazenamentoMock
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(anexoCodigo.ToString()))
                .ReturnsAsync(urlDownloadEsperada);

            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Anexos.Should().NotBeNull();
            resultado.Dados.Anexos.Should().ContainSingle();
            resultado.Dados.Anexos[0].UrlDownload.Should().Be(urlDownloadEsperada);
            _servicoArmazenamentoMock.Verify(s => s.ObterUrlPorChaveObjetoAsync(anexoCodigo.ToString()), Times.Once);
        }

        [Fact]
        public async Task DadoListaPresencaComStatusDevolvido_QuandoChamarExecutar_DeveObterComentarioDevolucao()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: 1,
                new(DataPublicacao: DateTime.Now,
                DataPublicacaoDom: DateTime.Now,
                NumeroComunicado: 123,
                PaginaComunicadoDom: 12,
                CodigoCursoEol: 1,
                CodigoNivel: 2,
                Observacao: "Observação teste"),
                Perfis.ADMIN_DF);
            var comentarioDevolucaoDto = new CodafComentarioDevolucaoDto
            {
                Id = _faker.Random.Long(1),
                CodafListaPresencaId = listaPresencaId,
                Comentario = _faker.Lorem.Sentence(),
                CriadoEm = DateTime.Now
            };
            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = 1,
                Status = StatusCodafListaPresenca.DevolvidoParaCorrecao
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);
            _repositorioCodafComentarioListaPresencaMock
                .Setup(r => r.ObterUltimoComentarioDevolucaoPorUsuarioAsync(
                    listaPresencaId,
                    StatusCodafListaPresenca.DevolvidoParaCorrecao,
                    StatusCodafListaPresenca.AguardandoDf))
                .ReturnsAsync(comentarioDevolucaoDto);

            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.Status.Should().Be(StatusCodafListaPresenca.DevolvidoParaCorrecao);
            resultado.Dados.Comentario.Should().NotBeNull();
            resultado.Dados.Comentario.Should().BeEquivalentTo(comentarioDevolucaoDto);
            _repositorioCodafComentarioListaPresencaMock.Verify(r => r.ObterUltimoComentarioDevolucaoPorUsuarioAsync(
                listaPresencaId,
                StatusCodafListaPresenca.DevolvidoParaCorrecao,
                StatusCodafListaPresenca.AguardandoDf), Times.Once);
        }

        [Fact]
        public async Task DadoDeltaInscritosVazio_QuandoChamarExecutar_DeveRetornarListaPresencaDtoComDeltaInscritosVazio()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: propostaTurmaId,
                new(DataPublicacao: DateTime.Now,
                DataPublicacaoDom: DateTime.Now,
                NumeroComunicado: 123,
                PaginaComunicadoDom: 12,
                CodigoCursoEol: 1,
                CodigoNivel: 2,
                Observacao: "Observação teste"),
                Perfis.ADMIN_DF);
            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = propostaTurmaId
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);

            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.DeltaInscritos.Should().NotBeNull();
            resultado.Dados.DeltaInscritos.InscritosRemovidos.Should().BeEmpty();
            resultado.Dados.DeltaInscritos.InscritosNovos.Should().BeEmpty();
        }

        [Fact]
        public async Task DadoDeltaInscritosComRemovidosEAdicionados_QuandoChamarExecutar_DeveRetornarListaPresencaDtoComDeltaInscritosPreenchido()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var propostaTurmaId = _faker.Random.Long(1);
            var listaPresencaEntidade = new CodafListaPresenca(
                propostaId: 1,
                propostaTurmaId: propostaTurmaId,
                new(DataPublicacao: DateTime.Now,
                DataPublicacaoDom: DateTime.Now,
                NumeroComunicado: 123,
                PaginaComunicadoDom: 12,
                CodigoCursoEol: 1,
                CodigoNivel: 2,
                Observacao: "Observação"),
                Perfis.ADMIN_DF);

            var deltaInscritos = new List<ResultadoDeltaInscritoCodafDto>
            {
                new()
                {
                    TipoDelta = TipoDeltaInscritoCodaf.Removido,
                    DadosInscrito = new()
                    {
                        Id = _faker.Random.Long(1),
                        Nome = _faker.Name.FullName(),
                        Login = _faker.Internet.UserName(),
                        Cpf = _faker.Person.Cpf()
                    }
                },
                new()
                {
                    TipoDelta = TipoDeltaInscritoCodaf.Novo,
                    DadosInscrito = new()
                    {
                        Id = _faker.Random.Long(1),
                        Nome = _faker.Name.FullName(),
                        Login = _faker.Internet.UserName(),
                        Cpf = _faker.Person.Cpf()
                    }
                }
            };

            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = propostaTurmaId
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);
            _repositorioCodafInscritosMock
                .Setup(r => r.ObterDeltaInscritosCodafAsync(It.IsAny<long>()))
                .ReturnsAsync(deltaInscritos);
            _mapperMock
                .Setup(m => m.Map<IList<CodafInscritoTurmaListaPresencaRetornoDto>>(It.IsAny<List<ResultadoInscritoTurmaCodafListaPresencaDto>>()))
                .Returns([new()]);

            // Act
            var resultado = await _casoDeUsoObterCodafListaPresencaPorId.ExecutarAsync(listaPresencaId);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.DeltaInscritos.Should().NotBeNull();
            resultado.Dados.DeltaInscritos.InscritosRemovidos.Should().HaveCount(1);
            resultado.Dados.DeltaInscritos.InscritosNovos.Should().HaveCount(1);
        }
    }
}