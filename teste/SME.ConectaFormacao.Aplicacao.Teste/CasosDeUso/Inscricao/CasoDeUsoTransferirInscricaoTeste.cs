using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
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
            var dto = new InscricaoTransferenciaDTO
            {
                IdFormacaoOrigem = 1,
                IdTurmaOrigem = 2,
                IdFormacaoDestino = 3,
                IdTurmaDestino = 4,
                Cursistas = new List<InscricaoTransferenciaDTOCursista>
                {
                    new InscricaoTransferenciaDTOCursista
                    {
                        Rf = "999",
                        IdInscricao = 123
                    }
                }
            };

            var retornoEsperado = new RetornoInscricaoDTO(
                "Transferência realizada com sucesso",
                new List<CursistaDTO>
                {
                new CursistaDTO
                    {
                        NomeCursista = "João da Silva",
                        Rf = 999,
                        Mensagem = "Transferido com sucesso"
                    }
                }
            );

            _mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<TransferirInscricaoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retornoEsperado);

            var resultado = await _casoDeUso.Executar(dto);

            Assert.NotNull(resultado);
            Assert.Equal("Transferência realizada com sucesso", resultado.Mensagem);
            Assert.Single(resultado.Cursistas);
            Assert.Equal(999, resultado.Cursistas.First().Rf);
            Assert.Equal("João da Silva", resultado.Cursistas.First().NomeCursista);
            Assert.Equal("Transferido com sucesso", resultado.Cursistas.First().Mensagem);

            _mediatorMock.Verify(m => m.Send(
                It.Is<TransferirInscricaoCommand>(cmd =>
                    cmd.InscricaoTransferenciaDTO == dto),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

