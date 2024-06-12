using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Formacao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_formacao_detalhada : TestePropostaBase
    {
        public Ao_obter_formacao_detalhada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterEnderecoArquivoServicoArmazenamentoQuery, string>), typeof(ObterEnderecoArquivoServicoArmazenamentoQueryHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Formação Detalhada - Obter a formação detalhada por Id")]
        public async Task Deve_obter_formacao_detalhada_por_id()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(tipoInscricao: TipoInscricao.Optativa, situacao: SituacaoProposta.Publicada);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterFormacaoDetalhada>();

            // act 
            var retornoFormacaoDetalhadaDto = await casoDeUso.Executar(1);

            // assert
            retornoFormacaoDetalhadaDto.ShouldNotBeNull();
            retornoFormacaoDetalhadaDto.Titulo.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.AreaPromotora.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.FormatoDescricao.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.TipoFormacaoDescricao.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.Periodo.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.Justificativa.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.PublicosAlvo.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.PalavrasChaves.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.Turmas.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.LinkParaInscricoesExterna.ShouldBeNull();
            retornoFormacaoDetalhadaDto.PodeEnviarInscricao.ShouldBeTrue();
        }

        [Fact(DisplayName = "Formação Detalhada - Obter a formação detalhada do tipo inscrição externa")]
        public async Task Deve_obter_formacao_detalhada_tipo_inscricao_externa()
        {
            // arrange
            await InserirNaBaseProposta(tipoInscricao: TipoInscricao.Externa, situacao: SituacaoProposta.Publicada);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterFormacaoDetalhada>();

            // act 
            var retornoFormacaoDetalhadaDto = await casoDeUso.Executar(1);

            // assert
            retornoFormacaoDetalhadaDto.ShouldNotBeNull();
            retornoFormacaoDetalhadaDto.Titulo.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.AreaPromotora.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.FormatoDescricao.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.TipoFormacaoDescricao.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.Periodo.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.Justificativa.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.PublicosAlvo.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.PalavrasChaves.ShouldNotBeEmpty();
            retornoFormacaoDetalhadaDto.Turmas.ShouldNotBeEmpty();

            var proposta = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();
            retornoFormacaoDetalhadaDto.LinkParaInscricoesExterna.ShouldBe(proposta.LinkParaInscricoesExterna);
            retornoFormacaoDetalhadaDto.PodeEnviarInscricao.ShouldBeFalse();
        }
    }
}
