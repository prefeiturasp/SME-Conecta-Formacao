using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.Mock;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria
{
    public class Ao_alterar_usuario_parceria : TesteBase
    {
        public Ao_alterar_usuario_parceria(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<InativarUsuarioCoreSSOServicoAcessosCommand, bool>), typeof(InativarUsuarioCoreSSOServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AtualizarUsuarioServicoAcessoCommand, bool>), typeof(AtualizarUsuarioServicoAcessoCommandHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<DesvincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve alterar usuario parceria com sucesso")]
        public async Task Deve_alterar_usuario_parceria_com_sucesso()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuarios(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria);
            await InserirNaBase(usuarios);

            var usuario = usuarios.FirstOrDefault();
            var dto = UsuarioParceriaMock.GetUsuarioRedeParceriaDTOValido(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarUsuarioRedeParceria>();

            // act
            var retorno = await casoDeUso.Executar(usuario.Id, dto);

            // assert
            retorno.Sucesso.ShouldBeTrue();

            var usuarioBanco = ObterPorId<Dominio.Entidades.Usuario, long>(usuario.Id);

            usuarioBanco.Login.ShouldBe(usuario.Login);
            usuarioBanco.Cpf.ShouldBe(usuario.Cpf);
            usuarioBanco.Nome.ShouldBe(dto.Nome);
            usuarioBanco.Email.ShouldBe(dto.Email);
            usuarioBanco.Telefone.ShouldBe(dto.Telefone);
            usuarioBanco.Tipo.ShouldBe(Dominio.Enumerados.TipoUsuario.RedeParceria);
            usuarioBanco.Situacao.ShouldBe(Dominio.Enumerados.SituacaoUsuario.Ativo);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao validacao preenchimento")]
        public async Task Deve_retornar_excecao_validacao_preenchimento()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuarios(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria);
            await InserirNaBase(usuarios);

            var usuario = usuarios.FirstOrDefault();
            var dto = UsuarioParceriaMock.GetUsuarioRedeParceriaDTOInvalido(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarUsuarioRedeParceria>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(usuario.Id, dto));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.NOME_DEVE_TER_SOBRENOME);
            retorno.Mensagens.Contains(MensagemNegocio.EMAIL_INVALIDO);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao usuario nao encontrado")]
        public async Task Deve_retornar_excecao_usuario_nao_encontrado()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var dto = UsuarioParceriaMock.GetUsuarioRedeParceriaDTOInvalido(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarUsuarioRedeParceria>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(1, dto));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }
    }
}
