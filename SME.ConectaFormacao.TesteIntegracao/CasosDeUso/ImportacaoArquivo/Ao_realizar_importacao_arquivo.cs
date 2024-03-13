using Microsoft.AspNetCore.Http;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo
{
    public class Ao_realizar_importacao_arquivo : TesteBase
    {
        public Ao_realizar_importacao_arquivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Importação Arquivo - Deve retornar excecao quando o arquivo estiver vazio")]
        public async Task Deve_retornar_excecao_arquivo_vazio()
        {
            // arrange
            var importacaoArquivo = ImportacaoArquivoMock.GerarImportacaoArquivoVazia();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoImportacaoArquivoInscricaoCursista>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(importacaoArquivo));

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.ARQUIVO_VAZIO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Importação Arquivo - Deve inserir importacao arquivo")]
        public async Task Deve_inserir_importacao_arquivo()
        {
            // arrange
            var importacaoArquivo = ImportacaoArquivoMock.GerarImportacaoArquivoValida();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoImportacaoArquivoInscricaoCursista>();

            // act
            var retorno = await casoDeUso.Executar(importacaoArquivo);

            // assert
            retorno.ShouldNotBeNull();
            var importacaoArquivos = ObterTodos<ImportacaoArquivo>();
            importacaoArquivos.Count().ShouldBe(1);
        }
    }
}
