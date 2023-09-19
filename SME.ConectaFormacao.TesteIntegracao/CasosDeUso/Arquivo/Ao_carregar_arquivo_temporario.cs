using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo
{
    public class Ao_carregar_arquivo_temporario : TesteBase
    {
        public Ao_carregar_arquivo_temporario(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarCommandFakes(IServiceCollection services)
        {
            base.RegistrarCommandFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ArmazenarArquivoTemporarioServicoArmazenamentoCommand, string>), typeof(ArmazenarArquivoTemporarioServicoArmazenamentoCommandHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Arquivo - Deve retornar excecao quando o arquivo estiver vazio")]
        public async Task Deve_retornar_excecao_arquivo_vazio()
        {
            // arrange
            var formFile = CarregarArquivoTemporarioMock.GerarFormFileVazio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoArquivoCarregarTemporario>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(formFile));

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.ARQUIVO_VAZIO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Arquivo - Deve retornar excecao quando o arquivo for maior que 10 mb")]
        public async Task Deve_retornar_excecao_arquivo_maior_que_10_mb()
        {
            // arrange
            var formFile = CarregarArquivoTemporarioMock.GerarFormFileMaiorQueDezMb();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoArquivoCarregarTemporario>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(formFile));

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.ARQUIVO_MAIOR_QUE_10_MB).ShouldBeTrue();
        }

        [Fact(DisplayName = "Arquivo - Deve retornar excecao quando o arquivo for maior que 10 mb")]
        public async Task Deve_carregar_arquivo_com_sucesso()
        {
            // arrange
            var formFile = CarregarArquivoTemporarioMock.GerarFormFileValido();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoArquivoCarregarTemporario>();

            // act
            var retorno = await casoDeUso.Executar(formFile);

            // assert
            retorno.ShouldNotBeNull();
        }
    }
}
