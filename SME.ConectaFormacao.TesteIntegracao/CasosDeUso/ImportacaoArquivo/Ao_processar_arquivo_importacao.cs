using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo
{
    public class Ao_processar_arquivo_importacao : TestePropostaBase
    {
        public Ao_processar_arquivo_importacao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<PublicarNaFilaRabbitCommand, bool>), typeof(PublicarNaFilaProcessamentoDeRegistroCommandFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CursistaCargoServicoEol>>), typeof(ObterCargosFuncoesDresFuncionarioServicoEolQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Importação Arquivo - Processar arquivo de importação com registros validados")]
        public async Task Ao_processar_arquivo_importacao_com_registros()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();
            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validando, 1);
            await InserirNaBase(arquivosValidado);

            var parametro = ParametroSistemaMock.GerarParametroSistema(TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista, "1");
            await InserirNaBase(parametro);

            var conteudo1 = JsonConvert.SerializeObject(new InscricaoCursistaImportacaoDTO()
            {
                ColaboradorRede = "1",
                Cpf = usuario.Cpf,
                RegistroFuncional = usuario.Login,
                Turma = proposta.Turmas.FirstOrDefault().Nome,
                Inscricao  = new Dominio.Entidades.Inscricao()
                {
                    PropostaTurmaId = 1,
                    UsuarioId = 1,
                    FuncaoId = 1
                }
            });

            var registro1 = ImportacaoArquivoRegistroMock.GerarImportacaoArquivo(
                                                    arquivosValidado.FirstOrDefault().Id,
                                                    conteudo1,
                                                    string.Empty,
                                                    1,
                                                    SituacaoImportacaoArquivoRegistro.Validado, 1);
            await InserirNaBase(registro1);

            // act
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoProcessarArquivoDeImportacaoInscricao>();
            
            await casoDeUso.Executar(new MensagemRabbit(arquivosValidado.FirstOrDefault().Id));

            // assert
            var arquivos = ObterTodos<Dominio.Entidades.ImportacaoArquivo>();
            arquivos.ShouldNotBeNull();
            arquivos.Find(a => a.Id == arquivosValidado.FirstOrDefault().Id).Situacao.ShouldBe(SituacaoImportacaoArquivo.Processado);

            var registros = ObterTodos<Dominio.Entidades.ImportacaoArquivoRegistro>();
            registros.ShouldNotBeNull();
            registros.Count(r => r.Situacao == SituacaoImportacaoArquivoRegistro.Processado).ShouldBe(1);

            var inscricao = ObterTodos<Dominio.Entidades.Inscricao>();
            inscricao.ShouldNotBeNull();
            inscricao.Count.ShouldBe(1);
            inscricao.Exists(i => i.UsuarioId == usuario.Id).ShouldBeTrue();
        }
    }
}
