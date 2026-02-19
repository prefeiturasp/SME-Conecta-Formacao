using AutoMapper;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandlerTeste
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ObterMeusDadosServicoAcessosPorLoginQueryHandler _handler;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandlerTeste()
        {
            var mocker = new AutoMocker();

            _mapperMock = mocker.GetMock<IMapper>();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _handler = mocker.CreateInstance<ObterMeusDadosServicoAcessosPorLoginQueryHandler>();
        }
        [Fact]
        public async Task DeveRetornarDadosUsuarioDTO_QuandoUsuarioInterno()
        {
            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Teste",
                Login = "1234567",
                Email = "usuario@teste.com",
                Tipo = (int)TipoUsuario.Interno
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("1234567")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("1234567")).ReturnsAsync((1, "usuario@edu.sme.prefeitura.sp.gov.br"));

            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns(new DadosUsuarioDTO { Nome = "Usuario Teste", Login = "1234567", Email = "usuario@teste.com", EmailEducacional = "usuario@edu.sme.prefeitura.sp.gov.br" });
            
            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("1234567");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("Usuario Teste", result.Nome);
            Assert.Equal("1234567", result.Login);
            Assert.Equal("usuario@teste.com", result.Email);
            Assert.Equal("usuario@edu.sme.prefeitura.sp.gov.br", result.EmailEducacional);
        }

        [Fact]
        public async Task DeveRetornarDadosUsuarioDTO_QuandoUsuarioExterno()
        {
            var usuario = ObterUsuario();

            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Externo",
                Login = "1234567",
                Email = "usuario@externo.com",
                Tipo = (int)TipoUsuario.Externo
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("1234567")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("1234567")).ReturnsAsync(usuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("1234567")).ReturnsAsync((1, "usuario@edu.sme.prefeitura.sp.gov.br"));

            _mediatorMock.Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync("Unidade Teste");
            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns(new DadosUsuarioDTO { Nome = "Usuario Externo", Login = "1234567", Email = "usuario@externo.com", NomeUnidade = "Unidade Teste" });

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("1234567");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("Usuario Externo", result.Nome);
            Assert.Equal("1234567", result.Login);
            Assert.Equal("usuario@externo.com", result.Email);
            Assert.Equal("Unidade Teste", result.NomeUnidade);
        }

        private static Usuario ObterUsuario()
        {
            var usuario = new Usuario
            {
                Login = "1234567",
                Nome = "Usuario Externo",
                Email = "usuario@externo.com",
                Cpf = "12345678901",
                Tipo = TipoUsuario.Externo,
                Situacao = SituacaoUsuario.Ativo,
                CodigoEolUnidade = "UE123"
            };
            return usuario;
        }
    }
}