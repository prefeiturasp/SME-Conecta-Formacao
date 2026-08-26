using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Dre;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterDreListaUsuarioLogadoTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoObterDreListaUsuarioLogado _sut;

        public CasoDeUsoObterDreListaUsuarioLogadoTestes()
        {
            _mocker = new AutoMocker();
            _sut = _mocker.CreateInstance<CasoDeUsoObterDreListaUsuarioLogado>();
        }

        [Fact]
        public async Task DadoUsuarioSemPerfil_QuandoExecutar_EntaoLancaExcecao()
        {
            // Arrange

            // Act
            Func<Task> acao = _sut.ExecutarAsync;

            // Assert
            await acao.Should().ThrowAsync<NegocioException>().WithMessage("Usuário não possui perfil de acesso.");
        }

        [Fact]
        public async Task DadoUsuarioComPerfil_QuandoExecutar_EntaoRetornaDres()
        {
            // Arrange
            Guid perfilId = Guid.NewGuid();
            _mocker.GetMock<IContextoAplicacao>()
                .Setup(m => m.IdPerfilUsuario)
                .Returns(perfilId);

            var dres = new List<Dre>
            {
                new() { Id = 1, Nome = "DRE 1" },
                new() { Id = 2, Nome = "DRE 2" }
            };

            _mocker.GetMock<IRepositorioAreaPromotora>()
                .Setup(m => m.ObterDresPorGrupoIdAsync(perfilId))
                .ReturnsAsync(dres);

            // Act
            var resultado = await _sut.ExecutarAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);

            var primeiraDre = resultado.First();
            primeiraDre.Id.Should().Be(1);
            primeiraDre.Descricao.Should().Be("DRE 1");

            _mocker.GetMock<IRepositorioAreaPromotora>().Verify(m => m.ObterDresPorGrupoIdAsync(perfilId), Times.Once);
        }
    }
}
