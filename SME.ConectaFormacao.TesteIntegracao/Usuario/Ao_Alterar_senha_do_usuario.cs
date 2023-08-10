using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using SME.ConectaFormacao.TesteIntegracao.Usuario.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Usuario.ServicosFakes;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.Usuario
{
    public class Ao_Alterar_senha_do_usuario : TesteBase
    {
        public Ao_Alterar_senha_do_usuario(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            UsuarioAlterarSenhaMock.Montar();
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AlterarSenhaServicoAcessosCommand, bool>), typeof(AlterarSenhaServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário - Deve retornar excessão ao tentar alterar senha com senha atual inválida")]
        public async Task Deve_retornar_excessao_ao_tentar_alterar_senha_com_senha_atual_invalida()
        {
            // arrange
            var login = UsuarioAlterarSenhaMock.Login;
            var alterarSenhaUsuarioDto = UsuarioAlterarSenhaMock.AlterarSenhaUsuarioDTOSenhaAtualInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarSenha>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, alterarSenhaUsuarioDto));

            // assert
            excessao.ShouldNotBeNull();
            excessao.StatusCode.ShouldBe(400);
            excessao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.LOGIN_OU_SENHA_ATUAL_NAO_COMFEREM);
        }

        [Fact(DisplayName = "Usuário - Deve retornar excessão ao tentar alterar senha com confirmação inválida")]
        public async Task Deve_retornar_excessao_ao_tentar_alterar_senha_com_confirmacao_invalida()
        {
            // arrange
            var login = UsuarioAlterarSenhaMock.Login;
            var alterarSenhaUsuarioDto = UsuarioAlterarSenhaMock.AlterarSenhaUsuarioDTOConfirmacaoInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarSenha>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, alterarSenhaUsuarioDto));

            // assert
            excessao.ShouldNotBeNull();
            excessao.StatusCode.ShouldBe(400);
            excessao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.CONFIRMACAO_SENHA_INVALIDA);
        }

        [Fact(DisplayName = "Usuário - Deve retornar excessão ao tentar alterar senha sem critérios de segurança")]
        public async Task Deve_retornar_excessao_ao_tentar_alterar_senha_sem_criterios_de_segurancao()
        {
            // arrange
            var login = UsuarioAlterarSenhaMock.Login;
            var alterarSenhaUsuarioDto = UsuarioAlterarSenhaMock.AlterarSenhaUsuarioDTOCriterioSegurancaInvalido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarSenha>();

            // act
            var excessao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(login, alterarSenhaUsuarioDto));

            // assert
            excessao.ShouldNotBeNull();
            excessao.StatusCode.ShouldBe(400);
            excessao.Mensagens.FirstOrDefault().ShouldBe(MensagemNegocio.SENHA_NAO_ATENDE_CRITERIOS_SEGURANCA);
        }

        [Fact(DisplayName = "Usuário - Deve alterar senha com sucesso")]
        public async Task Deve_alterar_senha_com_sucesso()
        {
            // arrange
            var login = UsuarioAlterarSenhaMock.Login;
            var alterarSenhaUsuarioDto = UsuarioAlterarSenhaMock.AlterarSenhaUsuarioDTOValido;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoUsuarioAlterarSenha>();

            // act
            var retorno = await casoDeUso.Executar(login, alterarSenhaUsuarioDto);

            // assert
            retorno.ShouldBeTrue();
        }
    }
}
