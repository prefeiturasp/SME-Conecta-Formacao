using DocumentFormat.OpenXml.Packaging;
using FluentAssertions;
using Moq.AutoMock;
using SME.ConectaFormacao.Infra.Dados.Dtos.Propostas;
using SME.ConectaFormacao.Infra.Dados.Servicos;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Servicos
{
    public class GeradorLaudaDocxServiceTeste
    {
        private readonly GeradorLaudaDocxService _sut;

        public GeradorLaudaDocxServiceTeste()
        {
            var mocker = new AutoMocker();
            _sut = mocker.CreateInstance<GeradorLaudaDocxService>();
        }

        [Fact]
        public async Task DadoDadosValidos_QuandoGerarArquivo_EntaoRetornaArrayDeBytesNaoVazio()
        {
            // Arrange
            var dados = new PropostaLaudaCompletaDto
            {
                NomeFormacao = "Formação de Teste",
                Justificativa = "<p>Teste Justificativa</p>",
                Objetivos = "Objetivos",
                ConteudoProgramatico = "Conteúdo",
                Procedimentos = "Procedimentos",
                DescricaoAtividade = "Atividade Obrigatória",
                CargaHorariaPresencial = "04:00",
                CargaHorariaDistancia = "02:00",
                CargaHorariaSincrona = "01:00",
                VagasRemanecentes = new List<PropostaPublicoAlvoDto> { new PropostaPublicoAlvoDto { Nome = "Diretor" } },
                CriteriosValidacao = new List<PropostaPublicoAlvoDto> { new PropostaPublicoAlvoDto { Nome = "Outros" } },
                CriteriosValidacao_Outros = "Teste critério",
                CriteriosCertificacao = new List<PropostaPublicoAlvoDto> { new PropostaPublicoAlvoDto { Nome = "Participação" } },
                PublicosAlvo = new List<PropostaPublicoAlvoDto> { new PropostaPublicoAlvoDto { Nome = "Professor" } },
                FuncaoEspecifica = new List<PropostaPublicoAlvoDto> { new PropostaPublicoAlvoDto { Nome = "Coordenador" } },
                Regentes = new List<RegenteLaudaDto> { new RegenteLaudaDto { Nome = "João" } }
            };

            // Act
            var resultado = await _sut.GerarArquivoLaudaCompletaAsync(dados);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().NotBeEmpty();
            
            using var stream = new MemoryStream(resultado);
            using var word = WordprocessingDocument.Open(stream, false);
            var conteudo = word.MainDocumentPart?.Document.Body?.InnerText;

            conteudo.Should().NotBeNull();
            conteudo.Should().Contain("Formação de Teste");
            conteudo.Should().Contain("07:00"); 
        }

        [Fact]
        public async Task DadoVagasRemanescentesVazia_QuandoGerarArquivo_EntaoRemoveTabelaCorrespondente()
        {
            // Arrange
            var dados = new PropostaLaudaCompletaDto
            {
                NomeFormacao = "Teste Sem Vagas",
                VagasRemanecentes = new List<PropostaPublicoAlvoDto>()
            };

            // Act
            var resultado = await _sut.GerarArquivoLaudaCompletaAsync(dados);

            // Assert
            using var stream = new MemoryStream(resultado);
            using var word = WordprocessingDocument.Open(stream, false);
            var conteudo = word.MainDocumentPart?.Document.Body?.InnerText;

            conteudo.Should().NotContain("{{VAGAS_REMANESCENTES}}");
        }
        
        [Fact]
        public async Task DadoCargaHorariaPresencialZerada_QuandoGerarArquivo_EntaoRemoveTabelaDeCargaPresencial()
        {
            // Arrange
            var dados = new PropostaLaudaCompletaDto
            {
                NomeFormacao = "Teste Sem Carga Presencial",
                CargaHorariaPresencial = "00:00"
            };

            // Act
            var resultado = await _sut.GerarArquivoLaudaCompletaAsync(dados);

            // Assert
            using var stream = new MemoryStream(resultado);
            using var word = WordprocessingDocument.Open(stream, false);
            var conteudo = word.MainDocumentPart?.Document.Body?.InnerText;

            conteudo.Should().NotContain("{{CH_PRESENCIAL}}");
        }
    }
}
