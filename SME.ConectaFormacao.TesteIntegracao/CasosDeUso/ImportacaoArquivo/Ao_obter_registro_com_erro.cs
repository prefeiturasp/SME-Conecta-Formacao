using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
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
    public class Ao_obter_registro_com_erro : TestePropostaBase
    {
        public Ao_obter_registro_com_erro(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Importação Arquivo - Deve obter registro com erro paginado")]
        public async Task Ao_obter_registro_com_erro_paginado()
        {
            // arrange
            AdicionarContextoAplicacaoPaginada("1", "2");

            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(SituacaoProposta.Publicada, FormacaoHomologada.NaoCursosPorIN);

            var arquivosValidado = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.Validando, 2);
            await InserirNaBase(arquivosValidado);

            var conteudoErro1 = JsonConvert.SerializeObject(new InscricaoCursistaDTO()
            {
                ColaboradorRede = "Não",
                Cpf = "99910000000",
                RegistroFuncional = "22222",
                Turma = "Turma 1",
                Nome = "Erro 1"
            }); 

            var registroErro1 = ImportacaoArquivoRegistroMock.GerarImportacaoArquivo(
                                                    arquivosValidado.FirstOrDefault().Id,
                                                    conteudoErro1,
                                                    "Turma invalida",
                                                    1,
                                                    SituacaoImportacaoArquivoRegistro.Erro, 1);
            await InserirNaBase(registroErro1);

            var conteudoErro2 = JsonConvert.SerializeObject(new InscricaoCursistaDTO()
            {
                ColaboradorRede = "Sim",
                Cpf = "90000000000",
                RegistroFuncional = "111111",
                Turma = "Turma 2",
                Nome = "Erro 2"
            });

            var registroErro2 = ImportacaoArquivoRegistroMock.GerarImportacaoArquivo(
                                        arquivosValidado.FirstOrDefault().Id,
                                        conteudoErro2,
                                        "Colaborador já cadastrado",
                                        2,
                                        SituacaoImportacaoArquivoRegistro.Erro, 1);
            await InserirNaBase(registroErro2);

            var conteudoErro3 = JsonConvert.SerializeObject(new InscricaoCursistaDTO()
            {
                ColaboradorRede = "Sim",
                Cpf = "90000000001",
                RegistroFuncional = "333333",
                Turma = "Turma 3",
                Nome = "Erro 3"
            });

            var registroErro3 = ImportacaoArquivoRegistroMock.GerarImportacaoArquivo(
                                        arquivosValidado.FirstOrDefault().Id,
                                        conteudoErro3,
                                        "Colaborador não encontrado",
                                        3,
                                        SituacaoImportacaoArquivoRegistro.Erro, 1);
            await InserirNaBase(registroErro3);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterRegistrosDaIncricaoInconsistentes>();

            var retorno = await casoDeUso.Executar(arquivosValidado.FirstOrDefault().Id);

            // assert
            retorno.ShouldNotBeNull();
            retorno.TotalRegistros.ShouldBe(3);
            retorno.TotalPaginas.ShouldBe(2);
            retorno.Items.Count().ShouldBe(2);
            var registro1 = retorno.Items.ToList().Find(r => r.Linha == 1);
            registro1.ShouldNotBeNull();
            registro1.ColaboradorRede.ShouldBe("Não");
            registro1.Turma.ShouldBe("Turma 1");
            registro1.Erro.ShouldBe("Turma invalida");
            var registro2 = retorno.Items.ToList().Find(r => r.Linha == 2);
            registro2.ShouldNotBeNull();
            registro2.ColaboradorRede.ShouldBe("Sim");
            registro2.Turma.ShouldBe("Turma 2");
            registro2.Erro.ShouldBe("Colaborador já cadastrado");
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
