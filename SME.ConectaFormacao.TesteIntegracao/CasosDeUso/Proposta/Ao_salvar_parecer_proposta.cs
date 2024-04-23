using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_parecer_proposta : TestePropostaBase
    {
        public Ao_salvar_parecer_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta parecer - Não deve cadastrar um parecer sem campo")]
        public async Task Nao_deve_cadastrar_proposta_parecer_sem_campo()
        {
            // arrange
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaParecer>();
            
            var proposta = await InserirNaBaseProposta();
            
            var propostaParecerDto = PropostaSalvarMock.GerarParecer();
            propostaParecerDto.PropostaId = proposta.Id;
            
            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(propostaParecerDto));

            // assert
            excecao.ShouldNotBeNull();
        }
    }
}