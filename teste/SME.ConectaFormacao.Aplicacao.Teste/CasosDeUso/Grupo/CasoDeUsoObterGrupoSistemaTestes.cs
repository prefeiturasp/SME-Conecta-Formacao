using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Grupo;
using SME.ConectaFormacao.Aplicacao.Dtos.Grupo;
using Xunit;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso.Grupo
{
    public class CasoDeUsoObterGrupoSistemaTestes
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoObterGrupoSistema _casoDeUso;

        public CasoDeUsoObterGrupoSistemaTestes()
        {
            var mocker = new AutoMocker();
            _mediatorMock = mocker.GetMock<IMediator>();

            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterGrupoSistema>();
        }

        [Fact]
        public async Task DadoExecucao_QuandoChamarExecutar_EntaoDeveRetornarGruposDoSistema()
        {
            // Arrange
            var gruposRetorno = new List<GrupoDTO>
            {
                new GrupoDTO { Id = Guid.NewGuid(), Nome = "Grupo 1", VisaoId = 1 },
                new GrupoDTO { Id = Guid.NewGuid(), Nome = "Grupo 2", VisaoId = 2 }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterGruposServicoAcessosQuery>(), default))
                .ReturnsAsync(gruposRetorno);

            // Act
            var resultado = await _casoDeUso.Executar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEquivalentTo(gruposRetorno);
            
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterGruposServicoAcessosQuery>(), default), Times.Once);
        }
    }
}
