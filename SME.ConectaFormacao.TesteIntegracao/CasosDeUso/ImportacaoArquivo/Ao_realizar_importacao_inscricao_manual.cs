using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.ImportacaoArquivo
{
    public class Ao_realizar_importacao_inscricao_manual : TestePropostaBase
    {
        public Ao_realizar_importacao_inscricao_manual(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CursistaCargoServicoEol>>), typeof(ObterCargosFuncoesDresFuncionarioServicoEolQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Importação de Inscrição Cursista - Deve distribuir os processamentos em filas")]
        public async Task Deve_distribuir_processamento_em_filas()
        {
            // arrange
            var parametro = ParametroSistemaMock.GerarParametroSistema(Dominio.Enumerados.TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista, "1000");
            await InserirNaBase(parametro);
            
            var usuario = UsuarioMock.GerarUsuarioFaker().Generate(5);
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario.FirstOrDefault();
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var importacaoArquivo = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial);
            await InserirNaBase(importacaoArquivo);
            
            var itensImportacao = ImportacaoArquivoRegistroMock.GerarImportacaoArquivoCarregamentoInicial(1,5);

            var usuarios = ObterTodos<Dominio.Entidades.Usuario>();
            var propostaTurmas = ObterTodos<Dominio.Entidades.PropostaTurma>();
            
            var count = 0;
            foreach (var item in itensImportacao)
            {
                item.Conteudo = (new InscricaoCursistaDTO()
                {
                    Turma = propostaTurmas.FirstOrDefault().Nome,
                    ColaboradorRede = "1",
                    RegistroFuncional = usuarios[count].Login,
                    Cpf = usuarios[count].Cpf,
                    Nome = usuarios[count].Nome
                }).ObjetoParaJson();
                count++;
                await InserirNaBase(item);
            }
           
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoImportacaoInscricaoCursistaValidar>();
            
            var mapper = ObterCasoDeUso<IMapper>();

            var importacaoArquivos = ObterTodos<Dominio.Entidades.ImportacaoArquivo>();

            var importacaoArquivoDto = mapper.Map<ImportacaoArquivoDTO>(importacaoArquivos.FirstOrDefault());
            
            // act
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(importacaoArquivoDto.ObjetoParaJson()));
            
            // assert
            retorno.ShouldBeTrue();
            
            importacaoArquivos = ObterTodos<Dominio.Entidades.ImportacaoArquivo>();
            importacaoArquivos.FirstOrDefault().Situacao.ShouldBe(SituacaoImportacaoArquivo.Validado);
        }
        
        [Fact(DisplayName = "Importação de Inscrição Cursista - Deve realizar inscrição manual com sucesso")]
        public async Task Deve_realizar_importacao_inscricao_manual_com_sucesso()
        {
            // arrange
            var parametro = ParametroSistemaMock.GerarParametroSistema(Dominio.Enumerados.TipoParametroSistema.QtdeRegistrosImportacaoArquivoInscricaoCursista, "1000");
            await InserirNaBase(parametro);
            
            var usuario = UsuarioMock.GerarUsuarioFaker().Generate(5);
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario.FirstOrDefault();
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var importacaoArquivo = ImportacaoArquivoMock.GerarImportacaoArquivo(proposta.Id, SituacaoImportacaoArquivo.CarregamentoInicial);
            await InserirNaBase(importacaoArquivo);
            
            var itensImportacao = ImportacaoArquivoRegistroMock.GerarImportacaoArquivoCarregamentoInicial(1,5);

            var usuarios = ObterTodos<Dominio.Entidades.Usuario>();
            var propostaTurmas = ObterTodos<Dominio.Entidades.PropostaTurma>();
            
            var count = 0;
            foreach (var item in itensImportacao)
            {
                item.Conteudo = (new InscricaoCursistaDTO()
                {
                    Turma = propostaTurmas.FirstOrDefault().Nome,
                    ColaboradorRede = "1",
                    RegistroFuncional = usuarios[count].Login,
                    Cpf = usuarios[count].Cpf,
                    Nome = usuarios[count].Nome
                }).ObjetoParaJson();
                count++;
                await InserirNaBase(item);
            }
           
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoImportacaoInscricaoCursistaValidarItem>();
            
            var mapper = ObterCasoDeUso<IMapper>();

            var importacaoArquivosRegistros = ObterTodos<Dominio.Entidades.ImportacaoArquivoRegistro>();

            var importacaoArquivoRegistroDto = mapper.Map<ImportacaoArquivoRegistroDTO>(importacaoArquivosRegistros.FirstOrDefault());
            
            // act
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(importacaoArquivoRegistroDto.ObjetoParaJson()));
            
            // assert
            retorno.ShouldBeTrue();
            
            importacaoArquivosRegistros = ObterTodos<Dominio.Entidades.ImportacaoArquivoRegistro>();
            importacaoArquivosRegistros.FirstOrDefault().Situacao.ShouldBe(SituacaoImportacaoArquivoRegistro.Validado);
        }
    }
}
