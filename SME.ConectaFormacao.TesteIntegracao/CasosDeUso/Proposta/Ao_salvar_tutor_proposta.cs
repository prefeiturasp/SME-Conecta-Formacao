using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_tutor_proposta : TestePropostaBase
    {
        public Ao_salvar_tutor_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Não Deve Cadastrar um novo Tutor com Turma Duplicada")]
        public async Task Nao_deve_cadastrar_tutor_com_turma_duplicada()
        {
            // arrange
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var proposta = await InserirNaBaseProposta();
            var tutor = PropostaSalvarMock.GerarTutor(1);
            await useCase.Executar(proposta.Id, tutor);
            
            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(proposta.Id, tutor));
            
            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(string.Format(MensagemNegocio.JA_EXISTE_ESSA_TURMA_PARA_ESSE_TURTOR,tutor.NomeTutor,tutor.Turmas.FirstOrDefault().Turma)).ShouldBeTrue();
        }
        [Fact(DisplayName = "Proposta - Deve Cadastrar um novo Tutor")]
        public async Task Deve_inserir_tutor()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);

            // act
            var id = await useCase.Executar(proposta.Id, tutorDto);

            // assert
            id.ShouldBeGreaterThan(0);
            ValidarPropostaTutorDTO(tutorDto, id);
        }
        
        [Fact(DisplayName = "Proposta - Deve Atualizar um Tutor Existente")]
        public async Task Deve_atualizar_tutor_existente()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();
            var tutorExistente = proposta.Tutores.FirstOrDefault();

            var tutorDto = PropostaSalvarMock.GerarTutor(3);
            tutorDto.Id = tutorExistente.Id;

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();

            // act
            await useCase.Executar(proposta.Id, tutorDto);

            // assert 
            var propostaTutorExistenteModificado = ObterPorId<PropostaTutor, long>(tutorExistente.Id);
            propostaTutorExistenteModificado.NomeTutor.ShouldBe(tutorDto.NomeTutor.ToUpper());
        }
        
        [Fact(DisplayName = "Proposta - Não Deve criar um Tutor sem Informar Turmas")]
        public async Task Nao_deve_criar_tutor_sem_informar_turmas()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);
            tutorDto.Turmas = new List<PropostaTutorTurmaDTO>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(proposta.Id, tutorDto));

            // assert
            excecao.Mensagens.Contains("É necessário informar uma Turma para para cadastrar um tutor").ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Não Deve criar um Tutor sem Informar o nome do Tutor")]
        public async Task Nao_deve_criar_tutor_sem_informar_o_nome_do_tutor()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaTutor>();
            var tutorDto = PropostaSalvarMock.GerarTutor(3);
            tutorDto.NomeTutor = "";

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(proposta.Id, tutorDto));

            // assert
            excecao.Mensagens.Contains("Informe o nome do Tutor").ShouldBeTrue();
        }
    }
}