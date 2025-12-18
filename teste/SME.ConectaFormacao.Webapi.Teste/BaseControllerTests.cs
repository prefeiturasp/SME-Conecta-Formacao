using Microsoft.AspNetCore.Mvc;
using SME.ConectaFormacao.Dominio.Comum;

namespace SME.ConectaFormacao.Webapi.Teste
{
    public class BaseControllerTests
    {
        private readonly BaseControllerWrapper _controller = new();

        [Fact]
        public void DadoResultadoComSucessoEDados_QuandoProcessarResultado_EntaoDeveRetornarOkObjectResult()
        {
            // Arrange
            var dados = new { Id = 1, Nome = "Teste" };
            var resultado = Resultado<object>.DeSucesso(dados);

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(dados, okResult.Value);
        }

        [Fact]
        public void DadoResultadoComSucessoMasDadosNulos_QuandoProcessarResultado_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var resultado = Resultado<object?>.DeSucesso(null);

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(response);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public void DadoResultadoVoidComSucesso_QuandoProcessarResultado_EntaoDeveRetornarNoContent()
        {
            // Arrange
            var resultado = Resultado.DeSucesso();

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(response);
            Assert.Equal(204, noContentResult.StatusCode);
        }

        [Fact]
        public void DadoResultadoComSucesso_QuandoProcessarCriado_EntaoDeveRetornarCreatedResult()
        {
            // Arrange
            var uri = "/api/teste/1";
            var dados = new { Id = 1 };
            var resultado = Resultado<object>.DeSucesso(dados);

            // Act
            var response = _controller.ProcessarCriado(uri, resultado);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(uri, createdResult.Location);
            Assert.Equal(dados, createdResult.Value);
        }

        [Fact]
        public void DadoFalhaTipoNaoEncontrado_QuandoProcessarResultado_EntaoDeveRetornarNotFound()
        {
            // Arrange
            var erro = Erro.NaoEncontrado("Registro inexistente");
            Resultado<object> resultado = erro; // Conversão implícita

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(response);
            Assert.Equal(404, notFoundResult.StatusCode);
            VerificarEstruturaDeErro(notFoundResult.Value, "Registro inexistente");
        }

        [Fact]
        public void DadoFalhaTipoValidacao_QuandoProcessarResultado_EntaoDeveRetornarBadRequest()
        {
            // Arrange
            var erro = Erro.Validacao("Campo obrigatório");
            Resultado<object> resultado = erro;

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            Assert.Equal(400, badRequestResult.StatusCode);
            VerificarEstruturaDeErro(badRequestResult.Value, "Campo obrigatório");
        }

        [Fact]
        public void DadoFalhaTipoRegraDeNegocio_QuandoProcessarResultado_EntaoDeveRetornarUnprocessableEntity()
        {
            // Arrange
            var erro = Erro.Negocio("Saldo insuficiente");
            Resultado<object> resultado = erro;

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(422, unprocessableResult.StatusCode);
            VerificarEstruturaDeErro(unprocessableResult.Value, "Saldo insuficiente");
        }

        [Fact]
        public void DadoFalhaTipoNaoAutorizado_QuandoProcessarResultado_EntaoDeveRetornarForbid()
        {
            // Arrange
            // Nota: O código original usa o 'Forbid(string)', que define o AuthenticationScheme.
            var mensagemErro = "Usuário sem permissão";
            var resultado = Resultado<object>.DeFalha(TipoFalha.NaoAutorizado, mensagemErro);

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var forbidResult = Assert.IsType<ForbidResult>(response);
            // O código original passa a mensagem como Scheme
            Assert.Contains(mensagemErro, forbidResult.AuthenticationSchemes);
        }

        [Fact]
        public void DadoFalhaTipoErroInterno_QuandoProcessarResultado_EntaoDeveRetornarInternalServerError()
        {
            // Arrange
            var resultado = Resultado<object>.DeFalha(TipoFalha.ErroInterno, "Erro fatal");

            // Act
            var response = _controller.ProcessarResultado(resultado);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(response);
            Assert.Equal(500, objectResult.StatusCode);
            VerificarEstruturaDeErro(objectResult.Value, "Erro fatal");
        }

        // --- Método Auxiliar para validar o corpo do erro anônimo ---
        private static void VerificarEstruturaDeErro(object? valorRetorno, string mensagemEsperada)
        {
            Assert.NotNull(valorRetorno);

            // Reflection é necessário pois o retorno é um objeto anônimo (new { sucesso = false ... })
            var propSucesso = valorRetorno.GetType().GetProperty("sucesso");
            var propErros = valorRetorno.GetType().GetProperty("erros");

            Assert.NotNull(propSucesso);
            Assert.NotNull(propErros);

            var sucesso = (bool?)propSucesso.GetValue(valorRetorno);
            var erros = (List<string>?)propErros.GetValue(valorRetorno);

            Assert.False(sucesso);
            Assert.Contains(mensagemEsperada, erros);
        }
    }
}
