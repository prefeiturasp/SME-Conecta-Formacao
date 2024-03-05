using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_informacoes_cadastrante_proposta : TesteBase
    {
        public Ao_obter_informacoes_cadastrante_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            PropostaInformacoesCadastranteMock.Montar();
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerInformacoesCadastranteFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterNomeUsuarioLogadoQuery, string>), typeof(ObterNomeUsuarioLogadoQueryHandlerInformacoesCadastranteFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterEmailUsuarioLogadoQuery, string>), typeof(ObterEmailUsuarioLogadoQueryHandlerInformacoesCadastranteFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve obter as informações do cadastrante valido")]
        public async Task Deve_obter_informacoes_cadastrante_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaInformacoesCadastranteMock.UsuarioLogadoGrupoId);
            await InserirNaBase(areaPromotora);

            var telefones = AreaPromotoraMock.GerarAreaTelefone(3, areaPromotora.Id);
            await InserirNaBase(telefones);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInformacoesCadastrante>();

            // act
            var retorno = await casoDeUso.Executar(null);

            // Assert
            retorno.UsuarioLogadoNome.ShouldBe(PropostaInformacoesCadastranteMock.UsuarioLogadoNome);
            retorno.UsuarioLogadoEmail.ShouldBe(PropostaInformacoesCadastranteMock.UsuarioLogadoEmail);
            retorno.AreaPromotora.ShouldBe(areaPromotora.Nome);
            retorno.AreaPromotoraTipo.ShouldBe(areaPromotora.Tipo.Nome());
            retorno.AreaPromotoraEmails.ShouldBe(areaPromotora.Email.Replace(";", ", "));
            retorno.AreaPromotoraTelefones.ShouldBe(string.Join(", ", telefones.Select(t => t.Telefone.Length > 10 ? t.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : t.Telefone.AplicarMascara(@"\(00\) 0000\-0000"))));
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando o perfil não possui area promotora")]
        public async Task Deve_retornar_excecao_perfil_sem_area_promotora()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var telefones = AreaPromotoraMock.GerarAreaTelefone(3, areaPromotora.Id);
            await InserirNaBase(telefones);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInformacoesCadastrante>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(null));

            // Assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA_GRUPO_USUARIO);
        }

        [Fact(DisplayName = "Proposta - Deve obter as informações do cadastrante da proposta")]
        public async Task Deve_obter_informacoes_cadastrante_da_proposta()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaInformacoesCadastranteMock.UsuarioLogadoGrupoId);
            await InserirNaBase(areaPromotora);

            var proposta = PropostaMock.GerarPropostaRascunho(areaPromotora.Id);
            proposta.CriadoLogin = usuario.Login;
            await InserirNaBase(proposta);

            var telefones = AreaPromotoraMock.GerarAreaTelefone(3, areaPromotora.Id);
            await InserirNaBase(telefones);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInformacoesCadastrante>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id);

            // Assert
            retorno.UsuarioLogadoNome.ShouldBe(usuario.Nome);
            retorno.UsuarioLogadoEmail.ShouldBe(usuario.Email);
            retorno.AreaPromotora.ShouldBe(areaPromotora.Nome);
            retorno.AreaPromotoraTipo.ShouldBe(areaPromotora.Tipo.Nome());
            retorno.AreaPromotoraEmails.ShouldBe(areaPromotora.Email.Replace(";", ", "));
            retorno.AreaPromotoraTelefones.ShouldBe(string.Join(", ", telefones.Select(t => t.Telefone.Length > 10 ? t.Telefone.AplicarMascara(@"\(00\) 00000\-0000") : t.Telefone.AplicarMascara(@"\(00\) 0000\-0000"))));
        }
    }
}
