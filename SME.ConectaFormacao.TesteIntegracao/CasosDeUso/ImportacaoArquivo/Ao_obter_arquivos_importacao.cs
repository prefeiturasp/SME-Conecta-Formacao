using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo
{
    public class Ao_obter_arquivos_importacao : TestePropostaBase
    {
        public Ao_obter_arquivos_importacao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Inscrição - Deve obter arquivos de importação paginada")]
        public async Task Ao_obter_arquivos_importacao_paginada()
        {
            // arrange
            AdicionarContextoAplicacaoPaginada("1", "2");

            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosInicial = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial, 2);
            await InserirNaBase(arquivosInicial);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validando, 2);
            await InserirNaBase(arquivosValidado);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterArquivosInscricaoImportados>();

            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            retorno.ShouldNotBeNull();
            retorno.TotalRegistros.ShouldBe(4);
            retorno.TotalPaginas.ShouldBe(2);
            retorno.Items.Count(r => r.Situacao == SituacaoImportacaoArquivo.Validando).ShouldBe(2);
            retorno.Items.Count(r => r.Situacao == SituacaoImportacaoArquivo.CarregamentoInicial).ShouldBe(0);
        }

        [Fact(DisplayName = "Inscrição - Deve obter arquivos de importação paginada com total de regitros")]
        public async Task Ao_obter_arquivos_importacao_paginada_com_total_registros()
        {
            // arrange
            AdicionarContextoAplicacaoPaginada("1", "2");

            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosInicial = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial, 2);
            await InserirNaBase(arquivosInicial);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validando, 2);
            await InserirNaBase(arquivosValidado);

            var registroProcessado = ImportacaoArquivoRegistroMock.GerarImportacaoArquivo(arquivosValidado.FirstOrDefault().Id, SituacaoImportacaoArquivoRegistro.Processado, 2);
            await InserirNaBase(registroProcessado);

            var registroValidado = ImportacaoArquivoRegistroMock.GerarImportacaoArquivo(arquivosValidado.FirstOrDefault().Id, SituacaoImportacaoArquivoRegistro.Validado, 2);
            await InserirNaBase(registroValidado);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterArquivosInscricaoImportados>();

            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            retorno.ShouldNotBeNull();
            retorno.TotalRegistros.ShouldBe(4);
            retorno.TotalPaginas.ShouldBe(2);
            var arquivoValidado = retorno.Items.ToList().Find(r => r.Id == arquivosValidado.FirstOrDefault().Id);
            arquivoValidado.ShouldNotBeNull();
            arquivoValidado.TotalProcessados.ShouldBe(2);
            arquivoValidado.TotalRegistros.ShouldBe(4);
        }

        private void AdicionarContextoAplicacaoPaginada(string pagina, string numeroRegistro)
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
            {
                { "NumeroPagina", pagina },
                { "NumeroRegistros", numeroRegistro }
            };

            contextoAplicacao.AdicionarVariaveis(variaveis);
        }
    }
}
