using FluentAssertions;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Domino.Teste.Extensoes
{
    public class StringExtensaoTestes
    {
        [Theory]
        [InlineData("123abc456", "123456")]
        [InlineData("A1B2C3", "123")]
        [InlineData("!@#$", "")]
        [InlineData(null, null)]
        [InlineData("", "")]
        public void DadoTexto_QuandoChamarSomenteNumeros_EntaoDeveRetornarSomenteNumeros(string? texto, string? esperado)
        {
            var resultado = texto.SomenteNumeros();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("  Texto  ", "Texto")]
        [InlineData("Texto<br>", "Texto")]
        [InlineData("<p>Texto</p>", "Texto")]
        public void DadoTexto_QuandoChamarRemoverTagsHtml_EntaoDeveRetornarSemTags(string texto, string esperado)
        {
            var resultado = texto.RemoverTagsHtml();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("Você", "Voce")]
        [InlineData("Ação", "Acao")]
        public void DadoTexto_QuandoChamarRemoverCaracteresEspeciais_EntaoDeveRetornarTextoLimpo(string texto, string esperado)
        {
            var resultado = texto.RemoverCaracteresEspeciais();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("Você", "Voce")]
        [InlineData("Ação", "Acao")]
        public void DadoTexto_QuandoChamarRemoverAcentosECaracteresEspeciais_EntaoDeveRetornarTextoSemAcentosEspeciais(string texto, string esperado)
        {
            var resultado = texto.RemoverAcentosECaracteresEspeciais();
            resultado.Should().Be(esperado);
        }

        [Fact]
        public void DadoTexto_QuandoChamarRemoverEspacoEmBranco_EntaoDeveRemoverEspacosExtras()
        {
            var texto = "  Texto com espaços  ";
            var esperado = "Texto com espaços";
            var resultado = texto.RemoverEspacoEmBranco();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("12345678909", true)]  // Valid fake CPF pattern logic
        [InlineData("11111111111", false)] // Same digits
        [InlineData("123", false)]
        [InlineData("00000000000", false)]
        [InlineData("86938210080", true)]  // Valid generated CPF
        public void DadoCpf_QuandoChamarCpfEhValido_EntaoDeveValidarCorretamente(string cpf, bool esperado)
        {
            var resultado = cpf.CpfEhValido();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", true)]
        [InlineData("application/pdf", false)]
        public void DadoContentType_QuandoChamarEhArquivoXlsx_EntaoDeveValidar(string contentType, bool esperado)
        {
            var resultado = contentType.EhArquivoXlsx();
            resultado.Should().Be(esperado);
            contentType.NaoEhArquivoXlsx().Should().Be(!esperado);
        }

        [Theory]
        [InlineData("Você", "Voce")]
        [InlineData("Ação", "Acao")]
        public void DadoTexto_QuandoChamarRemoverAcentuacao_EntaoDeveRetornarSemAcentos(string texto, string esperado)
        {
            var resultado = texto.RemoverAcentuacao();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("A", "B", true)]
        [InlineData("A", "a", false)]
        [InlineData("A", "A", false)]
        public void DadoValores_QuandoChamarSaoDiferentes_EntaoDeveCompararCorretamente(string valor1, string valor2, bool esperado)
        {
            var resultado = valor1.SaoDiferentes(valor2);
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("1", true)]
        [InlineData("2", false)]
        public void DadoValor_QuandoChamarEhColaboradorRede_EntaoDeveValidar(string valor, bool esperado)
        {
            var resultado = valor.EhColaboradorRede();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("01:30", 1)] // 90 min / 60
        [InlineData("02:00", 2)] // 120 min / 60
        [InlineData("xx:yy", 0)]
        [InlineData("", 0)]
        public void DadoHoras_QuandoChamarConverterHoraMinutoParaInteiro_EntaoDeveRetornarInteiro(string horas, int esperado)
        {
            var resultado = horas.ConverterHoraMinutoParaInteiro();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("12345678901", "123.456.789-01")]
        [InlineData("123", "123")]
        public void DadoCpf_QuandoChamarAplicarMascaraCpf_EntaoDeveAplicarMascara(string cpf, string esperado)
        {
            var resultado = cpf.AplicarMascaraCpf();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("1234567", "123.456.7")]
        [InlineData("123", "123")]
        public void DadoRf_QuandoChamarAplicarMascaraRf_EntaoDeveAplicarMascara(string rf, string esperado)
        {
            var resultado = rf.AplicarMascaraRf();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("A", "A", true)]
        [InlineData(" A ", "a", true)]
        [InlineData("A", "B", false)]
        [InlineData(null, null, true)]
        [InlineData("A", null, false)]
        public void DadoTextos_QuandoChamarSaoStringsIguais_EntaoDeveCompararCorretamente(string? str1, string? str2, bool esperado)
        {
            var resultado = str1.SaoStringsIguais(str2);
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("1234567", true)]
        [InlineData("123456", false)]
        [InlineData("123456A", false)]
        public void DadoValor_QuandoChamarEhRegistroFuncional_EntaoDeveValidar(string valor, bool esperado)
        {
            var resultado = valor.EhRegistroFuncional();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("joao da silva", "Joao Da Silva")]
        [InlineData("MARIA", "Maria")]
        [InlineData("", "")]
        public void DadoNome_QuandoChamarFormatarNomePessoa_EntaoDeveRetornarNomeFormatado(string nome, string esperado)
        {
            var resultado = nome.FormatarNomePessoa();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData(0, "***")]
        [InlineData(123, "123")]
        public void DadoValor_QuandoChamarMascararOuExibirValor_EntaoDeveMascararSeZero(long valor, string esperado)
        {
            var resultado = valor.MascararOuExibirValor();
            resultado.Should().Be(esperado);
        }

        [Fact]
        public void DadoHtml_QuandoChamarInserirSequencialNoHtml_EntaoDeveSubstituirMarcador()
        {
            var html = "Texto {{NUM_SEQ}} Texto";
            var resultado = html.InserirSequencialNoHtml(123);
            resultado.Should().Be("Texto 123 Texto");
        }

        [Fact]
        public void DadoHtml_QuandoChamarInserirEmissor_EntaoDeveSubstituirMarcador()
        {
            var html = "Texto {{EMISSOR}} Texto";
            var resultado = html.InserirEmissor("SME");
            resultado.Should().Be("Texto SME Texto");
        }

        [Theory]
        [InlineData("<div>   <p> Teste </p>  </div>", "<div><p> Teste </p></div>")]
        [InlineData("  ", "")]
        [InlineData(null, null)]
        public void DadoHtml_QuandoChamarMinificarHtml_EntaoDeveRemoverEspacosExtras(string? html, string? esperado)
        {
            var resultado = html?.MinificarHtml();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("12345678900", @"000\.000\.000\-00", "123.456.789-00")]
        [InlineData(null, "000", null)]
        [InlineData("abc", "000", "")]
        public void DadoValor_QuandoChamarAplicarMascara_EntaoDeveRetornarValorComMascara(string? valor, string mascara, string? esperado)
        {
            var resultado = valor?.AplicarMascara(mascara);
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("teste@gmail.com", "tes**@gmail.com")]
        [InlineData("joao.silva@empresa.com", "joa*******@empresa.com")]
        public void DadoEmail_QuandoChamarTratarEmail_EntaoDeveMascararParteDoEmail(string email, string esperado)
        {
            var resultado = email.TratarEmail();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("teste@gmail.com", true)]
        [InlineData("email.invalido", false)]
        [InlineData("@gmail.com", false)]
        public void DadoEmail_QuandoChamarEmailEhValido_EntaoDeveValidarCorretamente(string email, bool esperado)
        {
            var resultado = email.EmailEhValido();
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("Teste longo", 5, "Teste")]
        [InlineData("Curto", 10, "Curto")]
        public void DadoTexto_QuandoChamarLimite_EntaoDeveRetornarTextoLimitado(string texto, int limite, string esperado)
        {
            var resultado = texto.Limite(limite);
            resultado.Should().Be(esperado);
        }

        [Theory]
        [InlineData("Texto", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void DadoTexto_QuandoChamarEstaPreenchido_EntaoDeveValidarCorretamente(string? texto, bool esperado)
        {
#pragma warning disable CS0618
            var resultado = texto.EstaPreenchido();
            var resultadoNao = texto.NaoEstaPreenchido();
#pragma warning restore CS0618
            resultado.Should().Be(esperado);
            resultadoNao.Should().Be(!esperado);
        }

        [Fact]
        public void DadoTexto_QuandoChamarParametros_EntaoDeveFormatarCorretamente()
        {
            var texto = "Valor 1: {0}, Valor 2: {1}";
            var resultado = texto.Parametros("A", 2);
            resultado.Should().Be("Valor 1: A, Valor 2: 2");
        }

        [Fact]
        public void DadoTexto_QuandoChamarGerarHashSHA256_EntaoDeveRetornarHashCorreto()
        {
            var texto = "teste123";
            var resultado = texto.GerarHashSHA256();
            resultado.Should().Be("289160db0d9f39f9ae1754c4ec9c16f90b50e32e09c5fb5481ae642b3d3d1a36");
        }
    }
}
