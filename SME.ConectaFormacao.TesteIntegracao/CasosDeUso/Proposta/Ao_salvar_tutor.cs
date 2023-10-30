using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_tutor : TestePropostaBase
    {
        public Ao_salvar_tutor(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, ParametroSistema>), typeof(ObterParametroSistemaPorTipoEAnoQueryFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }
        [Fact(DisplayName = "Proposta - Deve Cadastrar um novo Tutor")]
        public async Task Deve_inserir_regente()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);

            var propostaId = await CriarProposta();
            var id = await useCase.Executar(propostaId,tutorDto);
            id.ShouldBeGreaterThan(0);
            ValidarPropostaTutorDTO(tutorDto, id);
        }
        
        [Fact(DisplayName = "Proposta - Deve Atualizar um Tutor Existente")]
        public async Task Deve_atualizar_regente_existente()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);

            var propostaId = await CriarProposta();
            var id = await useCase.Executar(propostaId,tutorDto);
            
            var tutorExistente = ObterPorId<PropostaTutor, long>(id);
            tutorExistente.Turmas = PropostaSalvarMock.GeraPropostaTutorTurmaValida(id,tutorDto.Turmas);
            tutorExistente.NomeTutor.ShouldBe(tutorDto.NomeTutor);
            tutorExistente.NomeTutor = new Person().FullName;
            var mapperParaDto = PropostaSalvarMock.GeraPropostaTutorDTOValida(tutorExistente);
            await useCase.Executar(propostaId,mapperParaDto);
            
            var propostaTutorExistenteModificado = ObterPorId<PropostaTutor, long>(id);
            propostaTutorExistenteModificado.NomeTutor.ShouldBe(tutorExistente.NomeTutor);
        }
        
        [Fact(DisplayName = "Proposta - Não Deve criar um Tutor sem Informar Turmas")]
        public async Task Nao_deve_criar_tutor_sem_informar_turmas()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);

            var propostaId = await CriarProposta();
            tutorDto.Turmas = new List<PropostaTutorTurmaDTO>();
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(propostaId,tutorDto));
            excecao.Mensagens.Contains("É necessário informar uma Turma para para cadastrar um tutor").ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Não Deve criar um Tutor sem Informar o nome do Tutor")]
        public async Task Nao_deve_criar_tutor_sem_informar_o_nome_do_tutor()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);

            var propostaId = await CriarProposta();
            tutorDto.NomeTutor = "";
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(propostaId,tutorDto));
            excecao.Mensagens.Contains("Informe o nome do Tutor").ShouldBeTrue();
        }
        private async Task<long> CriarProposta()
        {
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoProposta.Rascunho);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            var id = await casoDeUso.Executar(propostaDTO);

            id.ShouldBeGreaterThan(0);
            return id;
        }
    }
}