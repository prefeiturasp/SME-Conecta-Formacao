using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafListaPresencas;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using System.Collections.Generic;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCodafListaPresencaPorIdTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> _repositorioCodafListaPresencaMock;
        private readonly Mock<IRepositorioCodafComentarioListaPresenca> _repositorioCodafComentarioListaPresencaMock;
        private readonly Mock<IRepositorioCodafInscritosListaPresenca> _repositorioCodafInscritosMock;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamentoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IContextoAplicacao> _contextoAplicacaoMock;
        private readonly CasoDeUsoObterCodafListaPresencaPorId _casoDeUsoObterCodafListaPresencaPorId;
        private readonly Faker _faker;

        public CasoDeUsoObterCodafListaPresencaPorIdTestes()
        {
            var mocker = new Moq.AutoMock.AutoMocker();
            _repositorioCodafListaPresencaMock = mocker.GetMock<IRepositorioCodafListaPresenca>();
            _repositorioCodafComentarioListaPresencaMock = mocker.GetMock<IRepositorioCodafComentarioListaPresenca>();
            _repositorioCodafInscritosMock = mocker.GetMock<IRepositorioCodafInscritosListaPresenca>();
            _servicoArmazenamentoMock = mocker.GetMock<IServicoArmazenamento>();
            _mapperMock = mocker.GetMock<IMapper>();
            _contextoAplicacaoMock = mocker.GetMock<IContextoAplicacao>();
            _casoDeUsoObterCodafListaPresencaPorId = mocker.CreateInstance<CasoDeUsoObterCodafListaPresencaPorId>();
            _faker = new();

            _contextoAplicacaoMock.Setup(c => c.IdPerfilUsuario).Returns(Perfis.ADMIN_DF);
        }

        [Fact]
        public async Task DadoIdValido_QuandoChamarExecutar_DeveRetornarResultadoEsperado()
        {
            // Arrange
            var listaPresencaId = _faker.Random.Long(1);
            var criadoLogin = _faker.Internet.UserName();
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
                PropostaTurmaId = 1,
                CriadoLogin = criadoLogin,
                DeltaInscritos = new()
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);
            _repositorioCodafInscritosMock
                .Setup(r => r.ObterDeltaInscritosCodafAsync(It.IsAny<long>()))
                .ReturnsAsync((IList<ResultadoDeltaInscritoCodafDto>?)null);

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
            var criadoLogin = _faker.Internet.UserName();
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
                CriadoLogin = criadoLogin,
                Anexos = [anexoDto],
                DeltaInscritos = new()
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
            _repositorioCodafInscritosMock
                .Setup(r => r.ObterDeltaInscritosCodafAsync(It.IsAny<long>()))
                .ReturnsAsync((IList<ResultadoDeltaInscritoCodafDto>?)null);

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
            var criadoLogin = _faker.Internet.UserName();
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
                CriadoLogin = criadoLogin,
                Status = StatusCodafListaPresenca.DevolvidoParaCorrecao,
                DeltaInscritos = new()
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
            _repositorioCodafInscritosMock
                .Setup(r => r.ObterDeltaInscritosCodafAsync(It.IsAny<long>()))
                .ReturnsAsync((IList<ResultadoDeltaInscritoCodafDto>?)null);

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
            var criadoLogin = _faker.Internet.UserName();
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
                PropostaTurmaId = propostaTurmaId,
                CriadoLogin = criadoLogin
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);
            _repositorioCodafInscritosMock
                .Setup(r => r.ObterDeltaInscritosCodafAsync(It.IsAny<long>()))
                .ReturnsAsync((IList<ResultadoDeltaInscritoCodafDto>?)null);

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
            var criadoLogin = _faker.Internet.UserName();
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

            var cpfRemovido = _faker.Person.Cpf();
            var cpfNovo = _faker.Person.Cpf();
            var loginRemovido = _faker.Internet.UserName();
            var loginNovo = _faker.Internet.UserName();

            var deltaInscritos = new List<ResultadoDeltaInscritoCodafDto>
            {
                new()
                {
                    TipoDelta = TipoDeltaInscritoCodaf.Removido,
                    DadosInscrito = new()
                    {
                        Id = _faker.Random.Long(1),
                        Nome = _faker.Name.FullName(),
                        Login = loginRemovido,
                        Cpf = cpfRemovido
                    }
                },
                new()
                {
                    TipoDelta = TipoDeltaInscritoCodaf.Novo,
                    DadosInscrito = new()
                    {
                        Id = _faker.Random.Long(1),
                        Nome = _faker.Name.FullName(),
                        Login = loginNovo,
                        Cpf = cpfNovo
                    }
                }
            };

            var listaPresencaDto = new CodafListaPresencaDto
            {
                Id = listaPresencaId,
                PropostaId = 1,
                PropostaTurmaId = propostaTurmaId,
                CriadoLogin = criadoLogin
            };
            _repositorioCodafListaPresencaMock
                .Setup(r => r.ObterPorIdDetalhadoAsync(listaPresencaId))
                .ReturnsAsync(listaPresencaEntidade);
            _mapperMock
                .Setup(m => m.Map<CodafListaPresencaDto>(listaPresencaEntidade))
                .Returns(listaPresencaDto);
            _repositorioCodafInscritosMock
                .Setup(r => r.ObterDeltaInscritosCodafAsync(propostaTurmaId))
                .ReturnsAsync(deltaInscritos);

            var inscritoNovoMapeado = new CodafInscritoTurmaListaPresencaRetornoDto
            {
                Id = deltaInscritos[1].DadosInscrito.Id,
                Nome = deltaInscritos[1].DadosInscrito.Nome,
                Documento = cpfNovo
            };

            _mapperMock
                .Setup(m => m.Map<IList<CodafInscritoTurmaListaPresencaRetornoDto>>(It.IsAny<List<ResultadoInscritoTurmaCodafListaPresencaDto>>()))
                .Returns([inscritoNovoMapeado]);

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