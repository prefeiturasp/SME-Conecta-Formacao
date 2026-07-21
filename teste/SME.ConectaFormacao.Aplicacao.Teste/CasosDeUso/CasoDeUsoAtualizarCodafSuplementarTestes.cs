using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf.Dependencias;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafSuplementares;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafSuplementares;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Servicos.Interfaces;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAtualizarCodafSuplementarTestes
    {
        private const long CodafSuplementarId = 10;

        private readonly Mock<IRepositorioCodafSuplementar> repositorioCodaf;
        private readonly Mock<IRepositorioCodafListaPresenca> repositorioLista;
        private readonly Mock<IRepositorioCodafSuplementarRetificacao> repositorioRetificacao;
        private readonly Mock<ICodafSuplementarInscritosService> inscritosService;
        private readonly Mock<IGerenciadorAnexosCodafSuplementarService> anexoService;
        private readonly Mock<IMapper> mapper;
        private readonly Mock<ITransacao> transacao;
        private readonly Mock<IDbTransaction> transacaoDb;
        private readonly Mock<IValidator<CodafSuplementarCadastroDto>> validator;

        private readonly CasoDeUsoAtualizarCodafSuplementar casoDeUso;

        public CasoDeUsoAtualizarCodafSuplementarTestes()
        {
            repositorioCodaf = new Mock<IRepositorioCodafSuplementar>();
            repositorioLista = new Mock<IRepositorioCodafListaPresenca>();
            repositorioRetificacao = new Mock<IRepositorioCodafSuplementarRetificacao>();
            inscritosService = new Mock<ICodafSuplementarInscritosService>();
            anexoService = new Mock<IGerenciadorAnexosCodafSuplementarService>();
            mapper = new Mock<IMapper>();
            transacao = new Mock<ITransacao>();
            transacaoDb = new Mock<IDbTransaction>();
            validator = new Mock<IValidator<CodafSuplementarCadastroDto>>();

            transacao
                .Setup(t => t.Iniciar())
                .Returns(transacaoDb.Object);

            var dependencias = new CodafSuplementarDependencias(
                repositorioCodaf.Object,
                repositorioLista.Object,
                repositorioRetificacao.Object,
                inscritosService.Object,
                anexoService.Object,
                mapper.Object,
                transacao.Object);

            casoDeUso = new CasoDeUsoAtualizarCodafSuplementar(
                dependencias,
                validator.Object);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarErro_QuandoCodafSuplementarNaoExistir()
        {
            // Arrange
            var dto = CriarDto();

            repositorioCodaf
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(CodafSuplementarId))
                .ReturnsAsync((CodafSuplementar?)null);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(dto, CodafSuplementarId);

            // Assert
            Assert.Contains(
                "Codaf Suplementar não encontrado",
                ObterConteudoErro(resultado));

            repositorioCodaf.Verify(
                r => r.ObterPorIdDetalhadoAsync(It.IsAny<long>()),
                Times.Never);

            validator.Verify(
                v => v.ValidateAsync(
                    It.IsAny<CodafSuplementarCadastroDto>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            transacao.Verify(
                t => t.Iniciar(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Commit(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Rollback(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Dispose(),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveAtualizarCodafSemSalvarInscritos_QuandoPossuirCertificadoEmitido()
        {
            // Arrange
            var dto = CriarDto();
            var existente = CriarCodafSuplementar(CodafSuplementarId);

            AdicionarItemNaColecao(
                existente,
                "CodafCertificados");

            var anexosMapeados = new List<CodafSuplementarAnexo>();

            repositorioCodaf
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(CodafSuplementarId))
                .ReturnsAsync(existente);

            validator
                .Setup(v => v.ValidateAsync(
                    dto,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            repositorioRetificacao
                .Setup(r => r.ObterPorCodafSuplementarIdAsync(
                    CodafSuplementarId))
                .ReturnsAsync(new List<CodafSuplementarRetificacao>());

            mapper
                .Setup(m => m.Map<List<CodafSuplementarAnexo>>(dto.Anexos))
                .Returns(anexosMapeados);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(
                dto,
                CodafSuplementarId);

            // Assert
            Assert.False(resultado is Erro);

            validator.Verify(
                v => v.ValidateAsync(
                    dto,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mapper.Verify(
                m => m.Map<List<CodafSuplementarInscricao>>(dto.Inscritos),
                Times.Never);

            inscritosService.Verify(
                s => s.SalvarInscritosAsync(
                    It.IsAny<List<CodafSuplementarInscricao>>(),
                    It.IsAny<long>()),
                Times.Never);

            repositorioRetificacao.Verify(
                r => r.ObterPorCodafSuplementarIdAsync(
                    CodafSuplementarId),
                Times.Once);

            anexoService.Verify(
                s => s.ProcessarAnexosAsync(
                    CodafSuplementarId,
                    anexosMapeados),
                Times.Once);

            repositorioCodaf.Verify(
                r => r.Atualizar(existente),
                Times.Once);

            Assert.Same(
                anexosMapeados,
                existente.CodafAnexos);

            transacao.Verify(
                t => t.Iniciar(),
                Times.Once);

            transacaoDb.Verify(
                t => t.Commit(),
                Times.Once);

            transacaoDb.Verify(
                t => t.Rollback(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Dispose(),
                Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRetornarErro_QuandoDtoForInvalido()
        {
            // Arrange
            var dto = CriarDto();
            var existente = CriarCodafSuplementar(CodafSuplementarId);
            var detalhado = CriarCodafSuplementar(CodafSuplementarId);

            var falhas = new[]
            {
        new ValidationFailure(
            nameof(CodafSuplementarCadastroDto.CodafId),
            "O Id do Codaf é obrigatório.")
    };

            repositorioCodaf
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(CodafSuplementarId))
                .ReturnsAsync(existente);

            repositorioCodaf
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(detalhado);

            validator
                .Setup(v => v.ValidateAsync(
                    dto,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(falhas));

            // Act
            var resultado = await casoDeUso.ExecutarAsync(
                dto,
                CodafSuplementarId);

            // Assert
            Assert.Contains(
                "O Id do Codaf é obrigatório.",
                ObterConteudoErro(resultado));

            validator.Verify(
                v => v.ValidateAsync(
                    dto,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            transacao.Verify(
                t => t.Iniciar(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Commit(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Rollback(),
                Times.Never);

            transacaoDb.Verify(
                t => t.Dispose(),
                Times.Never);

            inscritosService.Verify(
                s => s.SalvarInscritosAsync(
                    It.IsAny<List<CodafSuplementarInscricao>>(),
                    It.IsAny<long>()),
                Times.Never);

            repositorioCodaf.Verify(
                r => r.Atualizar(
                    It.IsAny<CodafSuplementar>()),
                Times.Never);
        }

        [Fact]
        public async Task ExecutarAsync_DeveAtualizarCodafEProcessarTodasAsRetificacoes()
        {
            // Arrange
            var dto = CriarDto();

            var existente = CriarCodafSuplementar(CodafSuplementarId);
            var detalhado = CriarCodafSuplementar(CodafSuplementarId);

            var retificacaoParaRemover =
                CriarRetificacao(CodafSuplementarId, 100);

            var retificacaoParaAtualizar =
                CriarRetificacao(CodafSuplementarId, 200);

            var retificacoesExistentes = new List<CodafSuplementarRetificacao>
            {
                retificacaoParaRemover,
                retificacaoParaAtualizar
            };

            var retificacaoAtualizadaDto =
                new CodafSuplementarRetificacaoSalvarDto
                {
                    Id = 200,
                    DataRetificacao = new DateTime(2026, 2, 10),
                    PaginaRetificacaoDom = 15
                };

            var retificacaoInexistenteDto =
                new CodafSuplementarRetificacaoSalvarDto
                {
                    Id = 999,
                    DataRetificacao = new DateTime(2026, 2, 11),
                    PaginaRetificacaoDom = 16
                };

            var novaRetificacaoDto =
                new CodafSuplementarRetificacaoSalvarDto
                {
                    Id = 0,
                    DataRetificacao = new DateTime(2026, 2, 12),
                    PaginaRetificacaoDom = 17
                };

            dto.Retificacoes = new List<CodafSuplementarRetificacaoSalvarDto>
            {
                retificacaoAtualizadaDto,
                retificacaoInexistenteDto,
                novaRetificacaoDto
            };

            var inscritosMapeados = new List<CodafSuplementarInscricao>
            {
                CriarEntidade<CodafSuplementarInscricao>()
            };

            var anexosMapeados = new List<CodafSuplementarAnexo>
            {
                new()
                {
                    CodafSuplementarId = CodafSuplementarId,
                    ArquivoCodigo = Guid.NewGuid(),
                    NomeArquivo = "comunicado.pdf",
                    Extensao = ".pdf",
                    TipoAnexoId = default
                }
            };

            var novaRetificacaoMapeada =
                CriarRetificacao(codafSuplementarId: 0, id: 0);

            repositorioCodaf
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(CodafSuplementarId))
                .ReturnsAsync(existente);

            repositorioCodaf
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(detalhado);

            validator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            repositorioRetificacao
                .Setup(r => r.ObterPorCodafSuplementarIdAsync(CodafSuplementarId))
                .ReturnsAsync(retificacoesExistentes);

            mapper
                .Setup(m => m.Map<List<CodafSuplementarInscricao>>(dto.Inscritos))
                .Returns(inscritosMapeados);

            mapper
                .Setup(m => m.Map<List<CodafSuplementarAnexo>>(dto.Anexos))
                .Returns(anexosMapeados);

            mapper
                .Setup(m => m.Map<CodafSuplementarRetificacao>(novaRetificacaoDto))
                .Returns(novaRetificacaoMapeada);

            // Act
            var resultado = await casoDeUso.ExecutarAsync(dto, CodafSuplementarId);

            // Assert
            Assert.False(resultado is Erro);

            validator.Verify(
                v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()),
                Times.Once);

            inscritosService.Verify(
                s => s.SalvarInscritosAsync(
                    inscritosMapeados,
                    CodafSuplementarId),
                Times.Once);

            repositorioRetificacao.Verify(
                r => r.Remover(retificacaoParaRemover),
                Times.Once);

            repositorioRetificacao.Verify(
                r => r.Atualizar(retificacaoParaAtualizar),
                Times.Once);

            repositorioRetificacao.Verify(
                r => r.Atualizar(
                    It.Is<CodafSuplementarRetificacao>(
                        item => ObterId(item) == 999)),
                Times.Never);

            repositorioRetificacao.Verify(
                r => r.Inserir(novaRetificacaoMapeada),
                Times.Once);

            Assert.Equal(
                CodafSuplementarId,
                novaRetificacaoMapeada.CodafSuplementarId);

            anexoService.Verify(
                s => s.ProcessarAnexosAsync(
                    CodafSuplementarId,
                    anexosMapeados),
                Times.Once);

            repositorioCodaf.Verify(
                r => r.Atualizar(existente),
                Times.Once);

            Assert.Same(
                inscritosMapeados,
                existente.CodafInscricoes);

            Assert.Same(
                anexosMapeados,
                existente.CodafAnexos);

            transacao.Verify(t => t.Iniciar(), Times.Once);
            transacaoDb.Verify(t => t.Commit(), Times.Once);
            transacaoDb.Verify(t => t.Rollback(), Times.Never);
            transacaoDb.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveRemoverRetificacoesExistentes_QuandoListaEnviadaForNula()
        {
            // Arrange
            var dto = CriarDto();
            dto.Retificacoes = null;

            var existente = CriarCodafSuplementar(CodafSuplementarId);
            var detalhado = CriarCodafSuplementar(CodafSuplementarId);

            var retificacaoExistente =
                CriarRetificacao(CodafSuplementarId, 300);

            ConfigurarFluxoValido(
                dto,
                existente,
                detalhado,
                new List<CodafSuplementarRetificacao> { retificacaoExistente });

            // Act
            var resultado = await casoDeUso.ExecutarAsync(dto, CodafSuplementarId);

            // Assert
            Assert.False(resultado is Erro);

            repositorioRetificacao.Verify(
                r => r.Remover(retificacaoExistente),
                Times.Once);

            repositorioRetificacao.Verify(
                r => r.Atualizar(It.IsAny<CodafSuplementarRetificacao>()),
                Times.Never);

            repositorioRetificacao.Verify(
                r => r.Inserir(It.IsAny<CodafSuplementarRetificacao>()),
                Times.Never);

            transacao.Verify(t => t.Iniciar(), Times.Once);
            transacaoDb.Verify(t => t.Commit(), Times.Once);
            transacaoDb.Verify(t => t.Rollback(), Times.Never);
            transacaoDb.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveProsseguir_QuandoConsultaDetalhadaRetornarNulo()
        {
            // Arrange
            var dto = CriarDto();
            var existente = CriarCodafSuplementar(CodafSuplementarId);

            ConfigurarFluxoValido(
                dto,
                existente,
                detalhado: null,
                retificacoesExistentes: new List<CodafSuplementarRetificacao>());

            // Act
            var resultado = await casoDeUso.ExecutarAsync(dto, CodafSuplementarId);

            // Assert
            Assert.False(resultado is Erro);

            repositorioCodaf.Verify(
                r => r.Atualizar(existente),
                Times.Once);

            transacao.Verify(t => t.Iniciar(), Times.Once);
            transacaoDb.Verify(t => t.Commit(), Times.Once);
            transacaoDb.Verify(t => t.Rollback(), Times.Never);
            transacaoDb.Verify(t => t.Dispose(), Times.Once);
        }

        [Fact]
        public async Task ExecutarAsync_DeveExecutarRollback_QuandoOcorrerExcecao()
        {
            // Arrange
            var dto = CriarDto();
            var existente = CriarCodafSuplementar(CodafSuplementarId);
            var detalhado = CriarCodafSuplementar(CodafSuplementarId);

            ConfigurarFluxoValido(
                dto,
                existente,
                detalhado,
                new List<CodafSuplementarRetificacao>());

            transacaoDb
                .Setup(t => t.Commit())
                .Throws(new InvalidOperationException("Erro simulado no commit"));

            // Act
            var resultado = await casoDeUso.ExecutarAsync(
                dto,
                CodafSuplementarId);

            // Assert
            Assert.Contains(
                "Erro ao salvar CODAF Suplementar",
                ObterConteudoErro(resultado));

            transacao.Verify(
                t => t.Iniciar(),
                Times.Once);

            transacaoDb.Verify(
                t => t.Commit(),
                Times.Once);

            transacaoDb.Verify(
                t => t.Rollback(),
                Times.Once);

            transacaoDb.Verify(
                t => t.Dispose(),
                Times.Once);

            repositorioCodaf.Verify(
                r => r.Atualizar(existente),
                Times.Once);
        }

        private void ConfigurarFluxoValido(
            CodafSuplementarCadastroDto dto,
            CodafSuplementar existente,
            CodafSuplementar? detalhado,
            IList<CodafSuplementarRetificacao> retificacoesExistentes)
        {
            var inscritosMapeados = new List<CodafSuplementarInscricao>();
            var anexosMapeados = new List<CodafSuplementarAnexo>();

            repositorioCodaf
                .Setup(r => r.ObterNaoExcluidosPorIdAsync(CodafSuplementarId))
                .ReturnsAsync(existente);

            repositorioCodaf
                .Setup(r => r.ObterPorIdDetalhadoAsync(CodafSuplementarId))
                .ReturnsAsync(detalhado);

            validator
                .Setup(v => v.ValidateAsync(dto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            repositorioRetificacao
                .Setup(r => r.ObterPorCodafSuplementarIdAsync(CodafSuplementarId))
                .ReturnsAsync(retificacoesExistentes);

            mapper
                .Setup(m => m.Map<List<CodafSuplementarInscricao>>(dto.Inscritos))
                .Returns(inscritosMapeados);

            mapper
                .Setup(m => m.Map<List<CodafSuplementarAnexo>>(dto.Anexos))
                .Returns(anexosMapeados);
        }

        private static CodafSuplementarCadastroDto CriarDto()
        {
            return new CodafSuplementarCadastroDto
            {
                CodafId = 20,
                DataPublicacao = new DateTime(2026, 1, 10),
                DataPublicacaoDom = new DateTime(2026, 1, 11),
                NumeroComunicado = 123,
                PaginaComunicadoDom = 7,
                CodigoCursoEol = 456,
                CodigoNivel = 8,
                Observacao = "Observação para teste",
                Inscritos = new List<CodafSuplementarInscritoSalvarDto>(),
                Retificacoes = new List<CodafSuplementarRetificacaoSalvarDto>(),
                Anexos = new List<CodafAnexoSalvarDto>()
            };
        }

        private static CodafSuplementar CriarCodafSuplementar(long id)
        {
            var codaf = CriarEntidade<CodafSuplementar>();
            DefinirId(codaf, id);
            return codaf;
        }

        private static CodafSuplementarRetificacao CriarRetificacao(
            long codafSuplementarId,
            long id)
        {
            var retificacao = CriarEntidade<CodafSuplementarRetificacao>();

            DefinirId(retificacao, id);
            retificacao.CodafSuplementarId = codafSuplementarId;

            return retificacao;
        }

        private static T CriarEntidade<T>() where T : class
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
            }
            catch (MissingMethodException)
            {
                return (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
            }
        }

        private static void DefinirId(object entidade, long id)
        {
            DefinirPropriedade(entidade, "Id", id);
        }

        private static long ObterId(object entidade)
        {
            var propriedade = LocalizarPropriedade(entidade.GetType(), "Id");

            return propriedade?.GetValue(entidade) is long id
                ? id
                : 0;
        }

        private static void DefinirPropriedade(
            object objeto,
            string nomePropriedade,
            object? valor)
        {
            var propriedade =
                LocalizarPropriedade(objeto.GetType(), nomePropriedade)
                ?? throw new InvalidOperationException(
                    $"A propriedade {nomePropriedade} não foi encontrada.");

            var setter = propriedade.GetSetMethod(nonPublic: true);

            if (setter is not null)
            {
                setter.Invoke(objeto, new[] { valor });
                return;
            }

            var campo = LocalizarCampo(
                objeto.GetType(),
                $"<{nomePropriedade}>k__BackingField");

            if (campo is null)
                throw new InvalidOperationException(
                    $"Não foi possível atribuir a propriedade {nomePropriedade}.");

            campo.SetValue(objeto, valor);
        }

        private static PropertyInfo? LocalizarPropriedade(
            Type tipo,
            string nome)
        {
            for (var tipoAtual = tipo;
                 tipoAtual is not null;
                 tipoAtual = tipoAtual.BaseType)
            {
                var propriedade = tipoAtual.GetProperty(
                    nome,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);

                if (propriedade is not null)
                    return propriedade;
            }

            return null;
        }

        private static FieldInfo? LocalizarCampo(
            Type tipo,
            string nome)
        {
            for (var tipoAtual = tipo;
                 tipoAtual is not null;
                 tipoAtual = tipoAtual.BaseType)
            {
                var campo = tipoAtual.GetField(
                    nome,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly);

                if (campo is not null)
                    return campo;
            }

            return null;
        }

        private static void AdicionarItemNaColecao(
            object objeto,
            string nomePropriedade)
        {
            var propriedade =
                LocalizarPropriedade(objeto.GetType(), nomePropriedade)
                ?? throw new InvalidOperationException(
                    $"A propriedade {nomePropriedade} não foi encontrada.");

            var tipoElemento =
                ObterTipoElementoColecao(propriedade.PropertyType)
                ?? throw new InvalidOperationException(
                    $"Não foi possível determinar o tipo da coleção {nomePropriedade}.");

            var colecao = propriedade.GetValue(objeto);

            if (colecao is null)
            {
                var tipoLista = typeof(List<>).MakeGenericType(tipoElemento);
                colecao = Activator.CreateInstance(tipoLista)!;

                DefinirPropriedade(objeto, nomePropriedade, colecao);
            }

            var item = RuntimeHelpers.GetUninitializedObject(tipoElemento);

            var metodoAdicionar = colecao.GetType().GetMethod(
                "Add",
                new[] { tipoElemento });

            if (metodoAdicionar is null)
                throw new InvalidOperationException(
                    $"A coleção {nomePropriedade} não possui o método Add.");

            metodoAdicionar.Invoke(colecao, new[] { item });
        }

        private static Type? ObterTipoElementoColecao(Type tipoColecao)
        {
            if (tipoColecao.IsArray)
                return tipoColecao.GetElementType();

            var tipos = tipoColecao
                .GetInterfaces()
                .Append(tipoColecao);

            return tipos
                .FirstOrDefault(tipo =>
                    tipo.IsGenericType &&
                    tipo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GetGenericArguments()[0];
        }

        private static string ObterConteudoErro(object erro)
        {
            var valores = new List<string>
            {
                erro.ToString() ?? string.Empty
            };

            foreach (var propriedade in erro.GetType().GetProperties(
                         BindingFlags.Instance | BindingFlags.Public))
            {
                if (propriedade.GetIndexParameters().Length > 0)
                    continue;

                object? valor;

                try
                {
                    valor = propriedade.GetValue(erro);
                }
                catch
                {
                    continue;
                }

                if (valor is string texto)
                {
                    valores.Add(texto);
                    continue;
                }

                if (valor is IEnumerable itens)
                {
                    foreach (var item in itens)
                    {
                        if (item is not null)
                            valores.Add(item.ToString() ?? string.Empty);
                    }
                }
            }

            return string.Join(" ", valores);
        }
    }
}