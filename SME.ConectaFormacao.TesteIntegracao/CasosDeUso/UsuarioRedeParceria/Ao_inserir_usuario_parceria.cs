using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.Mock;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.UsuarioRedeParceria
{
    public class Ao_inserir_usuario_parceria : TesteBase
    {
        public Ao_inserir_usuario_parceria(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<UsuarioExisteNoCoreSsoQuery, bool>), typeof(UsuarioExisteNoCoreSsoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<AtualizarUsuarioServicoAcessoCommand, bool>), typeof(AtualizarUsuarioServicoAcessoCommandHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<CadastrarUsuarioServicoAcessoCommand, bool>), typeof(CadastrarUsuarioServicoAcessoCommandHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<VincularPerfilExternoCoreSSOServicoAcessosCommand, bool>), typeof(VincularPerfilExternoCoreSSOServicoAcessosCommandHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve inserir usuario parceria com sucesso")]
        public async Task Deve_inserir_usuario_parceria_com_sucesso()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var dto = UsuarioParceriaMock.GetUsuarioRedeParceriaDTOValido(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioRedeParceria>();

            // act
            var retorno = await casoDeUso.Executar(dto);

            // assert
            retorno.Sucesso.ShouldBeTrue();

            var usuarios = ObterTodos<Dominio.Entidades.Usuario>();

            usuarios.FirstOrDefault().Nome.ShouldBe(dto.Nome);
            usuarios.FirstOrDefault().Tipo.ShouldBe(Dominio.Enumerados.TipoUsuario.RedeParceria);
            usuarios.FirstOrDefault().Situacao.ShouldBe(Dominio.Enumerados.SituacaoUsuario.Ativo);
        }

        [Fact(DisplayName = "Usuário Rede Parceria - deve retornar excecao validacao preenchimento")]
        public async Task Deve_retornar_excecao_validacao_preenchimento()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var dto = UsuarioParceriaMock.GetUsuarioRedeParceriaDTOInvalido(areaPromotora);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirUsuarioRedeParceria>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(dto));

            // assert
            retorno.Mensagens.Contains(MensagemNegocio.CPF_COM_DIGITO_VERIFICADOR_INVALIDO.Parametros(dto.Cpf));
            retorno.Mensagens.Contains(MensagemNegocio.NOME_DEVE_TER_SOBRENOME);
            retorno.Mensagens.Contains(MensagemNegocio.EMAIL_INVALIDO);
        }
    }
}
