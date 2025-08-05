using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Inscricao
{
    public class CasoDeUsoTransferirInscricaoTeste
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ICasoDeUsoTransferirInscricao _casoDeUso;

        public CasoDeUsoTransferirInscricaoTeste()
        {
            _mediatorMock = new Mock<IMediator>();
            _casoDeUso = new CasoDeUsoTransferirInscricao(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Deve executar transferência de inscrição com sucesso")]
        public async Task Deve_Executar_Transferencia_De_Inscricao_Com_Sucesso()
        {
            var idInscricao = 123;

            var dto = new InscricaoTransferenciaDTO
            {
                IdFormacaoOrigem = 1,
                IdTurmaOrigem = 2,
                IdFormacaoDestino = 3,
                IdTurmaDestino = 4,
                Cursistas = new List<int> { 999 }  
            };

            var retornoEsperado = RetornoDTO.RetornarSucesso("Transferência realizada com sucesso", idInscricao);

            _mediatorMock.Setup(m => m.Send(It.IsAny<TransferirInscricaoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(retornoEsperado);

            var resultado = await _casoDeUso.Executar(idInscricao, dto);

            Assert.NotNull(resultado);
            Assert.True(resultado.Sucesso);
            Assert.Equal("Transferência realizada com sucesso", resultado.Mensagem);

            _mediatorMock.Verify(m => m.Send(It.Is<TransferirInscricaoCommand>(cmd => cmd.IdInscricao == idInscricao && cmd.InscricaoTransferenciaDTO == dto),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

