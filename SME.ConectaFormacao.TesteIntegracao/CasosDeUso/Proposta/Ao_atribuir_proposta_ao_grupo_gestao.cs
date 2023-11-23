﻿using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Grupo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_atribuir_proposta_ao_grupo_gestao : TestePropostaBase
    {
        public Ao_atribuir_proposta_ao_grupo_gestao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
            AoObterParecerPropostaMock.Montar();
        }

        protected override void RegistrarFakes(IServiceCollection services)
        {
            base.RegistrarFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Atribuir Proposta ao Grupo Gestao - Deve alterar parecer e mudar o status da proposta para análise")]
        public async Task Deve_alterar_parecer_e_mudar_status_proposta_para_analise()
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
            parecerDaProposta.Situacao = SituacaoProposta.AguardandoAnaliseGestao;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAtribuirPropostaAoGrupoGestao>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id,parecerDaProposta.Parecer);

            // assert 
            retorno.ShouldBeTrue();
                
            var propostaAlterada = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();
            propostaAlterada.Situacao.ShouldBe(parecerDaProposta.Situacao);
            propostaAlterada.Situacao.ShouldNotBe(SituacaoProposta.AguardandoAnaliseDf);
            
            var parecerDaPropostaInserida = ObterTodos<Dominio.Entidades.PropostaMovimentacao>().FirstOrDefault();
            parecerDaPropostaInserida.Situacao.ShouldBe(parecerDaProposta.Situacao);
            parecerDaPropostaInserida.Situacao.ShouldNotBe(SituacaoProposta.AguardandoAnaliseDf);
            parecerDaPropostaInserida.Parecer.ShouldBe(parecerDaProposta.Parecer);
            parecerDaPropostaInserida.PropostaId.ShouldBe(proposta.Id);
        }
    }
}