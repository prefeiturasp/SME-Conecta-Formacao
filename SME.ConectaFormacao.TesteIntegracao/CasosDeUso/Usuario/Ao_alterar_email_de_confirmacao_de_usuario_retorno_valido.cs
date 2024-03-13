using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Autenticacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Usuario
{
    public class Ao_alterar_email_de_confirmacao_de_usuario_retorno_valido : TesteBase
    {
        public Ao_alterar_email_de_confirmacao_de_usuario_retorno_valido(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioServicoAcessosPorLoginSenhaQuery, UsuarioAutenticacaoRetornoDTO>), typeof(ObterUsuarioServicoAcessosPorLoginSenhaQueryHandlerRetornoValido), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<EnviarEmailValidacaoUsuarioExternoServicoAcessoCommand, bool>), typeof(EnviarEmailValidacaoUsuarioExternoServicoAcessoCommandFake), ServiceLifetime.Scoped));
        }
        
        [Fact(DisplayName = "Usuario - Deve retornar sucesso na alteração de e-mail")]
        public async Task Deve_retornar_sucesso_ao_alterar_email()
        {
            // arrange
            var dto = new AlterarEmailUsuarioDto {Email = "teste@teste.com", Senha = "xyz@232323", Login = "12121212121"};
            var cadoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarEmailEReenviarEmailParaValidacao>();
            
            // act
            var retorno = await cadoDeUso.Executar(dto);
            
            // assert
            retorno.ShouldBeTrue();
        }
    }
}