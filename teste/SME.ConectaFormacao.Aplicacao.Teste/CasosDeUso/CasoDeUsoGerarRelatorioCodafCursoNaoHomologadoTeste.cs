// Aplicacao.Teste/CasosDeUso/CasoDeUsoGerarRelatorioCodafCursoNaoHomologadoTeste.cs

using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Aplicacao.Interfaces.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafCursosNaoHomologados;
using SME.ConectaFormacao.Infra.Dados.Relatorios;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoGerarRelatorioCodafCursoNaoHomologadoTeste
    {
        private readonly AutoMocker mocker;
        private readonly CasoDeUsoGerarRelatorioCodafCursoNaoHomologado casoDeUso;
        private readonly Faker faker;

        private const int STATUS_DECLARACAO_EMITIDA = 4;

        public CasoDeUsoGerarRelatorioCodafCursoNaoHomologadoTeste()
        {
            mocker = new AutoMocker();
            casoDeUso = mocker.CreateInstance<CasoDeUsoGerarRelatorioCodafCursoNaoHomologado>();
            faker = new Faker("pt_BR");
        }

        [Fact(DisplayName = "Gerar Relatório CODAF Não Homologado - Deve retornar NaoEncontrado quando o codaf não existir")]
        public async Task Deve_Retornar_NaoEncontrado_Quando_Codaf_Nao_Existir()
        {
            var id = faker.Random.Long(1);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterPorIdDetalhadoAsync(id))
                .ReturnsAsync((CodafCursoNaoHomologado?)null);

            var resultado = await casoDeUso.ExecutarAsync(id);

            resultado.Sucesso.Should().BeFalse();

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.ObterStatusDeclaracaoTurmaAsync(It.IsAny<long>()), Times.Never);
            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.ObterDadosRelatorioAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact(DisplayName = "Gerar Relatório CODAF Não Homologado - Deve retornar erro de negócio quando usuário não é administrador nem criador do registro")]
        public async Task Deve_Retornar_ErroNegocio_Quando_Usuario_Nao_For_Administrador_Nem_Criador()
        {
            var id = faker.Random.Long(1);
            var codaf = CriarCodaf(criadoLogin: "outro.usuario");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterPorIdDetalhadoAsync(id))
                .ReturnsAsync(codaf);

            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.EhAdministrador).Returns(false);
            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.UsuarioLogado).Returns("usuario.logado");

            var resultado = await casoDeUso.ExecutarAsync(id);

            resultado.Sucesso.Should().BeFalse();

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.ObterStatusDeclaracaoTurmaAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact(DisplayName = "Gerar Relatório CODAF Não Homologado - Deve permitir quando usuário não é criador mas é administrador")]
        public async Task Deve_Permitir_Quando_Usuario_For_Administrador_Ainda_Que_Nao_Seja_Criador()
        {
            var id = faker.Random.Long(1);
            var codaf = CriarCodaf(criadoLogin: "outro.usuario");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterPorIdDetalhadoAsync(id))
                .ReturnsAsync(codaf);

            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.EhAdministrador).Returns(true);
            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.UsuarioLogado).Returns("usuario.logado");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterStatusDeclaracaoTurmaAsync(id))
                .ReturnsAsync(STATUS_DECLARACAO_EMITIDA);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterDadosRelatorioAsync(id))
                .ReturnsAsync((DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto?)null); // sem dados -> caminho de NaoEncontrado, apenas para validar que passou da checagem de permissão

            await casoDeUso.ExecutarAsync(id);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.ObterStatusDeclaracaoTurmaAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Gerar Relatório CODAF Não Homologado - Deve retornar erro de negócio quando a declaração ainda não foi emitida")]
        public async Task Deve_Retornar_ErroNegocio_Quando_Declaracao_Nao_Foi_Emitida()
        {
            var id = faker.Random.Long(1);
            var codaf = CriarCodaf(criadoLogin: "usuario.logado");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterPorIdDetalhadoAsync(id))
                .ReturnsAsync(codaf);

            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.EhAdministrador).Returns(false);
            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.UsuarioLogado).Returns("usuario.logado");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterStatusDeclaracaoTurmaAsync(id))
                .ReturnsAsync(STATUS_DECLARACAO_EMITIDA - 1);

            var resultado = await casoDeUso.ExecutarAsync(id);

            resultado.Sucesso.Should().BeFalse();

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.ObterDadosRelatorioAsync(It.IsAny<long>()), Times.Never);
            mocker.GetMock<IGeradorRelatorioCodafCursoNaoHomologadoExcelService>()
                .Verify(s => s.GerarRelatorio(It.IsAny<DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto>()), Times.Never);
        }

        [Fact(DisplayName = "Gerar Relatório CODAF Não Homologado - Deve retornar NaoEncontrado quando não houver dados para o relatório")]
        public async Task Deve_Retornar_NaoEncontrado_Quando_Dados_Relatorio_For_Nulo()
        {
            var id = faker.Random.Long(1);
            var codaf = CriarCodaf(criadoLogin: "usuario.logado");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterPorIdDetalhadoAsync(id))
                .ReturnsAsync(codaf);

            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.EhAdministrador).Returns(false);
            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.UsuarioLogado).Returns("usuario.logado");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterStatusDeclaracaoTurmaAsync(id))
                .ReturnsAsync(STATUS_DECLARACAO_EMITIDA);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterDadosRelatorioAsync(id))
                .ReturnsAsync((DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto?)null);

            var resultado = await casoDeUso.ExecutarAsync(id);

            resultado.Sucesso.Should().BeFalse();

            mocker.GetMock<IGeradorRelatorioCodafCursoNaoHomologadoExcelService>()
                .Verify(s => s.GerarRelatorio(It.IsAny<DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto>()), Times.Never);
            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.Atualizar(It.IsAny<CodafCursoNaoHomologado>()), Times.Never);
        }

        [Fact(DisplayName = "Gerar Relatório CODAF Não Homologado - Deve gerar o arquivo, finalizar o codaf e atualizar quando tudo estiver correto")]
        public async Task Deve_Gerar_Arquivo_Finalizar_E_Atualizar_Codaf_Quando_Tudo_Correto()
        {
            var id = faker.Random.Long(1);
            var numeroHomologacao = faker.Random.Long(1, 9999);
            var nomeTurma = faker.Random.Word();
            var codaf = CriarCodaf(criadoLogin: "usuario.logado", numeroHomologacao: numeroHomologacao, nomeTurma: nomeTurma);

            var dadosRelatorio = new DadosPrincipaisRelatorioCodafCursoNaoHomologadoDto();
            var arquivoBytes = faker.Random.Bytes(10);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterPorIdDetalhadoAsync(id))
                .ReturnsAsync(codaf);

            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.EhAdministrador).Returns(false);
            mocker.GetMock<IContextoAplicacao>()
                .SetupGet(c => c.UsuarioLogado).Returns("usuario.logado");

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterStatusDeclaracaoTurmaAsync(id))
                .ReturnsAsync(STATUS_DECLARACAO_EMITIDA);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.ObterDadosRelatorioAsync(id))
                .ReturnsAsync(dadosRelatorio);

            mocker.GetMock<IGeradorRelatorioCodafCursoNaoHomologadoExcelService>()
                .Setup(s => s.GerarRelatorio(dadosRelatorio))
                .Returns(arquivoBytes);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Setup(r => r.Atualizar(codaf))
                .ReturnsAsync(codaf);

            var resultado = await casoDeUso.ExecutarAsync(id);

            resultado.Sucesso.Should().BeTrue();

            var arquivo = resultado.Dados;
            arquivo.Should().NotBeNull();
            arquivo!.Nome.Should().Be($"CODAF_{numeroHomologacao}_{nomeTurma}.xlsx");
            arquivo.ContentType.Should().Be("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

            mocker.GetMock<IGeradorRelatorioCodafCursoNaoHomologadoExcelService>()
                .Verify(s => s.GerarRelatorio(dadosRelatorio), Times.Once);

            mocker.GetMock<IRepositorioCodafCursoNaoHomologado>()
                .Verify(r => r.Atualizar(codaf), Times.Once);
        }

        /// <summary>
        /// Cria uma instância de CodafCursoNaoHomologado com os dados mínimos necessários para o caso de uso,
        /// usando o construtor público e os setters públicos de CriadoLogin (herdado de EntidadeBaseAuditavel),
        /// Proposta e PropostaTurma.
        /// </summary>
        private static CodafCursoNaoHomologado CriarCodaf(
            string criadoLogin,
            long numeroHomologacao = 123,
            string nomeTurma = "Turma Teste")
        {
            return new CodafCursoNaoHomologado
            {
                CriadoLogin = criadoLogin,
                Proposta = new Proposta { NumeroHomologacao = numeroHomologacao },
                PropostaTurma = new PropostaTurma { Nome = nomeTurma }
            };
        }
    }
}