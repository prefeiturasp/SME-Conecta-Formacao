using Bogus;
using FluentAssertions;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using System.Reflection;

namespace SME.ConectaFormacao.Domino.Teste.Entidades
{
    public class CodafConferenciaTestes
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        public void DadoConstrutorVazio_QuandoInstanciar_EntaoDeveInicializarValoresPadrao()
        {
            // Act
            var sut = new CodafConferencia();

            // Assert
            sut.PropostaId.Should().Be(0);
            sut.PropostaTurmaId.Should().Be(0);
            sut.CodafInscricoes.Should().NotBeNull().And.BeEmpty();
            sut.CodafAnexos.Should().BeNull();
            sut.CodafCertificados.Should().BeNull();
            sut.CertificadoEmitido.Should().BeFalse();
        }

        [Fact]
        public void DadoPropostaETurmaValidas_QuandoInstanciar_EntaoDeveDefinirIdsEStatusIniciado()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var propostaTurmaId = _faker.Random.Long(1, 1000);

            // Act
            var sut = new CodafConferencia(propostaId, propostaTurmaId);

            // Assert
            sut.PropostaId.Should().Be(propostaId);
            sut.PropostaTurmaId.Should().Be(propostaTurmaId);
            sut.Status.Should().Be(StatusCodafConferencia.Iniciado);
            sut.DataFinalizacao.Should().BeNull();
        }

        [Fact]
        public void DadoPropostaTurmaEDadosPublicacao_QuandoInstanciar_EntaoDeveDefinirPropriedades()
        {
            // Arrange
            var propostaId = _faker.Random.Long(1, 1000);
            var propostaTurmaId = _faker.Random.Long(1, 1000);
            var dadosPublicacao = GerarDadosPublicacaoLista();

            // Act
            var sut = new CodafConferencia(propostaId, propostaTurmaId, dadosPublicacao);

            // Assert
            sut.PropostaId.Should().Be(propostaId);
            sut.PropostaTurmaId.Should().Be(propostaTurmaId);
            sut.Status.Should().Be(StatusCodafConferencia.Iniciado);
            sut.DataComunicado.Should().Be(dadosPublicacao.DataPublicacao);
            sut.DataPublicacaoDom.Should().Be(dadosPublicacao.DataPublicacaoDom);
            sut.NumeroComunicado.Should().Be(dadosPublicacao.NumeroComunicado);
            sut.PaginaComunicadoDom.Should().Be(dadosPublicacao.PaginaComunicadoDom);
            sut.CodigoCursoEol.Should().Be(dadosPublicacao.CodigoCursoEol);
            sut.CodigoNivel.Should().Be(dadosPublicacao.CodigoNivel);
            sut.Observacao.Should().Be(dadosPublicacao.Observacao);
        }

        [Fact]
        public void DadoDadosPublicacaoValidos_QuandoAtualizarInformacoes_EntaoDeveAtualizarPropriedades()
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);
            var dadosPublicacao = GerarDadosPublicacaoLista();

            // Act
            sut.AtualizarInformacoes(dadosPublicacao);

            // Assert
            sut.DataComunicado.Should().Be(dadosPublicacao.DataPublicacao);
            sut.DataPublicacaoDom.Should().Be(dadosPublicacao.DataPublicacaoDom);
            sut.NumeroComunicado.Should().Be(dadosPublicacao.NumeroComunicado);
            sut.PaginaComunicadoDom.Should().Be(dadosPublicacao.PaginaComunicadoDom);
            sut.CodigoCursoEol.Should().Be(dadosPublicacao.CodigoCursoEol);
            sut.CodigoNivel.Should().Be(dadosPublicacao.CodigoNivel);
            sut.Observacao.Should().Be(dadosPublicacao.Observacao);
        }

        [Fact]
        public void DadoQualquerStatus_QuandoIniciar_EntaoDeveDefinirStatusComoIniciado()
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), StatusCodafConferencia.Aguardando);

            // Act
            sut.Iniciar();

            // Assert
            sut.Status.Should().Be(StatusCodafConferencia.Iniciado);
        }

        [Fact]
        public void DadoStatusAguardando_QuandoFinalizar_EntaoDeveDefinirStatusComoFinalizadoEDataFinalizacao()
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), StatusCodafConferencia.Aguardando);

            // Act
            sut.Finalizar();

            // Assert
            sut.Status.Should().Be(StatusCodafConferencia.Finalizado);
            sut.EstaFinalizado().Should().BeTrue();
            sut.DataFinalizacao.Should().NotBeNull();
        }

        [Theory]
        [InlineData(StatusCodafConferencia.Iniciado)]
        [InlineData(StatusCodafConferencia.Finalizado)]
        public void DadoStatusDiferenteDeAguardando_QuandoFinalizar_EntaoNaoDeveAlterarStatus(StatusCodafConferencia statusInicial)
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), statusInicial);

            // Act
            sut.Finalizar();

            // Assert
            sut.Status.Should().Be(statusInicial);
            sut.DataFinalizacao.Should().BeNull();
        }

        [Fact]
        public void DadoStatusIniciado_QuandoVerificarEstaFinalizado_EntaoDeveRetornarFalso()
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);

            // Act & Assert
            sut.EstaFinalizado().Should().BeFalse();
        }

        [Theory]
        [InlineData(StatusCodafConferencia.Iniciado, true)]
        [InlineData(StatusCodafConferencia.Aguardando, false)]
        [InlineData(StatusCodafConferencia.Finalizado, false)]
        public void DadoStatus_QuandoVerificarPodeSerExcluido_EntaoDevePermitirApenasSeIniciado(
            StatusCodafConferencia status,
            bool esperado)
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), status);

            // Act
            var podeExcluirSemPerfil = sut.PodeSerExcluido();
            var podeExcluirComPerfil = sut.PodeSerExcluido(Guid.NewGuid());

            // Assert
            podeExcluirSemPerfil.Should().Be(esperado);
            podeExcluirComPerfil.Should().Be(esperado);
        }

        [Fact]
        public void DadoStatusFinalizado_QuandoDefinirStatus_EntaoDeveManterStatusFinalizado()
        {
            // Arrange
            var sut = new CodafConferencia(1, 2);
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), StatusCodafConferencia.Finalizado);

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(StatusCodafConferencia.Finalizado);
        }

        [Fact]
        public void DadoTodosCamposObrigatoriosEParticipacoesPreenchidas_QuandoDefinirStatus_EntaoDeveDefinirComoAguardando()
        {
            // Arrange
            var dadosPublicacao = new DadosPublicacaoLista(null, null, null, null, 12345, null, null);
            var sut = new CodafConferencia(10, 20, dadosPublicacao)
            {
                CodafInscricoes =
                [
                    new CodafConferenciaInscricao { InscricaoId = 1, Participou = true },
                    new CodafConferenciaInscricao { InscricaoId = 2, Participou = false }
                ]
            };

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(StatusCodafConferencia.Aguardando);
        }

        [Theory]
        [InlineData(0, 20, 12345)]
        [InlineData(10, 0, 12345)]
        [InlineData(10, 20, null)]
        public void DadoCamposObrigatoriosAusentes_QuandoDefinirStatus_EntaoDeveDefinirComoIniciado(
            long propostaId,
            long propostaTurmaId,
            int? codigoCursoEol)
        {
            // Arrange
            var dadosPublicacao = new DadosPublicacaoLista(null, null, null, null, codigoCursoEol, null, null);
            var sut = new CodafConferencia(propostaId, propostaTurmaId, dadosPublicacao)
            {
                CodafInscricoes =
                [
                    new CodafConferenciaInscricao { InscricaoId = 1, Participou = true }
                ]
            };
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), StatusCodafConferencia.Aguardando);

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(StatusCodafConferencia.Iniciado);
        }

        [Fact]
        public void DadoInscricoesNulasOuVazias_QuandoDefinirStatus_EntaoDeveDefinirComoIniciado()
        {
            // Arrange
            var dadosPublicacao = new DadosPublicacaoLista(null, null, null, null, 12345, null, null);
            var sutVazio = new CodafConferencia(10, 20, dadosPublicacao)
            {
                CodafInscricoes = []
            };
            var sutNulo = new CodafConferencia(10, 20, dadosPublicacao)
            {
                CodafInscricoes = null!
            };

            // Act
            sutVazio.DefinirStatus();
            sutNulo.DefinirStatus();

            // Assert
            sutVazio.Status.Should().Be(StatusCodafConferencia.Iniciado);
            sutNulo.Status.Should().Be(StatusCodafConferencia.Iniciado);
        }

        [Fact]
        public void DadoInscritoSemParticipacaoPreenchida_QuandoDefinirStatus_EntaoDeveDefinirComoIniciado()
        {
            // Arrange
            var dadosPublicacao = new DadosPublicacaoLista(null, null, null, null, 12345, null, null);
            var sut = new CodafConferencia(10, 20, dadosPublicacao)
            {
                CodafInscricoes =
                [
                    new CodafConferenciaInscricao { InscricaoId = 1, Participou = true },
                    new CodafConferenciaInscricao { InscricaoId = 2, Participou = null }
                ]
            };
            DefinirPropriedadePrivada(sut, nameof(CodafConferencia.Status), StatusCodafConferencia.Aguardando);

            // Act
            sut.DefinirStatus();

            // Assert
            sut.Status.Should().Be(StatusCodafConferencia.Iniciado);
        }

        [Fact]
        public void DadoPropriedadesDeNavegacaoECertificados_QuandoAtribuir_EntaoDeveRefletirValoresCorretamente()
        {
            // Arrange
            var proposta = new Proposta { Id = 1 };
            var turma = new PropostaTurma { Id = 2 };
            var anexo = new CodafConferenciaAnexo
            {
                CodafConferenciaId = 10,
                ArquivoCodigo = Guid.NewGuid(),
                NomeArquivo = "lista.pdf",
                Extensao = ".pdf",
                TipoAnexoId = TipoAnexoCodaf.ListaPresenca
            };
            var inscricao = new CodafConferenciaInscricao
            {
                CodafConferenciaId = 10,
                InscricaoId = 99,
                Participou = true,
                Inscricao = new Inscricao { Id = 99 }
            };

            var sut = new CodafConferencia(1, 2)
            {
                Proposta = proposta,
                PropostaTurma = turma,
                CodafAnexos = [anexo],
                CodafInscricoes = [inscricao],
                CodafCertificados = []
            };
            anexo.CodafConferencia = sut;
            inscricao.CodafConferencia = sut;

            // Assert
            sut.Proposta.Should().BeSameAs(proposta);
            sut.PropostaTurma.Should().BeSameAs(turma);
            sut.CodafAnexos.Should().ContainSingle().Which.Should().BeSameAs(anexo);
            sut.CertificadoEmitido.Should().BeFalse();

            var certificado = (CodafCertificado)Activator.CreateInstance(typeof(CodafCertificado), true)!;
            sut.CodafCertificados = [certificado];
            sut.CertificadoEmitido.Should().BeTrue();

            anexo.CodafConferenciaId.Should().Be(10);
            anexo.CodafConferencia.Should().BeSameAs(sut);
            anexo.ArquivoCodigo.Should().NotBeEmpty();
            anexo.NomeArquivo.Should().Be("lista.pdf");
            anexo.Extensao.Should().Be(".pdf");
            anexo.TipoAnexoId.Should().Be(TipoAnexoCodaf.ListaPresenca);

            inscricao.CodafConferenciaId.Should().Be(10);
            inscricao.CodafConferencia.Should().BeSameAs(sut);
            inscricao.InscricaoId.Should().Be(99);
            inscricao.Participou.Should().BeTrue();
            inscricao.Inscricao.Should().NotBeNull();
        }

        private static DadosPublicacaoLista GerarDadosPublicacaoLista() => new Faker<DadosPublicacaoLista>("pt_BR")
            .CustomInstantiator(f => new DadosPublicacaoLista(
                f.Date.Recent(),
                f.Date.Recent(),
                f.Random.Short(1, 1000),
                f.Random.Short(1, 100),
                f.Random.Int(1, 9999),
                f.Random.Int(1, 10),
                f.Lorem.Sentence()
            )).Generate();

        private static void DefinirPropriedadePrivada<T>(T instancia, string nomePropriedade, object valor)
        {
            var prop = typeof(T).GetProperty(nomePropriedade, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop?.SetValue(instancia, valor, null);
        }
    }
}
