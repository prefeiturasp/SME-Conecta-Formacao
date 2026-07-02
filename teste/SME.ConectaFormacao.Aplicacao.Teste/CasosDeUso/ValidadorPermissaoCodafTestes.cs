using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Codaf;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class ValidadorPermissaoCodafTestes
    {
        private readonly Mock<IRepositorioCodafListaPresenca> repositorioMock;
        private readonly Mock<IContextoAplicacao> contextoMock;

        public ValidadorPermissaoCodafTestes()
        {
            repositorioMock = new Mock<IRepositorioCodafListaPresenca>();
            contextoMock = new Mock<IContextoAplicacao>();
        }

        private ValidadorPermissaoCodaf CriarCasoDeUso()
        {
            return new ValidadorPermissaoCodaf(
                repositorioMock.Object,
                contextoMock.Object);
        }

        [Fact]
        public void Deve_lancar_excecao_quando_repositorio_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ValidadorPermissaoCodaf(null!, contextoMock.Object));
        }

        [Fact]
        public void Deve_lancar_excecao_quando_contexto_for_nulo()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ValidadorPermissaoCodaf(repositorioMock.Object, null!));
        }

        [Fact]
        public async Task Deve_retornar_false_quando_codaf_nao_existir()
        {
            repositorioMock.Setup(x => x.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync((CodafListaPresenca)null!);

            var usuario = new Usuario
            {
                Login = "usuario"
            };

            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.ValidarSeUsuarioEhCriador(usuario, 1);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_usuario_for_criador()
        {
            repositorioMock.Setup(x => x.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca
                {
                    CriadoLogin = "usuario"
                });

            var usuario = new Usuario
            {
                Login = "usuario"
            };

            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.ValidarSeUsuarioEhCriador(usuario, 1);

            Assert.True(resultado);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_usuario_nao_for_criador()
        {
            repositorioMock.Setup(x => x.ObterPorIdDetalhadoAsync(It.IsAny<long>()))
                .ReturnsAsync(new CodafListaPresenca
                {
                    CriadoLogin = "outro.usuario"
                });

            var usuario = new Usuario
            {
                Login = "usuario"
            };

            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.ValidarSeUsuarioEhCriador(usuario, 1);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_guid_for_empty()
        {
            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.UsuarioPossuiPerfilAdminOuEMFORPEF(Guid.Empty);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_perfil_for_admin()
        {
            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.UsuarioPossuiPerfilAdminOuEMFORPEF(Perfis.ADMIN_DF);

            Assert.True(resultado);
        }

        [Fact]
        public async Task Deve_retornar_true_quando_perfil_for_emforpef()
        {
            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.UsuarioPossuiPerfilAdminOuEMFORPEF(Perfis.EMFORPEF);

            Assert.True(resultado);
        }

        [Fact]
        public async Task Deve_retornar_false_quando_perfil_nao_for_admin_nem_emforpef()
        {
            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.UsuarioPossuiPerfilAdminOuEMFORPEF(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task Deve_retornar_perfil_do_contexto()
        {
            var perfil = Guid.NewGuid();

            contextoMock.SetupGet(x => x.IdPerfilUsuario)
                .Returns(perfil);

            var casoDeUso = CriarCasoDeUso();

            var resultado = await casoDeUso.BuscarPerfilUsuario();

            Assert.Equal(perfil, resultado);
        }

        [Fact]
        public async Task Deve_lancar_negocio_exception_quando_perfil_for_nulo()
        {
            contextoMock.SetupGet(x => x.IdPerfilUsuario)
                .Returns((Guid?)null);

            var casoDeUso = CriarCasoDeUso();

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => casoDeUso.BuscarPerfilUsuario());

            Assert.Equal(
                "Não foi possível identificar os perfis do usuário logado. Por favor, faça login novamente.",
                excecao.Message);
        }
    }
}
