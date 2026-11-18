using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using SME.ConectaFormacao.Aplicacao.Dtos.Codaf;
using SME.ConectaFormacao.Aplicacao.Dtos.CodafConferencias;
using SME.ConectaFormacao.Aplicacao.Mapeamentos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Dtos.CodafConferencias;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.CodafConferencias
{
    public class CodafConferenciaDtosAplicacaoTestes
    {
        [Fact]
        public void DadoDtosDeCodafConferencia_QuandoAtribuirPropriedades_EntaoDevemManterValores()
        {
            // Arrange
            var agora = DateTime.UtcNow;
            var guid = Guid.NewGuid();

            var anexoDto = new CodafConferenciaAnexoDto
            {
                Id = 1,
                CodafConferenciaId = 10,
                ArquivoCodigo = guid,
                NomeArquivo = "ata.pdf",
                Extensao = ".pdf",
                TipoAnexoId = TipoAnexoCodaf.ListaPresenca,
                UrlDownload = "https://url",
                CriadoEm = agora,
                CriadoPor = "User",
                CriadoLogin = "user.login",
                AlteradoEm = agora,
                AlteradoPor = "User 2",
                AlteradoLogin = "user2.login"
            };

            var inscritoDto = new CodafConferenciaInscritoDto
            {
                Id = 2,
                InscricaoId = 20,
                CodafConferenciaId = 10,
                Documento = "1234567",
                Nome = "Maria",
                Participou = true
            };

            var detalhadoDto = new CodafConferenciaDetalhadoDto
            {
                Id = 10,
                PropostaId = 100,
                PropostaTurmaId = 200,
                NumeroComunicado = 12,
                DataComunicado = agora,
                PaginaComunicadoDom = 34,
                DataPublicacaoDom = agora,
                CodigoCursoEol = 567,
                CodigoNivel = 1,
                Observacao = "Obs",
                Status = StatusCodafConferencia.Aguardando,
                DataFinalizacao = agora,
                AlteradoEm = agora,
                AlteradoPor = "User",
                AlteradoLogin = "user",
                CriadoEm = agora,
                CriadoPor = "User",
                CriadoLogin = "user",
                NomeFormacao = "Formação",
                CodigoFormacao = 100,
                NumeroHomologacao = 20261,
                CertificadoEmitido = false,
                Anexos = [anexoDto],
                Inscritos = [inscritoDto],
                Turma = new PropostaTurmaComCodafDto { Id = 200, Descricao = "Turma A" }
            };

            var resumoDto = new CodafConferenciaResumoDto
            {
                Id = 10,
                NumeroHomologacao = 20261,
                NomeFormacao = "Formação",
                CodigoFormacao = 100,
                NomeTurma = "Turma A",
                NomeAreaPromotora = "DRE",
                Status = StatusCodafConferencia.Iniciado,
                StatusCertificacaoTurma = StatusCertificacaoTurma.PendenteEmissaoRemessa,
                CodigoCursoEol = 567,
                CodigoNivel = 1,
                PossuiAprovacoes = true,
                CriadoEm = agora
            };

            // Assert
            anexoDto.Id.Should().Be(1);
            anexoDto.CodafConferenciaId.Should().Be(10);
            anexoDto.ArquivoCodigo.Should().Be(guid);
            anexoDto.NomeArquivo.Should().Be("ata.pdf");
            anexoDto.Extensao.Should().Be(".pdf");
            anexoDto.TipoAnexoId.Should().Be(TipoAnexoCodaf.ListaPresenca);
            anexoDto.UrlDownload.Should().Be("https://url");
            anexoDto.CriadoEm.Should().Be(agora);
            anexoDto.CriadoPor.Should().Be("User");
            anexoDto.CriadoLogin.Should().Be("user.login");
            anexoDto.AlteradoEm.Should().Be(agora);
            anexoDto.AlteradoPor.Should().Be("User 2");
            anexoDto.AlteradoLogin.Should().Be("user2.login");

            inscritoDto.Id.Should().Be(2);
            inscritoDto.InscricaoId.Should().Be(20);
            inscritoDto.CodafConferenciaId.Should().Be(10);
            inscritoDto.Documento.Should().Be("1234567");
            inscritoDto.Nome.Should().Be("Maria");
            inscritoDto.Participou.Should().BeTrue();

            detalhadoDto.DataFinalizacao.Should().Be(agora);
            detalhadoDto.NomeFormacao.Should().Be("Formação");
            detalhadoDto.NumeroHomologacao.Should().Be(20261);
            detalhadoDto.CertificadoEmitido.Should().BeFalse();
            detalhadoDto.Inscritos.Should().ContainSingle();
            detalhadoDto.Turma!.Id.Should().Be(200);

            resumoDto.NumeroHomologacao.Should().Be(20261);
            resumoDto.NomeFormacao.Should().Be("Formação");
            resumoDto.CodigoFormacao.Should().Be(100);
            resumoDto.Status.Should().Be(StatusCodafConferencia.Iniciado);
            resumoDto.StatusCertificacaoTurma.Should().Be(StatusCertificacaoTurma.PendenteEmissaoRemessa);
            resumoDto.CodigoCursoEol.Should().Be(567);
            resumoDto.CodigoNivel.Should().Be(1);
            resumoDto.PossuiAprovacoes.Should().BeTrue();
            resumoDto.CriadoEm.Should().Be(agora);
        }

        [Fact]
        public void DadoListagemResultadoCodafConferenciaDto_QuandoMapearParaCodafConferenciaResumoDto_EntaoDeveMapearStatusCertificacaoTurma()
        {
            // Arrange
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CodafConferenciaProfile>(), new NullLoggerFactory());
            var mapper = config.CreateMapper();
            var agora = DateTime.UtcNow;

            var origem = new ListagemResultadoCodafConferenciaDto
            {
                Id = 15,
                NumeroHomologacao = 20262,
                NomeFormacao = "Conferência Municipal",
                CodigoFormacao = 446,
                NomeTurma = "Turma Única",
                NomeAreaPromotora = "COPED",
                Status = StatusCodafConferencia.Aguardando,
                StatusCertificacaoTurma = StatusCertificacaoTurma.DisponivelParaEmissao,
                CodigoCursoEol = 1234,
                CodigoNivel = 2,
                PossuiAprovacoes = true,
                CriadoEm = agora
            };

            // Act
            var destino = mapper.Map<CodafConferenciaResumoDto>(origem);

            // Assert
            destino.Id.Should().Be(15);
            destino.Status.Should().Be(StatusCodafConferencia.Aguardando);
            destino.StatusCertificacaoTurma.Should().Be(StatusCertificacaoTurma.DisponivelParaEmissao);
            destino.CodigoCursoEol.Should().Be(1234);
            destino.PossuiAprovacoes.Should().BeTrue();
            destino.CriadoEm.Should().Be(agora);
        }
    }
}
