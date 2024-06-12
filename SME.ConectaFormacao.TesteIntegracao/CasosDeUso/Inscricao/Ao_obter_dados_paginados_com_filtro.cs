using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_obter_dados_paginados_com_filtro : TestePropostaBase
    {
        private FiltroListagemInscricaoDTO Filtro = new();
        private long InscricaoId = 0;
        public Ao_obter_dados_paginados_com_filtro(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }
        [Fact(DisplayName = "Inscrição - Deve Obter Dados Paginados")]
        public async Task Deve_ober_dados_listagem_paginada()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao();
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterDadosPaginadosComFiltros>();
            var dtoFiltro = new FiltroListagemInscricaoComTurmaDTO();

            // act
            var retorno = await casoDeUso.Executar(dtoFiltro);

            // assert 

            retorno.Items.ShouldNotBeEmpty();
        }

        [Fact(DisplayName = "Inscrição - Deve Obter Dados Paginados por número de homologação")]
        public async Task Deve_ober_dados_listagem_paginada_por_numero_homologacao()
        {
            // arrange
            AdicionarPerfilUsuarioContextoAplicacao();
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterDadosPaginadosComFiltros>();
            var dtoFiltro = new FiltroListagemInscricaoComTurmaDTO();

            var proposta = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();

            dtoFiltro.NumeroHomologacao = proposta.NumeroHomologacao;

            // act
            var retorno = await casoDeUso.Executar(dtoFiltro);

            // assert 
            retorno.Items.ShouldNotBeEmpty();
            retorno.Items.Count().ShouldBe(1);
        }

        private async Task DadosBasico()
        {
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var CargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(CargosFuncoes);

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;

            var proposta = await InserirNaBaseProposta(
                SituacaoProposta.Publicada,
                FormacaoHomologada.NaoCursosPorIN,
                TipoInscricao.Automatica, false, false, true, false, true);

            var propostaTurma = proposta.Turmas.FirstOrDefault();

            var inscricao = InscricaoMock.GerarInscricao(usuario.Id, propostaTurma!.Id);
            inscricao.CargoId = CargosFuncoes.FirstOrDefault()!.Id;
            inscricao.FuncaoId = CargosFuncoes.FirstOrDefault()!.Id; ;
            await InserirNaBase(inscricao);

            var vaga = PropostaMock.GerarTurmaVaga(propostaTurma.Id, inscricao.Id);
            await InserirNaBase(vaga);

            InscricaoId = inscricao.Id;
            Filtro.Cpf = usuario.Cpf;
            Filtro.NomeCursista = usuario.Nome;
            Filtro.RegistroFuncional = usuario.Login;
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
    }
}