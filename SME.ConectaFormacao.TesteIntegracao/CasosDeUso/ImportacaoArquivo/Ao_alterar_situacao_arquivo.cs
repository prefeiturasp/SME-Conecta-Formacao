using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma;
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

            var arquivosInicial = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial, 2);
            await InserirNaBase(arquivosInicial);

            var arquivosValidando = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validando, 2);
            await InserirNaBase(arquivosValidando);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validado, 2);
            await InserirNaBase(arquivosValidado);

            // act
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarSituacaoArquivosParaAguardandoProcessamento>();

            var retorno = await casoDeUso.Executar(proposta.Id);

            // assert
            var arquivos = ObterTodos<Dominio.Entidades.ImportacaoArquivo>();
            arquivos.ShouldNotBeNull();
            arquivos.Count(a => a.Situacao == SituacaoImportacaoArquivo.AguardandoProcessamento).ShouldBe(2);
        }
    }
}
