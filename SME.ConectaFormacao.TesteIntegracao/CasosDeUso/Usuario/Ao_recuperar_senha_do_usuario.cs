using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_recuperar_senha_do_usuario : TesteBase
    {
        public Ao_recuperar_senha_do_usuario(CollectionFixture collectionFixture) : base(collectionFixture, false)
        {
            UsuarioRecuperarSenhaMock.Montar();
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommand, string>), typeof(SolicitarRecuperacaoSenhaServicoAcessosPorLoginCommandHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AlterarSenhaServicoAcessosPorTokenCommand, string>), typeof(AlterarSenhaServicoAcessosPorTokenCommandHandlerFake), ServiceLifetime.Scoped));
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ValidarTokenRecuperacaoSenhaServicoAcessosQuery, bool>), typeof(ValidarTokenRecuperacaoSenhaServicoAcessosQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterPerfisUsuarioServicoAcessosPorLoginQuery, UsuarioPerfisRetornoDTO>), typeof(ObterPerfisUsuarioServicoAcessosPorLoginQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário - Deve retornar exceção ao solicitar recuperãção de senha com login do usuário inválido")]
        public async Task Deve_retornar_excecao_quando_usuario_nao_encontrado()
        {
            // arrange
            var loginInvalido = UsuarioRecuperarSenhaMock.LoginInvalido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(loginInvalido));

            // assert
            excecao.ShouldNotBeNull();
            excecao.StatusCode.ShouldBe(400);
            excecao.Mensagens.Contains(MensagemNegocio.LOGIN_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Usuário - Deve retornar orientação ao solicitar recuperação de senha com login do usuário válido")]
        public async Task Deve_retornar_orientacao_recuperacao_senha()
        {
            // arrange
            var login = UsuarioRecuperarSenhaMock.LoginValido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioSolicitarRecuperacaoSenha>();

            var orientacaoEsperada = string.Format(MensagemNegocio.ORIENTACOES_RECUPERACAO_SENHA, UsuarioRecuperarSenhaMock.EmailValido.TratarEmail());

            // act
            var retorno = await casoDeUso.Executar(login);

            // assert
            retorno.ShouldNotBeNull();
            retorno.ShouldBe(orientacaoEsperada);
        }

        [Fact(DisplayName = "Usuário - Deve retornar falso ao validar recuperação de senha com token invalido")]
        public async Task Deve_retornar_falso_quando_token_recuperacao_senha_invalido()
        {
            // arrange
            var token = UsuarioRecuperarSenhaMock.TokenInvalido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioValidarTokenRecuperacaoSenha>();

            // act
            var retorno = await casoDeUso.Executar(token);

            // assert
            retorno.ShouldBeFalse();
        }

        [Fact(DisplayName = "Usuário - Deve retornar verdadeiro ao validar recuperação de senha com token válido")]
        public async Task Deve_retornar_verdadeiro_quando_token_recuperacao_senha_valido()
        {
            // arrange
            var token = UsuarioRecuperarSenhaMock.TokenValido;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioValidarTokenRecuperacaoSenha>();

            // act
            var retorno = await casoDeUso.Executar(token);

            // assert
            retorno.ShouldBeTrue();
        }

        [Fact(DisplayName = "Usuário - Deve retornar verdadeiro ao validar recuperação de senha com token válido")]
        public async Task Deve_retornar_token_quando_recuperacao_de_senha_for_realizada_com_sucesso()
        {
            // arrange
            var recuperacaoSenhaDto = UsuarioRecuperarSenhaMock.RecuperacaoSenhaDto;
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioRecuperarSenha>();

            // act
            var retorno = await casoDeUso.Executar(recuperacaoSenhaDto);

            // assert
            retorno.ShouldNotBeNull();
            retorno.UsuarioLogin.ShouldBe(UsuarioRecuperarSenhaMock.LoginValido);

            var usuarios = ObterTodos<Dominio.Entidades.Usuario>();
            usuarios.Any().ShouldBeTrue();

            var usuario = usuarios.FirstOrDefault();
            usuario.Nome.ShouldNotBeNull(retorno.UsuarioNome);
            usuario.Login.ShouldNotBeNull(retorno.UsuarioLogin);
        }
    }
}
