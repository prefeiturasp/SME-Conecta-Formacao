using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_parecer_da_proposta : TestePropostaBase
    {
        public Ao_salvar_parecer_da_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AoObterParecerPropostaMock.Montar();
        }

        protected override void RegistrarFakes(IServiceCollection services)
        {
            base.RegistrarFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Parecer Proposta - Deve alterar parecer e mudar o status da proposta")]
        public async Task Deve_alterar_parecer_e_mudar_status_proposta()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves);

            var parecerDaProposta = AoObterParecerPropostaMock.PropostaMovimentacaoDto;
            parecerDaProposta.Situacao = SituacaoProposta.Favoravel;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarParecerDaProposta>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id,parecerDaProposta);

            // assert 
            retorno.ShouldBeTrue();
                
            var propostaAlterada = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();
            propostaAlterada.Situacao.ShouldBe(parecerDaProposta.Situacao);
            propostaAlterada.AlteradoEm.ShouldNotBeNull();
            propostaAlterada.AlteradoEm.Value.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
            propostaAlterada.AlteradoPor.ShouldNotBeNull();
            propostaAlterada.AlteradoLogin.ShouldNotBeNull();
            
            var parecerDaPropostaInserida = ObterTodos<Dominio.Entidades.PropostaMovimentacao>().FirstOrDefault();
            parecerDaPropostaInserida.Situacao.ShouldBe(parecerDaProposta.Situacao);
            parecerDaPropostaInserida.Justificativa.ShouldBe(parecerDaProposta.Justificativa);
            parecerDaPropostaInserida.PropostaId.ShouldBe(proposta.Id);
            parecerDaPropostaInserida.CriadoEm.Date.ShouldBe(DateTimeExtension.HorarioBrasilia().Date);
            parecerDaPropostaInserida.CriadoPor.ShouldNotBeEmpty();
            parecerDaPropostaInserida.CriadoLogin.ShouldNotBeEmpty();
            parecerDaPropostaInserida.AlteradoEm.ShouldBeNull();
            parecerDaPropostaInserida.AlteradoPor.ShouldBeNull();
            parecerDaPropostaInserida.AlteradoLogin.ShouldBeNull();
        }
    }
}