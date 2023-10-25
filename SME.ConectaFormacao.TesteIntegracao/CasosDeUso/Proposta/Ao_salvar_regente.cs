using AutoMapper;
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
    public class Ao_salvar_regente : TestePropostaBase
    {
        public Ao_salvar_regente(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterParametroSistemaPorTipoEAnoQuery, ParametroSistema>), typeof(ObterParametroSistemaPorTipoEAnoQueryFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }
        [Fact(DisplayName = "Proposta - Deve Cadastrar um novo Regente")]
        public async Task Deve_inserir_regente()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);

            var propostaId = await CriarProposta();
            var id = await useCase.Executar(propostaId,regenteDto);
            id.ShouldBeGreaterThan(0);
            ValidarPropostaRegenteDTO(regenteDto, id);
        }
        [Fact(DisplayName = "Proposta - Deve Atualizar um Regente Existente")]
        public async Task Deve_atualizar_regente_existente()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);

            var propostaId = await CriarProposta();
            var id = await useCase.Executar(propostaId,regenteDto);
            
            var propostaRegenteExistente = ObterPorId<PropostaRegente, long>(id);
            propostaRegenteExistente.Turmas = PropostaSalvarMock.GeraPropostaRegenteTurmaValida(id,regenteDto.Turmas);
            propostaRegenteExistente.NomeRegente.ShouldBe(regenteDto.NomeRegente);
            propostaRegenteExistente.NomeRegente = new Person().FullName;
            var mapperParaDto = PropostaSalvarMock.GeraPropostaRegenteDTOValida(propostaRegenteExistente);
            await useCase.Executar(propostaId,mapperParaDto);
            
            var propostaRegenteExistenteModificado = ObterPorId<PropostaRegente, long>(id);
            propostaRegenteExistenteModificado.NomeRegente.ShouldBe(propostaRegenteExistente.NomeRegente);
        }

        [Fact(DisplayName = "Proposta - Não Deve criar um Regente sem Informar Turmas")]
        public async Task Nao_deve_criar_regente_sem_informar_turmas()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);

            var propostaId = await CriarProposta();
            regenteDto.Turmas = new List<PropostaRegenteTurmaDTO>();
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(propostaId,regenteDto));
            excecao.Mensagens.Contains("É necessário informar uma Turma para para cadastrar um regente").ShouldBeTrue();
        }
        [Fact(DisplayName = "Proposta - Não Deve criar um Regente sem Informar o nome do Regente")]
        public async Task Nao_deve_criar_regente_sem_informar_o_nome_do_regente()
        {
            var useCase = ObterCasoDeUso<ICasoDeUsoSalvarPropostaRegente>();
            var regenteDto = PropostaSalvarMock.GerarRegente(3);

            var propostaId = await CriarProposta();
            regenteDto.NomeRegente = "";
            var excecao = await Should.ThrowAsync<NegocioException>(useCase.Executar(propostaId,regenteDto));
            excecao.Mensagens.Contains("Informe o nome do Regente").ShouldBeTrue();
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