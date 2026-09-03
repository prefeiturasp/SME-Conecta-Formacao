using AutoMapper;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterCodafCursoNaoHomologadoPorIdTestes
    {
        private readonly Mock<IRepositorioCodafCursoNaoHomologado> repositorio;
        private readonly Mock<IServicoArmazenamento> servicoArmazenamento;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<IContextoAplicacao> contextoAplicacao;

        private readonly CasoDeUsoObterCodafCursoNaoHomologadoPorId casoDeUso;

        public CasoDeUsoObterCodafCursoNaoHomologadoPorIdTestes()
        {
            repositorio = new Mock<IRepositorioCodafCursoNaoHomologado>();
            servicoArmazenamento = new Mock<IServicoArmazenamento>();
            mapper = new Mock<IMapper>();
            contextoAplicacao = new Mock<IContextoAplicacao>();

            casoDeUso = new CasoDeUsoObterCodafCursoNaoHomologadoPorId(
                repositorio.Object,
                servicoArmazenamento.Object,
                mapper.Object,
                contextoAplicacao.Object);
        }

        [Fact]
        public async Task Deve_Retornar_NaoEncontrado_Quando_Codaf_Nao_Existir()
        {
            // Arrange
            const long codafId = 1;

            contextoAplicacao
                .SetupGet(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            repositorio
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync((CodafCursoNaoHomologado?)null);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
            Assert.Equal(TipoFalha.NaoEncontrado, resultado.TipoFalha);
            Assert.Contains("Codaf não encontrado.", resultado.MensagensErro);

            repositorio.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafId),
                Times.Once);

            mapper.Verify(
                m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(
                    It.IsAny<object>()),
                Times.Never);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_Retornar_Erro_De_Negocio_Quando_Perfil_Restrito_Tentar_Visualizar_Codaf_De_Outro_Usuario()
        {
            // Arrange
            const long codafId = 2;
            const string loginUsuario = "usuario.logado";
            const string loginCriador = "outro.usuario";

            var perfilRestrito = Guid.NewGuid();

            var codaf = new CodafCursoNaoHomologado();

            var dto = new CodafCursoNaoHomologadoDetalhadoDto
            {
                Id = codafId,
                CriadoLogin = loginCriador
            };

            contextoAplicacao
                .SetupGet(c => c.IdPerfilUsuario)
                .Returns(perfilRestrito);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(loginUsuario);

            repositorio
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(codaf);

            mapper
                .Setup(m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf))
                .Returns(dto);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafId);

            // Assert
            Assert.False(resultado.Sucesso);
            Assert.Null(resultado.Dados);
            Assert.Equal(TipoFalha.RegraDeNegocio, resultado.TipoFalha);

            Assert.Contains(
                "Você não tem permissão para visualizar este codaf.",
                resultado.MensagensErro);

            repositorio.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafId),
                Times.Once);

            mapper.Verify(
                m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf),
                Times.Once);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_Retornar_Codaf_Quando_Perfil_Restrito_For_O_Criador()
        {
            // Arrange
            const long codafId = 3;
            const string loginUsuario = "usuario.criador";

            var perfilRestrito = Guid.NewGuid();

            var codaf = new CodafCursoNaoHomologado();

            var dto = new CodafCursoNaoHomologadoDetalhadoDto
            {
                Id = codafId,
                CriadoLogin = loginUsuario,
                Anexos = null
            };

            contextoAplicacao
                .SetupGet(c => c.IdPerfilUsuario)
                .Returns(perfilRestrito);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns(loginUsuario);

            repositorio
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(codaf);

            mapper
                .Setup(m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf))
                .Returns(dto);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Same(dto, resultado.Dados);
            Assert.Equal(TipoFalha.Nenhuma, resultado.TipoFalha);
            Assert.Empty(resultado.MensagensErro);

            repositorio.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafId),
                Times.Once);

            mapper.Verify(
                m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf),
                Times.Once);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_Permitir_Admin_Df_Visualizar_Codaf_De_Qualquer_Usuario_E_Preencher_Urls_Dos_Anexos()
        {
            // Arrange
            const long codafId = 4;

            var arquivoCodigo1 = Guid.NewGuid();
            var arquivoCodigo2 = Guid.NewGuid();

            const string url1 =
                "https://storage.teste/arquivo-1.pdf";

            const string url2 =
                "https://storage.teste/arquivo-2.pdf";

            var codaf = new CodafCursoNaoHomologado();

            var anexo1 = new CodafCursoNaoHomologadoAnexoDto
            {
                ArquivoCodigo = arquivoCodigo1,
                NomeArquivo = "arquivo-1.pdf",
                Extensao = ".pdf"
            };

            var anexo2 = new CodafCursoNaoHomologadoAnexoDto
            {
                ArquivoCodigo = arquivoCodigo2,
                NomeArquivo = "arquivo-2.pdf",
                Extensao = ".pdf"
            };

            var dto = new CodafCursoNaoHomologadoDetalhadoDto
            {
                Id = codafId,

                // Login propositalmente diferente.
                // ADMIN_DF deve ignorar essa restrição.
                CriadoLogin = "outro.usuario",

                Anexos =
                [
                    anexo1,
                    anexo2
                ]
            };

            contextoAplicacao
                .SetupGet(c => c.IdPerfilUsuario)
                .Returns(Perfis.ADMIN_DF);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns("admin.df");

            repositorio
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(codaf);

            mapper
                .Setup(m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf))
                .Returns(dto);

            servicoArmazenamento
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(
                    arquivoCodigo1.ToString()))
                .ReturnsAsync(url1);

            servicoArmazenamento
                .Setup(s => s.ObterUrlPorChaveObjetoAsync(
                    arquivoCodigo2.ToString()))
                .ReturnsAsync(url2);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Same(dto, resultado.Dados);

            Assert.NotNull(resultado.Dados.Anexos);
            Assert.Equal(2, resultado.Dados.Anexos.Count);

            Assert.Equal(url1, resultado.Dados.Anexos[0].UrlDownload);
            Assert.Equal(url2, resultado.Dados.Anexos[1].UrlDownload);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(
                    arquivoCodigo1.ToString()),
                Times.Once);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(
                    arquivoCodigo2.ToString()),
                Times.Once);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task Deve_Permitir_Emforpef_Visualizar_Codaf_De_Qualquer_Usuario()
        {
            // Arrange
            const long codafId = 5;

            var codaf = new CodafCursoNaoHomologado();

            var dto = new CodafCursoNaoHomologadoDetalhadoDto
            {
                Id = codafId,
                CriadoLogin = "outro.usuario",
                Anexos = []
            };

            contextoAplicacao
                .SetupGet(c => c.IdPerfilUsuario)
                .Returns(Perfis.EMFORPEF);

            contextoAplicacao
                .SetupGet(c => c.LoginUsuario)
                .Returns("usuario.emforpef");

            repositorio
                .Setup(r => r.ObterPorIdDetalhadoAsync(codafId))
                .ReturnsAsync(codaf);

            mapper
                .Setup(m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf))
                .Returns(dto);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(codafId);

            // Assert
            Assert.True(resultado.Sucesso);
            Assert.NotNull(resultado.Dados);
            Assert.Same(dto, resultado.Dados);

            repositorio.Verify(
                r => r.ObterPorIdDetalhadoAsync(codafId),
                Times.Once);

            mapper.Verify(
                m => m.Map<CodafCursoNaoHomologadoDetalhadoDto>(codaf),
                Times.Once);

            servicoArmazenamento.Verify(
                s => s.ObterUrlPorChaveObjetoAsync(It.IsAny<string>()),
                Times.Never);
        }
    }
}
