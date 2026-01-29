using FluentAssertions;
using SME.ConectaFormacao.Infra.Dados.Templates;
using System.Reflection;

namespace SME.ConectaFormacao.Infra.Dados.Teste.Templates
{
    public class TemplateServiceTests
    {
        private readonly TemplateService _sut;

        public TemplateServiceTests()
        {
            var assemblyDeTeste = Assembly.GetExecutingAssembly();
            _sut = new TemplateService(assemblyDeTeste);
        }

        [Fact]
        public void ObterTemplateCertificado_DeveRetornarConteudo_QuandoArquivoExiste()
        {
            // Arrange
            var nomeArquivo = "teste_template.html";

            // Act
            var resultado = _sut.ObterTemplateCertificado(nomeArquivo);

            // Assert
            resultado.Should().Be("<h1>Ola Mundo</h1>");
        }

        [Fact]
        public void ObterTemplateCertificado_DeveLancarException_QuandoArquivoNaoExiste()
        {
            // Arrange
            var nomeArquivo = "arquivo_fantasma.html";

            // Act
            Action act = () => _sut.ObterTemplateCertificado(nomeArquivo);

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("Template não encontrado")
               .And.FileName.Should().Be(nomeArquivo);
        }

        [Fact]
        public void ObterTemplateCertificado_DeveUsarCache_NaSegundaChamada()
        {
            // Arrange
            var nomeArquivo = "teste_template.html";

            // Act
            var resultado1 = _sut.ObterTemplateCertificado(nomeArquivo);
            var resultado2 = _sut.ObterTemplateCertificado(nomeArquivo);

            // Assert
            resultado1.Should().Be(resultado2);
        }

        [Fact]
        public void ObterImagemBase64_DeveRetornarBase64_QuandoArquivoExiste()
        {
            // Arrange
            var nomeArquivo = "teste_imagem.txt";
            // "dadosbinarios" em Base64 é "ZGFkb3NiaW5hcmlvcw=="
            var esperadoBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("dadosbinarios"));

            // Act
            var resultado = _sut.ObterImagemBase64(nomeArquivo);

            // Assert
            resultado.Should().Be(esperadoBase64);
        }

        [Fact]
        public void ObterImagemBase64_DeveLancarException_QuandoArquivoNaoExiste()
        {
            // Arrange
            var nomeArquivo = "imagem_fantasma.png";

            // Act
            Action act = () => _sut.ObterImagemBase64(nomeArquivo);

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("Imagem não encontrada")
               .And.FileName.Should().Be(nomeArquivo);
        }

        [Fact]
        public void Construtor_DeveUsarAssemblyPadrao_QuandoNenhumForInformado()
        {
            var sutProducao = new TemplateService(); // Sem parametros

            Action act = () => sutProducao.ObterTemplateCertificado("teste_template.html");

            act.Should().Throw<FileNotFoundException>();
        }
    }
}
