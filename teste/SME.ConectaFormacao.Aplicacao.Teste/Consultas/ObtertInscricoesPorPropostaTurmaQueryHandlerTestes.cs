using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObtertInscricoesPorPropostaTurmaQueryHandlerTestes
    {
        private readonly Mock<IRepositorioInscricao> _repositorioInscricaoMock;
        private readonly ObtertInscricoesPorPropostaTurmaQueryHandler _sut;

        public ObtertInscricoesPorPropostaTurmaQueryHandlerTestes()
        {
            var mocker = new AutoMocker();
            _repositorioInscricaoMock = mocker.GetMock<IRepositorioInscricao>();

            _sut = mocker.CreateInstance<ObtertInscricoesPorPropostaTurmaQueryHandler>();
        }

        [Fact]
        public async Task DadoTurmasIds_QuandoChamarHandle_EntaoRetornaListaDeInscricoes()
        {
            // Arrange
            var ids = new long[] { 1, 2 };
            var comando = new ObtertInscricoesPorPropostaTurmaQuery(ids);

            var retornoRepositorio = new List<InscricaoUsuarioInternoDto>
            {
                new InscricaoUsuarioInternoDto { InscricaoId = 1 },
                new InscricaoUsuarioInternoDto { InscricaoId = 2 }
            };

            _repositorioInscricaoMock.Setup(r => r.ObterInscricoesUsuariosInternosPorPropostasTurmasId(ids))
                .ReturnsAsync(retornoRepositorio);

            // Act
            var resultado = await _sut.Handle(comando, CancellationToken.None);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().InscricaoId.Should().Be(1);
        }
    }
}
