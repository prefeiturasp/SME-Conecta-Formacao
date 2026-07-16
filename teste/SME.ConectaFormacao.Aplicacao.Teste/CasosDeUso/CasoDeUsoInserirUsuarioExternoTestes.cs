using Bogus;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuarioPorCpf;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoInserirUsuarioExternoTestes
    {
        private readonly AutoMocker _mocker;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CasoDeUsoInserirUsuarioExterno _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoInserirUsuarioExternoTestes()
        {
            _mocker = new AutoMocker();
            _mediatorMock = _mocker.GetMock<IMediator>();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoInserirUsuarioExterno>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoDadosInvalidos_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = CriarDtoValido();
            dto.Cpf = "11111111111";
            dto.Email = "email-invalido";
            dto.EmailEducacional = string.Empty;
            dto.Senha = "abc";
            dto.ConfirmarSenha = "xyz";

            // Act
            var acao = () => _casoDeUso.Executar(dto);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.EMAIL_EDU_INVALIDO);
            excecao.Which.Mensagens.Should().Contain(MensagemNegocio.CONFIRMACAO_SENHA_DEVE_SER_IGUAL_A_SENHA);
        }

        [Fact]
        public async Task DadoUsuarioJaExistenteNoConecta_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = CriarDtoValido();
            var usuarioExistente = new Usuario(dto.Cpf, dto.Nome, dto.Email);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            // Act
            var acao = () => _casoDeUso.Executar(dto);

            // Assert
            var excecao = await acao.Should().ThrowAsync<NegocioException>();
            excecao.Which.Message.Should().Be(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CONECTA);

            _mediatorMock.Verify(m => m.Send(It.IsAny<CadastrarUsuarioServicoAcessoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DadoUsuarioExistenteNoCoreSsoESemConfirmacaoEmail_QuandoExecutar_EntaoDeveAtualizarESalvarUsuarioAtivo()
        {
            // Arrange
            var dto = CriarDtoValido();

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorCpfQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UsuarioExisteNoCoreSsoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AtualizarUsuarioServicoAcessoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ParametroSistema { Valor = "false" });

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _casoDeUso.Executar(dto);

            // Assert
            resultado.ValidarEmail.Should().BeFalse();
            resultado.Mensagem.Should().Be(MensagemNegocio.USUARIO_EXTRNO_CADASTRADO_COM_SUCESSO);

            _mediatorMock.Verify(m => m.Send(
                It.Is<AtualizarUsuarioServicoAcessoCommand>(c => c.Login == dto.Cpf && c.Email == dto.Email),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(
                It.Is<SalvarUsuarioCommand>(c => c.Usuario.Situacao == SituacaoUsuario.Ativo && c.Usuario.Login == dto.Cpf),
                It.IsAny<CancellationToken>()), Times.Once);

            _mediatorMock.Verify(m => m.Send(It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private UsuarioExternoDTO CriarDtoValido()
        {
            return new UsuarioExternoDTO
            {
                CodigoUnidade = _faker.Random.Number(1000, 9999).ToString(),
                Email = _faker.Internet.Email().ToLower(),
                EmailEducacional = "usuario@edu.sme.prefeitura.sp.gov.br",
                Nome = _faker.Person.FullName,
                Cpf = "12345678909",
                Senha = "Senha123",
                ConfirmarSenha = "Senha123",
                TipoEmail = TipoEmail.FuncionarioUnidadeParceira,
                Tipo = TipoUsuario.Externo
            };
        }
    }
}
