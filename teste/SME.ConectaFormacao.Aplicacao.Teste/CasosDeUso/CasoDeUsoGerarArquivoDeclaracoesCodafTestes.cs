using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafDeclaracoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Email;
using SME.ConectaFormacao.Aplicacao.Interfaces.Utilitarios;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos;
using SME.ConectaFormacao.Infra.Dados.Estrategias.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Armazenamento.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Relatorio;
using SME.ConectaFormacao.Infra.Servicos.Relatorio.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public partial class CasoDeUsoGerarArquivoDeclaracoesCodafTestes
    {

        private readonly Mock<IServicoRelatorio> _servicoRelatorio;
        private readonly Mock<IRepositorioCodafDeclaracao> _repositorio;
        private readonly Mock<IServicoArmazenamento> _servicoArmazenamento;
        private readonly Mock<IKeyedServiceProvider> _serviceProvider;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IUtilitariosCodaf> _utilitarios;
        private readonly Mock<IDeclaracaoCodafGeradorConteudo> _geradorDeclaracao;

        private readonly CasoDeUsoGerarArquivoDeclaracoesCodaf _casoDeUso;

        public CasoDeUsoGerarArquivoDeclaracoesCodafTestes()
        {
            _servicoRelatorio = new Mock<IServicoRelatorio>();
            _repositorio = new Mock<IRepositorioCodafDeclaracao>();
            _servicoArmazenamento = new Mock<IServicoArmazenamento>();
            _serviceProvider = new Mock<IKeyedServiceProvider>();
            _configuration = new Mock<IConfiguration>();
            _utilitarios = new Mock<IUtilitariosCodaf>();
            _geradorDeclaracao = new Mock<IDeclaracaoCodafGeradorConteudo>();

            _repositorio.SetReturnsDefault(Task.CompletedTask);
            _utilitarios.SetReturnsDefault(Task.CompletedTask);

            _configuration
                .Setup(x => x["UrlFrontEnd"])
                .Returns("https://conecta-formacao.teste/");

            _serviceProvider
                .Setup(x => x.GetRequiredKeyedService(
                    typeof(IDeclaracaoCodafGeradorConteudo),
                    It.IsAny<object>()))
                .Returns(_geradorDeclaracao.Object);

            _geradorDeclaracao
                .Setup(x => x.GerarConteudoEmail(
                    It.IsAny<DadosProcessamentoCodafDto>(),
                    It.IsAny<string>()))
                .Returns(("Declaração disponível", "Sua declaração está disponível."));

            _casoDeUso = new CasoDeUsoGerarArquivoDeclaracoesCodaf(
                _servicoRelatorio.Object,
                _repositorio.Object,
                _servicoArmazenamento.Object,
                _serviceProvider.Object,
                _configuration.Object,
                _utilitarios.Object);
        }

        [Fact]
        public async Task Executar_QuandoNaoExistemDeclaracoes_DeveFinalizarComSucesso()
        {
            // Arrange
            _repositorio
                .Setup(x => x.ObterDeclaracoesParaProcessamentoAsync())
                .ReturnsAsync([]);

            // Act
            var resultado = await _casoDeUso.Executar(null!);

            // Assert
            Assert.True(resultado);

            _repositorio.Verify(
                x => x.ObterDeclaracoesParaProcessamentoAsync(),
                Times.Once);

            _servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(It.IsAny<HtmlCodafDto>()),
                Times.Never);

            _serviceProvider.Verify(
                x => x.GetRequiredKeyedService(
                    It.IsAny<Type>(),
                    It.IsAny<object>()),
                Times.Never);

            Assert.Contains(
                _utilitarios.Invocations,
                x =>
                    x.Method.Name == nameof(IUtilitariosCodaf.SalvarLogAsync) &&
                    x.Arguments.Any(a =>
                        a is string mensagem &&
                        mensagem.Contains("Início do processamento")));

            Assert.Contains(
                _utilitarios.Invocations,
                x =>
                    x.Method.Name == nameof(IUtilitariosCodaf.SalvarLogAsync) &&
                    x.Arguments.Any(a =>
                        a is string mensagem &&
                        mensagem.Contains(
                            "https://conecta-formacao.teste/declaracoes")));

            Assert.Contains(
                _utilitarios.Invocations,
                x =>
                    x.Method.Name == nameof(IUtilitariosCodaf.SalvarLogAsync) &&
                    x.Arguments.Any(a =>
                        a is string mensagem &&
                        mensagem.Contains("Fim do processamento")));
        }

        [Fact]
        public async Task Executar_QuandoExistemDeclaracoes_DeveProcessarTodasEEnviarEmailSomenteParaQuemPossuiEmail()
        {
            // Arrange
            var declaracaoComEmail = CriarDeclaracao(
                id: 1,
                codigo: "DEC-001",
                email: "usuario@teste.com");

            var declaracaoSemEmail = CriarDeclaracao(
                id: 2,
                codigo: "DEC-002",
                email: string.Empty);

            var lote = new List<DadosProcessamentoCodafDto>
        {
            declaracaoComEmail,
            declaracaoSemEmail
        };

            _repositorio
                .SetupSequence(x => x.ObterDeclaracoesParaProcessamentoAsync())
                .ReturnsAsync(lote)
                .ReturnsAsync([]);

            var arquivoPdf = new byte[] { 1, 2, 3, 4 };

            _servicoRelatorio
                .Setup(x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>()))
                .ReturnsAsync(arquivoPdf);

            _servicoArmazenamento
                .SetupSequence(x => x.UploadCodafAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()))
                .ReturnsAsync("codaf/001/arquivo.pdf")
                .ReturnsAsync("codaf/002/arquivo.pdf");

            // Act
            var resultado = await _casoDeUso.Executar(null!);

            // Assert
            Assert.True(resultado);

            /*
             * Uma chamada devolve o lote.
             * A segunda devolve vazio e encerra o while.
             */
            _repositorio.Verify(
                x => x.ObterDeclaracoesParaProcessamentoAsync(),
                Times.Exactly(2));

            _servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>()),
                Times.Exactly(2));

            _servicoArmazenamento.Verify(
                x => x.UploadCodafAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()),
                Times.Exactly(2));

            _serviceProvider.Verify(
                x => x.GetRequiredKeyedService(
                    typeof(IDeclaracaoCodafGeradorConteudo),
                    It.IsAny<object>()),
                Times.Exactly(2));

            _geradorDeclaracao.Verify(
                x => x.GerarConteudoEmail(
                    It.IsAny<DadosProcessamentoCodafDto>(),
                    "https://conecta-formacao.teste/declaracoes"),
                Times.Exactly(2));

            ValidarAtualizacoesComSucesso(
                declaracaoComEmail,
                declaracaoSemEmail);

            ValidarNomesDosArquivosGerados(
                declaracaoComEmail,
                declaracaoSemEmail);

            ValidarEmailEnviado(declaracaoComEmail);
        }

        [Fact]
        public async Task Executar_QuandoOcorrerErroAoGerarPdf_DeveAtualizarStatusComErroEContinuarProcessamento()
        {
            // Arrange
            var declaracao = CriarDeclaracao(
                id: 10,
                codigo: "DEC-ERRO-001",
                email: "usuario@teste.com");

            var lote = new List<DadosProcessamentoCodafDto>
        {
            declaracao
        };

            _repositorio
                .SetupSequence(x => x.ObterDeclaracoesParaProcessamentoAsync())
                .ReturnsAsync(lote)
                .ReturnsAsync([]);

            var excecao = new InvalidOperationException(
                "Erro proposital ao gerar PDF");

            _servicoRelatorio
                .Setup(x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>()))
                .ThrowsAsync(excecao);

            // Act
            var resultado = await _casoDeUso.Executar(null!);

            // Assert
            Assert.True(resultado);

            _servicoRelatorio.Verify(
                x => x.ConveterHtmlCodafParaPdfAsync(
                    It.IsAny<HtmlCodafDto>()),
                Times.Once);

            /*
             * O erro aconteceu antes do upload.
             */
            _servicoArmazenamento.Verify(
                x => x.UploadCodafAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>()),
                Times.Never);

            /*
             * Uma declaração com erro não é adicionada à coleção
             * declaracoesProcessadas.
             */
            _serviceProvider.Verify(
                x => x.GetRequiredKeyedService(
                    It.IsAny<Type>(),
                    It.IsAny<object>()),
                Times.Never);

            _geradorDeclaracao.Verify(
                x => x.GerarConteudoEmail(
                    It.IsAny<DadosProcessamentoCodafDto>(),
                    It.IsAny<string>()),
                Times.Never);

            ValidarAtualizacaoComErro(declaracao, excecao);

            ValidarLogCritico(declaracao, excecao);

            /*
             * Mesmo não havendo declarações processadas com sucesso,
             * o código chama EnviarEmailsAsync com uma coleção vazia.
             */
            var chamadaEnvioEmail = _utilitarios.Invocations.Single(
                x => x.Method.Name ==
                     nameof(IUtilitariosCodaf.EnviarEmailsAsync));

            var emails = Assert.IsType<IEnumerable<EnviarEmailDto>>(
                chamadaEnvioEmail.Arguments[0], exactMatch: false);

            Assert.Empty(emails);
        }

        private void ValidarAtualizacoesComSucesso(
            params DadosProcessamentoCodafDto[] declaracoes)
        {
            var atualizacoes = _repositorio.Invocations
                .Where(x =>
                    x.Method.Name ==
                    nameof(
                        IRepositorioCodafDeclaracao
                            .AtualizarStatusProcessamentoAsync))
                .ToList();

            Assert.Equal(declaracoes.Length, atualizacoes.Count);

            foreach (var declaracao in declaracoes)
            {
                var atualizacao = atualizacoes.Single(
                    x => Equals(x.Arguments[0], declaracao.Id));

                Assert.Equal(
                    StatusProcessamentoDeclaracaoCodaf.ProcessadoComSucesso,
                    atualizacao.Arguments[1]);

                var chaveArmazenamento =
                    Assert.IsType<string>(atualizacao.Arguments[2]);

                Assert.StartsWith("codaf/", chaveArmazenamento);
                Assert.EndsWith("arquivo.pdf", chaveArmazenamento);

                Assert.Null(atualizacao.Arguments[3]);
            }
        }

        private void ValidarAtualizacaoComErro(
            DadosProcessamentoCodafDto declaracao,
            Exception excecao)
        {
            var atualizacao = _repositorio.Invocations.Single(
                x =>
                    x.Method.Name ==
                    nameof(
                        IRepositorioCodafDeclaracao
                            .AtualizarStatusProcessamentoAsync));

            Assert.Equal(declaracao.Id, atualizacao.Arguments[0]);

            Assert.Equal(
                StatusProcessamentoDeclaracaoCodaf.ProcessadoComErro,
                atualizacao.Arguments[1]);

            Assert.Null(atualizacao.Arguments[2]);

            Assert.Equal(
                excecao.Message,
                atualizacao.Arguments[3]);
        }

        private void ValidarLogCritico(
            DadosProcessamentoCodafDto declaracao,
            Exception excecao)
        {
            var chamadaLog = _utilitarios.Invocations.Single(
                x =>
                    x.Method.Name ==
                        nameof(IUtilitariosCodaf.SalvarLogAsync) &&
                    x.Arguments.Any(a =>
                        a is string mensagem &&
                        mensagem.Contains(
                            $"Erro ao processar declaração Codaf com Id {declaracao.Id}")));

            Assert.Contains(
                declaracao.CodigoDeclaracaoOuCertificado.ToString(),
                chamadaLog.Arguments[0]?.ToString() ?? "");

            Assert.Equal(
                LogNivel.Critico,
                chamadaLog.Arguments[1]);

            Assert.Same(
                excecao,
                chamadaLog.Arguments[2]);
        }

        private void ValidarEmailEnviado(
            DadosProcessamentoCodafDto declaracaoComEmail)
        {
            /*
             * Inspecionar Mock.Invocations aqui evita acoplamento
             * desnecessário ao tipo concreto do parâmetro de
             * EnviarEmailsAsync (List, IEnumerable etc.).
             */
            var chamada = _utilitarios.Invocations.Single(
                x => x.Method.Name ==
                     nameof(IUtilitariosCodaf.EnviarEmailsAsync));

            var emails =
                Assert.IsType<IEnumerable<EnviarEmailDto>>(
                    chamada.Arguments[0], exactMatch: false)
                .ToList();

            var email = Assert.Single(emails);

            Assert.Equal(
                declaracaoComEmail.EmailUsuario,
                email.EmailDestinatario);

            Assert.Equal(
                declaracaoComEmail.NomeCompleto,
                email.NomeDestinatario);

            Assert.Equal(
                "Declaração disponível",
                email.Titulo);

            Assert.Equal(
                "Sua declaração está disponível.",
                email.Texto);
        }

        private void ValidarNomesDosArquivosGerados(
            params DadosProcessamentoCodafDto[] declaracoes)
        {
            var uploads = _servicoArmazenamento.Invocations
                .Where(x =>
                    x.Method.Name ==
                    nameof(IServicoArmazenamento.UploadCodafAsync))
                .ToList();

            Assert.Equal(declaracoes.Length, uploads.Count);

            foreach (var declaracao in declaracoes)
            {
                var upload = uploads.FirstOrDefault(
                    x =>
                    {
                        var nomeArquivo =
                            x.Arguments[0]?.ToString();

                        return nomeArquivo?.Contains(
                            declaracao.CodigoDeclaracaoOuCertificado.ToString()) ==
                               true;
                    });

                Assert.NotNull(upload);

                var nomeArquivo =
                    Assert.IsType<string>(upload.Arguments[0]);

                Assert.Contains(
                    declaracao.CodigoDeclaracaoOuCertificado.ToString(),
                    nomeArquivo);

                Assert.EndsWith(
                    ".pdf",
                    nomeArquivo,
                    StringComparison.OrdinalIgnoreCase);

                /*
                 * yyyy/MM/CODIGO-{guid}.pdf
                 */
                var partes = nomeArquivo.Split('/');

                Assert.True(partes.Length >= 3);
            }
        }

        private static DadosProcessamentoCodafDto CriarDeclaracao(
            int id,
            string codigo,
            string email)
        {
            var numeroStr = MyRegex().Replace(codigo, "");

            return new DadosProcessamentoCodafDto
            {
                Id = id,
                CodigoDeclaracaoOuCertificado = long.Parse(numeroStr),
                HtmlContentSnapshot = """
                                  <html>
                                    <body>
                                      <h1>Declaração CODAF</h1>
                                      <p>Documento de teste</p>
                                    </body>
                                  </html>
                                  """,
                Emissor = "CODAF",
                EmailUsuario = email,
                NomeCompleto = $"Usuário {id}"
            };
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"[^\d]")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();
    }
}
