using FluentValidation.TestHelper;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricao
{
    public class CasoDeUsoSalvarInscricaoManualTeste
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoSalvarInscricaoManual _casoDeUso;

        public CasoDeUsoSalvarInscricaoManualTeste()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoSalvarInscricaoManual(_mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_Chamar_Mediator_Send_E_Retornar_Retorno_DTO()
        {
            var dto = new InscricaoManualDTO
            {
                PropostaTurmaId = 123,
                Cpf = "12345678900",
                RegistroFuncional = "99999",
                ProfissionalRede = true,
                PodeContinuar = true,
                CargoCodigo = "C1",
                CargoDreCodigo = "D1",
                CargoUeCodigo = "U1",
                FuncaoCodigo = "F1",
                FuncaoDreCodigo = "FD1",
                FuncaoUeCodigo = "FU1",
                TipoVinculo = 10
            };

            var retornoEsperado = RetornoDTO.RetornarSucesso("ok", 123);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarInscricaoManualCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            var resultado = await _casoDeUso.Executar(dto);

            Assert.True(resultado.Sucesso);
            Assert.Equal("ok", resultado.Mensagem);
            Assert.Equal(123, resultado.EntidadeId);

            _mediatorMock.Verify(m => m.Send(It.IsAny<SalvarInscricaoManualCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void RetornoDTO_Deve_Retornar_Sucesso_Com_Id()
        {
            var retorno = RetornoDTO.RetornarSucesso("mensagem", 10);

            Assert.True(retorno.Sucesso);
            Assert.Equal("mensagem", retorno.Mensagem);
            Assert.Equal(10, retorno.EntidadeId);
        }

        [Fact]
        public void RetornoDTO_Deve_Retornar_Sucesso_Sem_Id()
        {
            var retorno = RetornoDTO.RetornarSucesso("mensagem");

            Assert.True(retorno.Sucesso);
            Assert.Equal("mensagem", retorno.Mensagem);
            Assert.Equal(0, retorno.EntidadeId); // default long
        }

        [Fact]
        public void RetornoDTO_Deve_Retornar_Erro()
        {
            var retorno = RetornoDTO.RetornarErro("erro");

            Assert.False(retorno.Sucesso);
            Assert.Equal("erro", retorno.Mensagem);
        }

        [Fact]
        public void Validator_Deve_Retornar_Erro_Quando_PropostaTurmaId_Vazio()
        {
            var validator = new SalvarInscricaoManualCommandValidator();

            var command = new SalvarInscricaoManualCommand(new InscricaoManualDTO
            {
                PropostaTurmaId = 0,
                Cpf = "12345678900"
            }, false);

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.InscricaoManualDTO.PropostaTurmaId)
                  .WithErrorMessage("É necessário informar o id da turma para salvar inscrição manual");
        }

        [Fact]
        public void Validator_Deve_Retornar_Erro_Quando_Cpf_Vazio()
        {
            var validator = new SalvarInscricaoManualCommandValidator();

            var command = new SalvarInscricaoManualCommand(new InscricaoManualDTO
            {
                PropostaTurmaId = 1,
                Cpf = ""
            }, false);

            var result = validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.InscricaoManualDTO.Cpf)
                  .WithErrorMessage("É necessário informar o cpf do cursista para salvar inscrição manual");
        }

        [Fact]
        public void Validator_Deve_Passar_Quando_Dados_Validos()
        {
            var validator = new SalvarInscricaoManualCommandValidator();

            var command = new SalvarInscricaoManualCommand(new InscricaoManualDTO
            {
                PropostaTurmaId = 10,
                Cpf = "12345678900"
            }, false);

            var result = validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
