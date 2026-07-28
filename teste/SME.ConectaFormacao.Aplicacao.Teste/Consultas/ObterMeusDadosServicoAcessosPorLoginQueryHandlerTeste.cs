using AutoMapper;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomesFuncionarioPorRf;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Acessos;
using SME.ConectaFormacao.Infra.Servicos.Acessos.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterMeusDadosServicoAcessosPorLoginQueryHandlerTeste
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IServicoAcessos> _servicoAcessosMock;
        private readonly Mock<IRepositorioUsuario> _repositorioUsuarioMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRepositorioUsuarioAcessibilidade> _repositorioUsuarioAcessibilidadeMock;
        private readonly ObterMeusDadosServicoAcessosPorLoginQueryHandler _handler;

        public ObterMeusDadosServicoAcessosPorLoginQueryHandlerTeste()
        {
            var mocker = new AutoMocker();

            _mapperMock = mocker.GetMock<IMapper>();
            _servicoAcessosMock = mocker.GetMock<IServicoAcessos>();
            _repositorioUsuarioMock = mocker.GetMock<IRepositorioUsuario>();
            _mediatorMock = mocker.GetMock<IMediator>();
            _repositorioUsuarioAcessibilidadeMock = mocker.GetMock<IRepositorioUsuarioAcessibilidade>();
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
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync((UsuarioAcessibilidade?)null);
            
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
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync((UsuarioAcessibilidade?)null);

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("1234567");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("Usuario Externo", result.Nome);
            Assert.Equal("1234567", result.Login);
            Assert.Equal("usuario@externo.com", result.Email);
            Assert.Equal("Unidade Teste", result.NomeUnidade);
        }

        [Fact]
        public async Task DeveCopiarEmailEducacional_QuandoEmailDoAcessoForEducacionalEEmailEducacionalVazio()
        {
            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Aluno Edu",
                Login = "7654321",
                Email = "aluno@edu.sme.prefeitura.sp.gov.br",
                Tipo = (int)TipoUsuario.Interno,
                EmailEducacional = null
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("7654321")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("7654321")).ReturnsAsync((0, string.Empty));
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync((UsuarioAcessibilidade?)null);

            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns((AcessosDadosUsuario a) => new DadosUsuarioDTO
            {
                Nome = a.Nome,
                Login = a.Login,
                Email = a.Email,
                EmailEducacional = a.EmailEducacional!
            });

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("7654321");

            var result = await _handler.Handle(query, CancellationToken.None);

            // Como o email do acesso é do dominio educacional e EmailEducacional estava vazio, deve ser copiado
            Assert.Equal("aluno@edu.sme.prefeitura.sp.gov.br", result.EmailEducacional);
        }

        [Fact]
        public async Task DeveGerarEmailEducacional_QuandoUsuarioExisteEEmailEducacionalVazio()
        {
            var usuario = ObterUsuario();
            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Gerador",
                Login = "9999999",
                Email = "usuario@naoedu.com",
                Tipo = (int)TipoUsuario.Interno,
                EmailEducacional = null
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("9999999")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("9999999")).ReturnsAsync(usuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("9999999")).ReturnsAsync((0, string.Empty));
            _mediatorMock.Setup(m => m.Send(It.IsAny<GerarEmailEducacionalCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("gerado@edu.sme.prefeitura.sp.gov.br");
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync((UsuarioAcessibilidade?)null);

            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns(new DadosUsuarioDTO { Nome = "Usuario Gerador", Login = "9999999", Email = "usuario@naoedu.com", EmailEducacional = "gerado@edu.sme.prefeitura.sp.gov.br" });

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("9999999");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("gerado@edu.sme.prefeitura.sp.gov.br", result.EmailEducacional);
        }

        [Fact]
        public async Task DeveObterNomePeloLoginEMapearAcessibilidadeQuandoNomeNulo()
        {
            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = null!,
                Login = "8888888",
                Email = "usuario@naoedu.com",
                NomeSocial = "NomeSocial Via EOL",
                Tipo = (int)TipoUsuario.Interno
            };

            var acessibilidade = new UsuarioAcessibilidade
            {
                UsuarioId = 1,
                PossuiDeficiencia = true,
                DescricaoDeficiencia = "Visual",
                NecessitaAdaptacao = false,
                DescricaoAdaptacao = null
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("8888888")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("8888888")).ReturnsAsync((0, string.Empty));
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync(acessibilidade);

            _mediatorMock.Setup(m => m.Send(It.IsAny<ObterNomesFuncionarioPorRfQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FuncionarioNomesDto { Nome = "NomeSocial Via EOL", NomeSocial = "NomeSocial Via EOL" });

            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns((AcessosDadosUsuario a) => new DadosUsuarioDTO
            {
                Nome = a.Nome ?? "NomeSocial Via EOL",
                Login = a.Login,
                Email = a.Email,
                EmailEducacional = a.EmailEducacional!
            });

            _mapperMock.Setup(m => m.Map<UsuarioAcessibilidadeDto>(It.IsAny<UsuarioAcessibilidade>()))
                .Returns(new UsuarioAcessibilidadeDto { UsuarioId = 1, PossuiDeficiencia = true, DescricaoDeficiencia = "Visual", NecessitaAdaptacao = false, DescricaoAdaptacao = null, Salvar = false });

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("8888888");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("NomeSocial Via EOL", result.Nome);
            Assert.NotNull(result.UsuarioAcessibilidade);
            Assert.True(result.UsuarioAcessibilidade.PossuiDeficiencia);
            Assert.Equal("Visual", result.UsuarioAcessibilidade.DescricaoDeficiencia);
        }

        [Fact]
        public async Task DeveRetornarTelefoneDoUsuario_QuandoUsuarioPossuiTelefone()
        {
            var usuario = ObterUsuario();
            usuario.Telefone = "11999999999";

            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Telefone",
                Login = "7777777",
                Email = "usuario@naoedu.com",
                Tipo = (int)TipoUsuario.Interno,
                Telefone = null!
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("7777777")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("7777777")).ReturnsAsync(usuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("7777777")).ReturnsAsync((0, string.Empty));
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync((UsuarioAcessibilidade?)null);

            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns((AcessosDadosUsuario a) => new DadosUsuarioDTO
            {
                Nome = a.Nome,
                Login = a.Login,
                Email = a.Email,
                Telefone = a.Telefone
            });

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("7777777");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("11999999999", result.Telefone);
        }

        [Fact]
        public async Task DeveSobrescreverTelefoneDoAcessoPeloTelefoneDoUsuario_QuandoAmbosExistirem()
        {
            var usuario = ObterUsuario();
            usuario.Telefone = "22222222";

            var acessoDadosUsuario = new AcessosDadosUsuario
            {
                Nome = "Usuario Telefone Override",
                Login = "6666666",
                Email = "usuario@naoedu.com",
                Tipo = (int)TipoUsuario.Interno,
                Telefone = "11111111"
            };

            _servicoAcessosMock.Setup(s => s.ObterMeusDados("6666666")).ReturnsAsync(acessoDadosUsuario);
            _repositorioUsuarioMock.Setup(r => r.ObterPorLogin("6666666")).ReturnsAsync(usuario);
            _repositorioUsuarioMock.Setup(r => r.ObterEmailEducacionalPorLogin("6666666")).ReturnsAsync((0, string.Empty));
            _repositorioUsuarioAcessibilidadeMock.Setup(r => r.ObterAcessibilidadeAtualDoUsuarioAsync()).ReturnsAsync((UsuarioAcessibilidade?)null);

            _mapperMock.Setup(m => m.Map<DadosUsuarioDTO>(It.IsAny<AcessosDadosUsuario>())).Returns((AcessosDadosUsuario a) => new DadosUsuarioDTO
            {
                Nome = a.Nome,
                Login = a.Login,
                Email = a.Email,
                Telefone = a.Telefone
            });

            var query = new ObterMeusDadosServicoAcessosPorLoginQuery("6666666");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.Equal("22222222", result.Telefone);
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