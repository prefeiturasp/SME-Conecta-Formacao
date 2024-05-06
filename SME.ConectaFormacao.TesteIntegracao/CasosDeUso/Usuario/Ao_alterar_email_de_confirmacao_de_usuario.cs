using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_alterar_email_de_confirmacao_de_usuario : TesteBase
    {
        public Ao_alterar_email_de_confirmacao_de_usuario(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
            UsuarioAlterarEmailValidacaoMock.Montar();
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>), typeof(ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand, bool>), typeof(EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandFake), ServiceLifetime.Scoped));
        }
        
        [Fact(DisplayName = "Usuario - Deve retornar sucesso na alteração de e-mail")]
        public async Task Deve_retornar_sucesso_ao_alterar_email()
        {
            // arrange
            var dto = new AlterarEmailUsuarioDto {Email = UsuarioAlterarEmailValidacaoMock.Email, Senha = UsuarioAlterarEmailValidacaoMock.Senha, Login = UsuarioAlterarEmailValidacaoMock.Login};
            var cadoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();
            
            // act
            var retorno = await cadoDeUso.Executar(dto);
            
            // assert
            retorno.ShouldBeTrue();
        }
        [Fact(DisplayName = "Usuario - Deve retornar erro 401 com login ou senha inválida na alteração de e-mail")]
        public async Task Deve_retornar_erro_401_com_login_ou_senha_invalido()
        {
            // arrange
            var dto = new AlterarEmailUsuarioDto {Email = UsuarioAlterarEmailValidacaoMock.Email, Senha = "3342342", Login = UsuarioAlterarEmailValidacaoMock.Login};
            var cadoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();
            
            // act
            var exception = await Should.ThrowAsync<NegocioException>(cadoDeUso.Executar(dto));
            
            // assert
            exception.ShouldNotBeNull();
            exception.StatusCode.ShouldBe(401);
        }
    }
}