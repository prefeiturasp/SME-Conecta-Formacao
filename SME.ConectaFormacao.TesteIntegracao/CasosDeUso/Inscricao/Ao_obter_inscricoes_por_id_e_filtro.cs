using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_obter_inscricoes_por_id_e_filtro : TestePropostaBase
    {
        private FiltroListagemInscricaoDTO Filtro = new();
        private long InscricaoId = 0;
        public Ao_obter_inscricoes_por_id_e_filtro(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Inscrição - Deve obter inscrição Informando Somente o Id")]
        public async Task Deve_ober_dados_somente_com_id()
        {
            // arrange
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoPorId>();
            var dtoFiltro = new FiltroListagemInscricaoDTO();

            // act
            var retorno = await casoDeUso.Executar(InscricaoId, dtoFiltro);

            // assert 

            retorno.Items.ShouldNotBeEmpty();
        }
        [Fact(DisplayName = "Inscrição - Deve obter inscrição Informando Somente o Id e CPF")]
        public async Task Deve_ober_dados_somente_com_id_cpf()
        {
            // arrange
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoPorId>();
            var dtoFiltro = new FiltroListagemInscricaoDTO() { Cpf = Filtro.Cpf };

            // act
            var retorno = await casoDeUso.Executar(InscricaoId, dtoFiltro);

            // assert 

            retorno.Items.ShouldNotBeEmpty();
        }

        [Fact(DisplayName = "Inscrição - Deve obter inscrição Informando Somente o Id e RF")]
        public async Task Deve_ober_dados_somente_com_id_rf()
        {
            // arrange
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoPorId>();
            var dtoFiltro = new FiltroListagemInscricaoDTO() { RegistroFuncional = Filtro.RegistroFuncional };

            // act
            var retorno = await casoDeUso.Executar(InscricaoId, dtoFiltro);

            // assert 

            retorno.Items.ShouldNotBeEmpty();
        }
        [Fact(DisplayName = "Inscrição - Deve obter inscrição Informando Somente o Id e Nome")]
        public async Task Deve_ober_dados_somente_com_id_nome()
        {
            // arrange
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoPorId>();
            var dtoFiltro = new FiltroListagemInscricaoDTO() { NomeCursista = Filtro.NomeCursista };

            // act
            var retorno = await casoDeUso.Executar(InscricaoId, dtoFiltro);

            // assert 

            retorno.Items.ShouldNotBeEmpty();
        }
        [Fact(DisplayName = "Inscrição - Deve obter inscrição Informando Somente o Id e Todos Parametros")]
        public async Task Deve_ober_dados_somente_com_id_todos_parametros()
        {
            // arrange
            await DadosBasico();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoPorId>();
            var dtoFiltro = new FiltroListagemInscricaoDTO() { NomeCursista = Filtro.NomeCursista, Cpf = Filtro.Cpf, RegistroFuncional = Filtro.RegistroFuncional };

            // act
            var retorno = await casoDeUso.Executar(InscricaoId, dtoFiltro);

            // assert 

            retorno.Items.ShouldNotBeEmpty();
        }
        [Fact(DisplayName = "Inscrição - Não Deve obter inscrição Informando Id que não existe")]
        public async Task Nao_Deve_ober_dados_informando_id_que_nao_existe()
        {
            // arrange
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterInscricaoPorId>();
            var dtoFiltro = new FiltroListagemInscricaoDTO();

            // act
            var excecao = await casoDeUso.Executar(InscricaoId, dtoFiltro);

            // assert 
            excecao.Items.Count().ShouldBeEquivalentTo(0);
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

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

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
    }
}
