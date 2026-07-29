using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuarios;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuarioPorCpf;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoInserirUsuarioExternoTestes
    {
        private const string CpfComMascara = "529.982.247-25";
        private const string CpfSemMascara = "52998224725";
        private const string EmailEducacionalComEspacos =
            "  MARIA.SILVA@EDU.SME.PREFEITURA.SP.GOV.BR  ";
        private const string EmailEducacionalNormalizado =
            "maria.silva@edu.sme.prefeitura.sp.gov.br";

        private readonly Mock<IMediator> mediatorMock;
        private readonly CasoDeUsoInserirUsuarioExterno sut;

        public CasoDeUsoInserirUsuarioExternoTestes()
        {
            mediatorMock = new Mock<IMediator>();
            sut = new CasoDeUsoInserirUsuarioExterno(mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_acumular_todos_os_erros_de_preenchimento_aplicaveis()
        {
            var dto = CriarDtoValido();
            dto.Cpf = "111.111.111-11";
            dto.Email = "email-invalido";
            dto.EmailEducacional = "USUARIO@OUTRO-DOMINIO.COM";
            dto.Senha = "abc def";
            dto.ConfirmarSenha = "outra-senha";

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal("11111111111", dto.Cpf);
            Assert.Equal("11111111111", dto.Login);
            Assert.Equal("usuario@outro-dominio.com", dto.EmailEducacional);
            Assert.Contains(
                MensagemNegocio.CPF_COM_DIGITO_VERIFICADOR_INVALIDO.Parametros(dto.Cpf),
                excecao.Message);
            Assert.Contains(
                MensagemNegocio.EMAIL_INVALIDO.Parametros(dto.Email),
                excecao.Message);
            Assert.Contains(MensagemNegocio.EMAIL_EDU_INVALIDO_NAO_VALIDO, excecao.Message);
            Assert.Contains(MensagemNegocio.A_SENHA_NAO_PODE_CONTER_ESPACOS_EM_BRANCO, excecao.Message);
            Assert.Contains(MensagemNegocio.CONFIRMACAO_SENHA_DEVE_SER_IGUAL_A_SENHA, excecao.Message);
            Assert.Contains(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÍNIMO_8_CARACTERES, excecao.Message);
            Assert.Contains(MensagemNegocio.A_SENHA_DEVE_CONTER_SOMENTE, excecao.Message);

            mediatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Executar_Quando_email_educacional_nao_estiver_preenchido_Deve_lancar_excecao()
        {
            var dto = CriarDtoValido();
            dto.EmailEducacional = "   ";

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(string.Empty, dto.EmailEducacional);
            Assert.Contains(MensagemNegocio.EMAIL_EDU_INVALIDO, excecao.Message);
            Assert.DoesNotContain(MensagemNegocio.EMAIL_EDU_INVALIDO_NAO_VALIDO, excecao.Message);
            mediatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Executar_Quando_senha_possuir_mais_de_doze_caracteres_Deve_lancar_excecao()
        {
            var dto = CriarDtoValido();
            dto.Senha = "SenhaMuitoLonga123";
            dto.ConfirmarSenha = dto.Senha;

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Contains(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÁXIMO_12_CARACTERES, excecao.Message);
            Assert.Contains(MensagemNegocio.A_SENHA_DEVE_CONTER_SOMENTE, excecao.Message);
            mediatorMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Executar_Quando_usuario_existir_por_login_Deve_interromper_sem_consultar_cpf()
        {
            var dto = CriarDtoValido();
            var usuarioExistente = new Usuario
            {
                Id = 10,
                Login = CpfSemMascara,
                Cpf = CpfSemMascara,
                Nome = "Usuário existente",
                Email = "existente@teste.com.br"
            };

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == CpfSemMascara),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CONECTA, excecao.Message);
            Assert.Equal(CpfSemMascara, dto.Login);
            Assert.Equal(CpfSemMascara, dto.Cpf);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterUsuarioPorCpfQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<UsuarioExisteNoCoreSsoQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Quando_usuario_existir_por_cpf_Deve_interromper_antes_do_CoreSSO()
        {
            var dto = CriarDtoValido();
            var usuarioExistente = new Usuario
            {
                Id = 20,
                Login = "login-diferente",
                Cpf = CpfSemMascara,
                Nome = "Usuário existente",
                Email = "existente@teste.com.br"
            };

            ConfigurarUsuarioPorLogin(null);
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorCpfQuery>(q => q.Cpf == CpfSemMascara),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioExistente);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(MensagemNegocio.VOCE_JA_POSSUI_LOGIN_CONECTA, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<UsuarioExisteNoCoreSsoQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Quando_usuario_nao_existir_no_CoreSSO_e_confirmacao_for_desabilitada_Deve_cadastrar_usuario_ativo()
        {
            var dto = CriarDtoValido();
            Usuario? usuarioPersistido = null;

            ConfigurarUsuarioNaoExistenteNoConecta();
            ConfigurarExistenciaNoCoreSso(false);
            ConfigurarCadastroNoCoreSso(true);
            ConfigurarParametroConfirmacaoEmail(new ParametroSistema
            {
                Valor = "false"
            });
            ConfigurarPersistencia(usuario => usuarioPersistido = usuario);

            var retorno = await sut.Executar(dto);

            Assert.False(retorno.ValidarEmail);
            Assert.Equal(MensagemNegocio.USUARIO_EXTRNO_CADASTRADO_COM_SUCESSO, retorno.Mensagem);
            Assert.Equal(CpfSemMascara, dto.Login);
            Assert.Equal(CpfSemMascara, dto.Cpf);
            Assert.Equal(EmailEducacionalNormalizado, dto.EmailEducacional);

            Assert.NotNull(usuarioPersistido);
            Assert.Equal(CpfSemMascara, usuarioPersistido!.Login);
            Assert.Equal(dto.Nome, usuarioPersistido.Nome);
            Assert.Equal(dto.NomeSocial, usuarioPersistido.NomeSocial);
            Assert.Equal(dto.Email, usuarioPersistido.Email);
            Assert.Equal(CpfSemMascara, usuarioPersistido.Cpf);
            Assert.Equal(TipoUsuario.Externo, usuarioPersistido.Tipo);
            Assert.Equal(SituacaoUsuario.Ativo, usuarioPersistido.Situacao);
            Assert.Equal(dto.CodigoUnidade, usuarioPersistido.CodigoEolUnidade);
            Assert.Equal(EmailEducacionalNormalizado, usuarioPersistido.EmailEducacional);
            Assert.Equal((TipoEmail?)dto.TipoEmail, usuarioPersistido.TipoEmail);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<CadastrarUsuarioServicoAcessoCommand>(c =>
                        c.Login == CpfSemMascara &&
                        c.Nome == dto.Nome &&
                        c.Email == dto.Email &&
                        c.Senha == dto.Senha &&
                        c.NomeSocial == dto.NomeSocial),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Quando_usuario_existir_no_CoreSSO_e_confirmacao_for_habilitada_Deve_atualizar_e_enviar_email()
        {
            var dto = CriarDtoValido();
            dto.Tipo = TipoUsuario.RedeParceria;
            Usuario? usuarioPersistido = null;

            ConfigurarUsuarioNaoExistenteNoConecta();
            ConfigurarExistenciaNoCoreSso(true);
            ConfigurarAtualizacaoNoCoreSso(true);
            ConfigurarParametroConfirmacaoEmail(new ParametroSistema
            {
                Valor = "true"
            });
            ConfigurarPersistencia(usuario => usuarioPersistido = usuario);
            ConfigurarEnvioEmailValidacao(true);

            var retorno = await sut.Executar(dto);

            Assert.True(retorno.ValidarEmail);
            Assert.Equal(MensagemNegocio.VALIDAR_EMAIL_USUARIO_EXTERNO, retorno.Mensagem);
            Assert.NotNull(usuarioPersistido);
            Assert.Equal(TipoUsuario.RedeParceria, usuarioPersistido!.Tipo);
            Assert.Equal(SituacaoUsuario.AguardandoValidacaoEmail, usuarioPersistido.Situacao);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AtualizarUsuarioServicoAcessoCommand>(c =>
                        c.Login == CpfSemMascara &&
                        c.Nome == dto.Nome &&
                        c.Email == dto.Email &&
                        c.Senha == dto.Senha &&
                        c.NomeSocial == dto.NomeSocial),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(c =>
                        c.Login == CpfSemMascara),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(false, "valor-invalido")]
        public async Task Executar_Quando_parametro_for_nulo_ou_invalido_Deve_confirmar_email_por_padrao(
            bool retornarParametroNulo,
            string? valorParametro)
        {
            var dto = CriarDtoValido();
            Usuario? usuarioPersistido = null;

            ConfigurarUsuarioNaoExistenteNoConecta();
            ConfigurarExistenciaNoCoreSso(false);
            ConfigurarCadastroNoCoreSso(true);
            ConfigurarParametroConfirmacaoEmail(
                retornarParametroNulo
                    ? null
                    : new ParametroSistema { Valor = valorParametro! });
            ConfigurarPersistencia(usuario => usuarioPersistido = usuario);
            ConfigurarEnvioEmailValidacao(true);

            var retorno = await sut.Executar(dto);

            Assert.True(retorno.ValidarEmail);
            Assert.Equal(MensagemNegocio.VALIDAR_EMAIL_USUARIO_EXTERNO, retorno.Mensagem);
            Assert.NotNull(usuarioPersistido);
            Assert.Equal(SituacaoUsuario.AguardandoValidacaoEmail, usuarioPersistido!.Situacao);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(c =>
                        c.Login == CpfSemMascara),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Executar_Quando_CoreSSO_rejeitar_cadastro_ou_atualizacao_Deve_lancar_excecao_e_nao_persistir(
            bool existeNoCoreSso)
        {
            var dto = CriarDtoValido();

            ConfigurarUsuarioNaoExistenteNoConecta();
            ConfigurarExistenciaNoCoreSso(existeNoCoreSso);
            ConfigurarCadastroNoCoreSso(false);
            ConfigurarAtualizacaoNoCoreSso(false);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(MensagemNegocio.USUARIO_JA_POSSUI_ACESSO_NO_CORRESSO, excecao.Message);

            if (existeNoCoreSso)
            {
                mediatorMock.Verify(
                    m => m.Send(
                        It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
                mediatorMock.Verify(
                    m => m.Send(
                        It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never);
            }
            else
            {
                mediatorMock.Verify(
                    m => m.Send(
                        It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                        It.IsAny<CancellationToken>()),
                    Times.Once);
                mediatorMock.Verify(
                    m => m.Send(
                        It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never);
            }

            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<ObterParametroSistemaPorTipoEAnoQuery>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<SalvarUsuarioCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private void ConfigurarUsuarioNaoExistenteNoConecta()
        {
            ConfigurarUsuarioPorLogin(null);

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorCpfQuery>(q => q.Cpf == CpfSemMascara),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario?)null);
        }

        private void ConfigurarUsuarioPorLogin(Usuario? usuario)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == CpfSemMascara),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario!);
        }

        private void ConfigurarExistenciaNoCoreSso(bool existe)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<UsuarioExisteNoCoreSsoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existe);
        }

        private void ConfigurarCadastroNoCoreSso(bool resultado)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarAtualizacaoNoCoreSso(bool resultado)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);
        }

        private void ConfigurarParametroConfirmacaoEmail(ParametroSistema? parametro)
        {
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterParametroSistemaPorTipoEAnoQuery>(q =>
                        q.TipoParametroSistema == TipoParametroSistema.ConfirmarEmailUsuarioExterno &&
                        q.Ano == anoAtual),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(parametro!);
        }

        private void ConfigurarPersistencia(Action<Usuario>? aoSalvar = null)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<SalvarUsuarioCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (request, _) =>
                    {
                        if (request is SalvarUsuarioCommand command)
                        {
                            aoSalvar?.Invoke(command.Usuario);
                        }
                    })
                .ReturnsAsync(true);
        }

        private void ConfigurarEnvioEmailValidacao(bool resultado)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultado);
        }

        private static UsuarioExternoDTO CriarDtoValido()
        {
            return new UsuarioExternoDTO
            {
                CodigoUnidade = "094765",
                Email = "maria.silva@teste.com.br",
                EmailEducacional = EmailEducacionalComEspacos,
                Tipo = null,
                Nome = "Maria da Silva",
                NomeSocial = "Maria Silva",
                Cpf = CpfComMascara,
                Senha = "Senha123",
                ConfirmarSenha = "Senha123",
                TipoEmail = TipoEmail.FuncionarioUnidadeParceira
            };
        }
    }
}
