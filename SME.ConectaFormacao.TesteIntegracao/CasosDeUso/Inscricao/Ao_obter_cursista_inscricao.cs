using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using SME.ConectaFormacao.Aplicacao.Dtos;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_obter_cursista_inscricao : TesteBase
    {
        public Ao_obter_cursista_inscricao(CollectionFixture collectionFixture, bool limparBanco = true) : base(collectionFixture, limparBanco)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterNomeCpfProfissionalPorRegistroFuncionalQuery, RetornoUsuarioDTO>), typeof(ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Inscrição - Deve obter nome cursista")]
        public async Task Deve_obter_nome_cursista_com_sucesso()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNomeCpfCursistaInscricao>();

            // act
            var cursista = await casoDeUso.Executar(usuario.Login, null);

            // assert
            cursista.Nome.ShouldBe(usuario.Nome);
        }

        [Fact(DisplayName = "Inscrição - Deve obter nome cursista eol")]
        public async Task Deve_obter_nome_cursista_eol_com_sucesso()
        {
            // arrange

            var usuario = UsuarioMock.GerarUsuario();
            ObterNomeCursistaInscricaoMock.Usuario = usuario;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNomeCpfCursistaInscricao>();

            // act
            var cursista = await casoDeUso.Executar(usuario.Login, null);

            // assert
            cursista.Nome.ShouldBe(usuario.Nome);
        }

        [Fact(DisplayName = "Inscrição - Deve obter nome cursista eol")]
        public async Task Deve_retornar_excecao_ao_obter_nome_cursista_nao_encontrado()
        {
            // arrange

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterNomeCpfCursistaInscricao>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar("teste", null));

            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.CURSISTA_NAO_ENCONTRADO);
        }
    }
}
