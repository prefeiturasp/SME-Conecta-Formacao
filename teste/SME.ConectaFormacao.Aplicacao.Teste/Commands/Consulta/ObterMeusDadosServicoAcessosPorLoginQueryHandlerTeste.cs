using AutoMapper;
using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste
{

    public class ObterMeusDadosServicoAcessosPorLoginQueryHandlerTeste
    {
        [Fact]
        public async Task DeveRetornarDadosUsuarioDTO_QuandoUsuarioInterno()
        {
            var mapperMock = new Mock<IMapper>();
            var servicoAcessosMock = new Mock<IServicoAcessos>();
            var repositorioUsuarioMock = new Mock<IRepositorioUsuario>();
            var mediatorMock = new Mock<IMediator>();

            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Teste",
                Login = "1234567",
                Email = "usuario@teste.com",
                Tipo = (int)TipoUsuario.Interno
            };

            servicoAcessosMock.Setup(s => s.ObterMeusDados("1234567")).ReturnsAsync(acessoDadosUsuario);
            repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("1234567")).ReturnsAsync((1, "usuario@edu.sme.prefeitura.sp.gov.br"));

            mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns(new DadosUsuarioDTO { Nome = "Usuario Teste", Login = "1234567", Email = "usuario@teste.com", EmailEducacional = "usuario@edu.sme.prefeitura.sp.gov.br" });

            var handler = new ObterMeusDadosServicoAcessosPorLoginQueryHandler(mapperMock.Object, servicoAcessosMock.Object, repositorioUsuarioMock.Object, mediatorMock.Object);
            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("1234567");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal("Usuario Teste", result.Nome);
            Assert.Equal("1234567", result.Login);
            Assert.Equal("usuario@teste.com", result.Email);
            Assert.Equal("usuario@edu.sme.prefeitura.sp.gov.br", result.EmailEducacional);
        }

        [Fact]
        public async Task DeveRetornarDadosUsuarioDTO_QuandoUsuarioExterno()
        {
            var mapperMock = new Mock<IMapper>();
            var servicoAcessosMock = new Mock<IServicoAcessos>();
            var repositorioUsuarioMock = new Mock<IRepositorioUsuario>();
            var mediatorMock = new Mock<IMediator>();

            var usuario = ObterUsuario();

            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Externo",
                Login = "1234567",
                Email = "usuario@externo.com",
                Tipo = (int)TipoUsuario.Externo
            };

            servicoAcessosMock.Setup(s => s.ObterMeusDados("1234567")).ReturnsAsync(acessoDadosUsuario);
            repositorioUsuarioMock.Setup(r => r.ObterPorLogin("1234567")).ReturnsAsync(usuario);
            repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("1234567")).ReturnsAsync((1, "usuario@edu.sme.prefeitura.sp.gov.br"));

            mediatorMock.Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync("Unidade Teste");
            mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns(new DadosUsuarioDTO { Nome = "Usuario Externo", Login = "1234567", Email = "usuario@externo.com", NomeUnidade = "Unidade Teste" });

            var handler = new ObterMeusDadosServicoAcessosPorLoginQueryHandler(mapperMock.Object, servicoAcessosMock.Object, repositorioUsuarioMock.Object, mediatorMock.Object);
            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("1234567");

            var result = await handler.Handle(query, CancellationToken.None);

            Assert.Equal("Usuario Externo", result.Nome);
            Assert.Equal("1234567", result.Login);
            Assert.Equal("usuario@externo.com", result.Email);
            Assert.Equal("Unidade Teste", result.NomeUnidade);
        }

        private static Usuario ObterUsuario()
        {
            var usuario = new SME.ConectaFormacao.Dominio.Entidades.Usuario();
            usuario.Login = "1234567";
            usuario.Nome = "Usuario Externo";
            usuario.Email = "usuario@externo.com";
            usuario.Cpf = "12345678901";
            usuario.Tipo = TipoUsuario.Externo;
            usuario.Situacao = SituacaoUsuario.Ativo;
            usuario.CodigoEolUnidade = "UE123";
            return usuario;
        }
    }
}