using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Comandos.PublicarNaFilaRabbit;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo
{
    public class Ao_alterar_situacao_arquivo : TestePropostaBase
    {
        public Ao_alterar_situacao_arquivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<PublicarNaFilaRabbitCommand, bool>), typeof(PublicarNaFilaRabbitCommandFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Importação Arquivo - Deve alterar situação do arquivos para aguardando processamento")]
        public async Task Deve_alterar_situacao_arquivos_para_aguardando_processamento()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validado, 2);
            await InserirNaBase(arquivosValidado);

            // act
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInscricaoManualContinuarProcessamento>();

            var retorno = await casoDeUso.Executar(arquivosValidado.FirstOrDefault().Id);

            // assert
            var arquivos = ObterTodos<Dominio.Entidades.ImportacaoArquivo>();
            arquivos.ShouldNotBeNull();
            arquivos.Count(a => a.Situacao == SituacaoImportacaoArquivo.AguardandoProcessamento).ShouldBe(1);
        }

        [Fact(DisplayName = "Importação Arquivo - Situação do arquivo deve ser validado")]
        public async Task Situacao_arquivo_deve_ser_validado()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosInicial = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial, 1);
            await InserirNaBase(arquivosInicial);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInscricaoManualContinuarProcessamento>();
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(arquivosInicial.FirstOrDefault().Id));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.SITUACAO_DO_ARQUIVO_DEVE_SER_VALIDADO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Importação Arquivo - Deve alterar situação do arquivo para cancelado")]
        public async Task Deve_alterar_situacao_arquivo_para_cancelado()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validado, 2);
            await InserirNaBase(arquivosValidado);

            // act
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInscricaoManualCancelarProcessamento>();

            var retorno = await casoDeUso.Executar(arquivosValidado.FirstOrDefault().Id);

            // assert
            var arquivos = ObterTodos<Dominio.Entidades.ImportacaoArquivo>();
            arquivos.ShouldNotBeNull();
            arquivos.Count(a => a.Situacao == SituacaoImportacaoArquivo.Cancelado).ShouldBe(1);
        }

        [Fact(DisplayName = "Importação Arquivo - Arquivo não encontrado ao cancelar")]
        public async Task Arquivo_nao_encontrado_ao_cancelar()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosInicial = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial, 1);
            await InserirNaBase(arquivosInicial);

            // act 
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInscricaoManualCancelarProcessamento>();
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(99));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.ARQUIVO_NAO_ENCONTRADO).ShouldBeTrue();
        }
    }
}
