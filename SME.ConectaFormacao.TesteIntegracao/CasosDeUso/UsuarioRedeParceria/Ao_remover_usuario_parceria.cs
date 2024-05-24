using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria
{
    public class Ao_remover_usuario_parceria : TestePropostaBase
    {
        public Ao_remover_usuario_parceria(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<InativarUsuarioCoreSSOServicoAcessosCommand, bool>), typeof(InativarUsuarioCoreSSOServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<DesvincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(DesvincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao usuario nao encontrado")]
        public async Task Deve_retornar_excecao_usuario_nao_encontrado()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverUsuarioRedeParceria>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(1));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao usuario nao encontrado diferente de rede parceria")]
        public async Task Deve_retornar_excecao_usuario_nao_encontrado_diferente_rede_parceria()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario(Dominio.Enumerados.TipoUsuario.Externo);
            await InserirNaBase(usuario);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverUsuarioRedeParceria>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(usuario.Id));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve remover com sucesso")]
        public async Task Deve_remover_com_sucesso()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuario(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria);
            await InserirNaBase(usuarios);

            var usuario = usuarios.FirstOrDefault();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverUsuarioRedeParceria>();

            // act
            var retorno = await casoDeUso.Executar(usuario.Id);

            // assert
            retorno.Sucesso.ShouldBeTrue();
            var usuarioBanco = ObterPorId<Dominio.Entidades.Usuario, long>(usuario.Id);

            usuarioBanco.Excluido.ShouldBeTrue();
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve inativar usuário com sucesso")]
        public async Task Deve_remover_inativar_com_sucesso()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var usuarios = UsuarioMock.GerarUsuario(areaPromotora, Dominio.Enumerados.TipoUsuario.RedeParceria);
            await InserirNaBase(usuarios);

            var usuario = usuarios.FirstOrDefault();

            await InserirNaBaseProposta(criado_login: usuario.Login);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverUsuarioRedeParceria>();

            // act
            var retorno = await casoDeUso.Executar(usuario.Id);

            // assert
            retorno.Sucesso.ShouldBeTrue();
            var usuarioBanco = ObterPorId<Dominio.Entidades.Usuario, long>(usuario.Id);

            usuarioBanco.Situacao.ShouldBe(Dominio.Enumerados.SituacaoUsuario.Inativo);
        }
    }
}
