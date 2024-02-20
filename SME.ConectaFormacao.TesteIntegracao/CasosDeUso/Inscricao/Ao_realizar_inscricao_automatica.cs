using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Dominio.ObjetosDeValor;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_realizar_inscricao_automatica : TestePropostaBase
    {
        public Ao_realizar_inscricao_automatica(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterFuncionarioPorFiltroPropostaServicoEolQuery, IEnumerable<CursistaServicoEol>>), typeof(ObterFuncionarioPorFiltroPropostaServicoEolQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Inscrição - Deve realizar inscrição automática com sucesso")]
        public async Task Deve_realizar_inscricao_automatica_com_sucesso()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var parametro = ParametroSistemaMock.GerarParametroSistema(Dominio.Enumerados.TipoParametroSistema.QtdeCursistasSuportadosPorTurma, "5");
            await InserirNaBase(parametro);

            AoRealizarInscricaoAutomaticaMock.CargosFuncoes = CargosFuncoes;
            AoRealizarInscricaoAutomaticaMock.CargosFuncoesDeparaEol = depara;

            var mensagem = proposta.Id.ToString();
            var mensagemRabbit = new Infra.MensagemRabbit(mensagem);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRealizarInscricaoAutomatica>();

            // act
            await casoDeUso.Executar(mensagemRabbit);
        }

        [Fact(DisplayName = "Inscrição - Deve realizar inscrição automática tratar turma sem dre com sucesso")]
        public async Task Deve_realizar_inscricao_automatica_tratar_turmas_sem_dre_com_sucesso()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var dres = ObterTodos<Dominio.Entidades.Dre>();

            AoRealizarInscricaoAutomaticaMock.CargosFuncoes = CargosFuncoes;
            AoRealizarInscricaoAutomaticaMock.CargosFuncoesDeparaEol = depara;

            var tratarTurma = new InscricaoAutomaticaTratarTurmasDTO
            {
                PropostaInscricaoAutomatica = new Dominio.ObjetosDeValor.PropostaInscricaoAutomatica
                {
                    PropostaId = proposta.Id,
                    PropostasTurmas = proposta.Turmas.Select(t => new Dominio.ObjetosDeValor.PropostaInscricaoAutomaticaTurma
                    {
                        Id = t.Id
                    }),

                },
                CursistasEOL = AoRealizarInscricaoAutomaticaMock.ObterCursistasEol(500, dres)
            };

            var mensagem = tratarTurma.ObjetoParaJson();
            var mensagemRabbit = new Infra.MensagemRabbit(mensagem);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRealizarInscricaoAutomaticaTratarTurmas>();

            // act
            await casoDeUso.Executar(mensagemRabbit);
        }

        [Fact(DisplayName = "Inscrição - Deve realizar inscrição automática tratar turma com dre com sucesso")]
        public async Task Deve_realizar_inscricao_automatica_tratar_turmas_com_dre_com_sucesso()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var dres = ObterTodos<Dominio.Entidades.Dre>();

            AoRealizarInscricaoAutomaticaMock.CargosFuncoes = CargosFuncoes;
            AoRealizarInscricaoAutomaticaMock.CargosFuncoesDeparaEol = depara;

            var turmasInscricaoAutomatica = new List<PropostaInscricaoAutomaticaTurma>();
            foreach (var turma in proposta.Turmas)
            {
                foreach (var dre in turma.Dres)
                    turmasInscricaoAutomatica.Add(new PropostaInscricaoAutomaticaTurma
                    {
                        Id = dre.PropostaTurmaId,
                        DreId = dre.DreId,
                        CodigoDre = dres.FirstOrDefault(t => t.Id == dre.DreId).Codigo,
                    });
            }

            var tratarTurma = new InscricaoAutomaticaTratarTurmasDTO
            {
                PropostaInscricaoAutomatica = new Dominio.ObjetosDeValor.PropostaInscricaoAutomatica
                {
                    PropostaId = proposta.Id,
                    PropostasTurmas = turmasInscricaoAutomatica
                },
                CursistasEOL = AoRealizarInscricaoAutomaticaMock.ObterCursistasEol(4955, dres)
            };

            var mensagem = tratarTurma.ObjetoParaJson();
            var mensagemRabbit = new Infra.MensagemRabbit(mensagem);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRealizarInscricaoAutomaticaTratarTurmas>();

            // act
            await casoDeUso.Executar(mensagemRabbit);
        }
    }
}
