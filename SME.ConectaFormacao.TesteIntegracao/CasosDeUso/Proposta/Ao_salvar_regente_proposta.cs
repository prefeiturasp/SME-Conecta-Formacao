using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
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
    public class Ao_salvar_regente_proposta : TestePropostaBase
    {
        public Ao_salvar_regente_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve Cadastrar um novo Regente")]
        public async Task Deve_inserir_regente()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);

            // act
            var id = await useCase.Executar(proposta.Id, regenteDto);

            // assert
            id.ShouldBeGreaterThan(0);
            ValidarPropostaRegenteDTO(regenteDto, id);
        }
        [Fact(DisplayName = "Proposta - Não Deve Cadastrar um novo Regente com Turma Duplicada")]
        public async Task Nao_deve_cadastrar_regente_com_turma_duplicada()
        {
            // arrange
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var proposta = await InserirNaBaseProposta();
            var regente = PropostaSalvarMock.GerarRegente(1);
            await useCase.Executar(proposta.Id, regente);
            
            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(proposta.Id, regente));
            
            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(string.Format(MensagemNegocio.JA_EXISTE_ESSA_TURMA_PARA_ESSE_REGENTE,regente.NomeRegente,regente.Turmas.FirstOrDefault().Turma)).ShouldBeTrue();
        }
        [Fact(DisplayName = "Proposta - Deve Atualizar um Regente Existente")]
        public async Task Deve_atualizar_regente_existente()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();
            var regenteExistente = proposta.Regentes.FirstOrDefault();

            var regenteDTO = PropostaSalvarMock.GerarRegente(3);
            regenteDTO.Id = regenteExistente.Id;

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();

            // act
            await useCase.Executar(proposta.Id, regenteDTO);

            // assert 
            var propostaRegenteExistenteModificado = ObterPorId<PropostaRegente, long>(regenteExistente.Id);
            propostaRegenteExistenteModificado.NomeRegente.ShouldBe(regenteDTO.NomeRegente.ToUpper());
        }

        [Fact(DisplayName = "Proposta - Não Deve criar um Regente sem Informar Turmas")]
        public async Task Nao_deve_criar_regente_sem_informar_turmas()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);
            regenteDto.Turmas = new List<PropostaRegenteTurmaDTO>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(proposta.Id, regenteDto));

            // assert
            excecao.Mensagens.Contains("É necessário informar uma Turma para cadastrar um regente").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Não Deve criar um Regente sem Informar o nome do Regente")]
        public async Task Nao_deve_criar_regente_sem_informar_o_nome_do_regente()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);
            regenteDto.NomeRegente = string.Empty;

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(proposta.Id, regenteDto));

            // assert
            excecao.Mensagens.Contains("Informe o nome do Regente").ShouldBeTrue();
        }
    }
}