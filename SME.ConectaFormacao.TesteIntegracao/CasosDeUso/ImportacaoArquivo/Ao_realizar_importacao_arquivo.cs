using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Arquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo
{
    public class Ao_realizar_importacao_arquivo : TestePropostaBase
    {
        public Ao_realizar_importacao_arquivo(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Importação Arquivo - Deve retornar excecao quando o arquivo estiver vazio")]
        public async Task Deve_retornar_excecao_arquivo_vazio()
        {
            // arrange
            var arquivo = new Mock<IFormFile>().Object;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoImportacaoArquivoInscricaoCursista>();

            // act
            var retorno = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(arquivo,1));
            
            // // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagens.Contains(MensagemNegocio.ARQUIVO_VAZIO).ShouldBeTrue();
        }

        [Fact(DisplayName = "Importação Arquivo - Deve inserir importacao arquivo")]
        public async Task Deve_inserir_importacao_arquivo()
        {
            // arrange
            var parametroQtdeCursistasSuportadosPorTurma = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, "950");
            await InserirNaBase(parametroQtdeCursistasSuportadosPorTurma);

            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves, modalidades, anosTurmas, componentesCurriculares);
            
            var arquivoValido = ImportacaoArquivoMock.GerarArquivoValido();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoImportacaoArquivoInscricaoCursista>();

            // act
            var retorno = await casoDeUso.Executar(arquivoValido,1);

            // assert
            retorno.ShouldNotBeNull();
            retorno.Mensagem.ShouldBe(MensagemNegocio.ARQUIVO_IMPORTADO_COM_SUCESSO);
            retorno.EntidadeId.ShouldBe(1);
            retorno.Sucesso.ShouldBeTrue();
                
            var importacaoArquivos = ObterTodos<ImportacaoArquivo>();
            importacaoArquivos.Count().ShouldBe(1);
            importacaoArquivos.FirstOrDefault().Id.ShouldBe(1);
            importacaoArquivos.FirstOrDefault().Nome.ShouldNotBeEmpty();
            importacaoArquivos.FirstOrDefault().Situacao.ShouldBe(SituacaoImportacaoArquivo.CarregamentoInicial);
            importacaoArquivos.FirstOrDefault().Tipo.ShouldBe(TipoImportacaoArquivo.Inscricao_Manual);
            importacaoArquivos.FirstOrDefault().PropostaId.ShouldBe(1);
            importacaoArquivos.FirstOrDefault().Excluido.ShouldBeFalse();
        }
    }
}
