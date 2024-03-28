using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_inserir_usuario_externo : TesteBase
    {
        public Ao_inserir_usuario_externo(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>), typeof(CadastrarUsuarioServicoAcessoCommandHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<UsuarioExisteNoCoreSsoQuery, bool>), typeof(UsuarioExisteNoCoreSsoQueryFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand, bool>), typeof(EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandFake), ServiceLifetime.Scoped));
        }


        [Fact(DisplayName = "Usuário - Deve Cadastrar Um Usuario Externo - aguardando validar email")]
        public async Task Deve_Cadastrar_Usuario_Externo_aguardando_validar_email()
        {
            //arrange

            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var retorno = await casoDeUso.InserirUsuarioExterno(usuarioExterno);

            // assert
            retorno.Mensagem.ShouldBe(MensagemNegocio.VALIDAR_EMAIL_USUARIO_EXTERNO);

            var usuario = ObterTodos<Dominio.Entidades.Usuario>();

            usuario.FirstOrDefault().Situacao.ShouldBe(Dominio.Enumerados.SituacaoCadastroUsuario.AguardandoValidacaoEmail);

        }

        [Fact(DisplayName = "Usuário - Deve Cadastrar Um Usuario Externo - sem validar email")]
        public async Task Deve_Cadastrar_Usuario_Externo_sem_validar_email()
        {
            //arrange
            var parametro = ParametroSistemaMock.GerarParametroSistema(Dominio.Enumerados.TipoParametroSistema.ConfirmarEmailUsuarioExterno, "false");
            await InserirNaBase(parametro);

            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var retorno = await casoDeUso.InserirUsuarioExterno(usuarioExterno);

            // assert
            retorno.Mensagem.ShouldBe(MensagemNegocio.USUARIO_EXTRNO_CADASTRADO_COM_SUCESSO);

            var usuario = ObterTodos<Dominio.Entidades.Usuario>();

            usuario.FirstOrDefault().Situacao.ShouldBe(Dominio.Enumerados.SituacaoCadastroUsuario.Ativo);
        }

        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo com a Confirmação de Senha Diferente da Senha")]
        public async Task Deve_Cadastrar_Usuario_Externo_Com_ConfirmacaoSenha_Diferente_da_Senha()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.ConfirmarSenha = string.Empty;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.CONFIRMACAO_SENHA_DEVE_SER_IGUAL_A_SENHA);
        }
        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo com senha Menor que 8 digitos")]
        public async Task Deve_Cadastrar_Usuario_Externo_Com_senha_menor_que_oito()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.Senha = usuarioExterno.Senha.Substring(6);
            usuarioExterno.ConfirmarSenha = usuarioExterno.ConfirmarSenha.Substring(6);
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÍNIMO_8_CARACTERES);
        }

        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo com senha Maior que  12 digitos")]
        public async Task Deve_Cadastrar_Usuario_Externo_Com_senha_maior_que_doze()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.Senha = $"{usuarioExterno.Senha}{usuarioExterno.Senha}";
            usuarioExterno.ConfirmarSenha = $"{usuarioExterno.ConfirmarSenha}{usuarioExterno.ConfirmarSenha}";
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.A_SENHA_DEVE_TER_NO_MÁXIMO_12_CARACTERES);
        }

        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo com senha que tenha espaço")]
        public async Task Deve_Cadastrar_Usuario_Externo_Com_senha_com_espaco()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.Senha = $"{usuarioExterno.Senha} {usuarioExterno.Senha}";
            usuarioExterno.ConfirmarSenha = $"{usuarioExterno.ConfirmarSenha} {usuarioExterno.ConfirmarSenha}";
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.A_SENHA_NAO_PODE_CONTER_ESPACOS_EM_BRANCO);
        }
        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo com senha que tenha acentuação")]
        public async Task Deve_Cadastrar_Usuario_Externo_Com_senha_com_acentuacao()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.Senha = $"{usuarioExterno.Senha.Substring(1)}ã";
            usuarioExterno.ConfirmarSenha = $"{usuarioExterno.ConfirmarSenha.Substring(1)}ã";
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.A_SENHA_DEVE_CONTER_SOMENTE);
        }
        
        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo com E-mail Edu Invalido")]
        public async Task Deve_Cadastrar_Usuario_Externo_Com_Email_Edu_Invalido()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.EmailEducacional = "teste@edu.sme.prefeitura.sp.gov";
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.EMAIL_EDU_INVALIDO.Parametros(usuarioExterno.EmailEducacional));
        }
        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo sem informar o E-mail Edu")]
        public async Task Deve_Cadastrar_Usuario_Externo_sem_informar_email_edu()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.EmailEducacional = null;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.EMAIL_EDU_INVALIDO);
        }
        [Fact(DisplayName = "Usuário - Não Deve Cadastrar Um Usuario Externo Caso o Front envie uma string vazia no email @edu")]
        public async Task Deve_Cadastrar_Usuario_Externo_caso_front_envia_string_vazia_email_edu()
        {
            //arrange
            var usuarioExterno = UsuarioInserirExternoMock.GerarUsuarioExternoDTO();
            usuarioExterno.EmailEducacional = string.Empty;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioExterno>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.InserirUsuarioExterno(usuarioExterno));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.EMAIL_EDU_INVALIDO);
        }
    }
}