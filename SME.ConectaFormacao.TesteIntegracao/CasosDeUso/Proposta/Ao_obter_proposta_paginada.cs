using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta_paginada : TestePropostaBase
    {
        public Ao_obter_proposta_paginada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada com filtro")]
        public async Task Deve_retornar_registros_com_filtro()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao();

            var propostas = await InserirNaBaseProposta(15);

            var filtro = PropostaPaginacaoMock.GerarPropostaFiltrosDTOValido(propostas.FirstOrDefault().AreaPromotora, propostas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(1);
            var propostaPesquisada = retorno.Items.FirstOrDefault();
            var propostaCadastro = propostas.FirstOrDefault();

            propostaPesquisada.Id.ShouldBe(propostaCadastro.Id);
            propostaPesquisada.TipoFormacao.ShouldBe(propostaCadastro.TipoFormacao.GetValueOrDefault().Nome());
            propostaPesquisada.AreaPromotora.ShouldBe(propostaCadastro.AreaPromotora.Nome);
            propostaPesquisada.Formato.ShouldBe(propostaCadastro.Formato.GetValueOrDefault().Nome());
            propostaPesquisada.NomeFormacao.ShouldBe(propostaCadastro.NomeFormacao);
            propostaPesquisada.NumeroHomologacao.ShouldBe(propostaCadastro.NumeroHomologacao.GetValueOrDefault());
            propostaPesquisada.DataRealizacaoInicio.ShouldBe(ObterDataFormatada(propostaCadastro?.DataRealizacaoInicio));
            propostaPesquisada.DataRealizacaoFim.ShouldBe(ObterDataFormatada(propostaCadastro?.DataRealizacaoFim));
            propostaPesquisada.Situacao.ShouldBe(propostaCadastro.Situacao.Nome());
            propostaPesquisada.FormacaoHomologada.ShouldBe(propostaCadastro.FormacaoHomologada.GetValueOrDefault());
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada sem filtro")]
        public async Task Deve_retornar_registros_sem_filtros()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao();

            await InserirNaBaseProposta(15);

            var filtro = new PropostaFiltrosDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(2);
        }

        [Fact(DisplayName = "Proposta - Não deve retornar registros consulta paginada filtros invalidos")]
        public async Task Nao_deve_retornar_registros_filtros_invalidos()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao();

            await InserirNaBaseProposta(15);

            var filtro = PropostaPaginacaoMock.GerarPropostaFiltrosDTOInvalido();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeFalse();
        }

        private void AdicionarPerfilUsuarioContextoAplicacao()
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
                {
                    { "PerfilUsuario", Perfis.ADMIN_DF.ToString() }
                };

            contextoAplicacao.AdicionarVariaveis(variaveis);
        }

        private string ObterDataFormatada(DateTime? data)
        {
            return data.HasValue ? data.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
    }
}
