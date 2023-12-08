using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.ServicoFake;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System.Text.Json;
using AutoMapper;
using Bogus;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma.Mock;
using SME.ConectaFormacao.TesteIntegracao.ServicosFakes;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.EOL.ComponenteCurricularAnoTurma
{
    public class Ao_sincronizar_componente_curricular_ano_turma : TesteBase
    {
        public Ao_sincronizar_componente_curricular_ano_turma(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterComponentesCurricularesEAnosTurmaEOLQuery, IEnumerable<ComponenteCurricularAnoTurmaEOLDTO>>), typeof(ObterComponentesCurricularesEAnoTurmaEOLQueryFake), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<PublicarNaFilaRabbitCommand, bool>), typeof(PublicarNaFilaRabbitCommandFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Componente Curricular e Ano da Turma - Deve inserir componentes curriculares e anos da turma novos - carga inicial")]
        public async Task Deve_inserir_componentes_ano_turma_carga_inicial()
        {
            // arrange
            var componenteCurricularAnoTurmaMock = ComponenteCurricularAnoTurmaMock.GerarLista();

            var casoDeUso = ObterCasoDeUso<IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase>();

            // act 
            var mensagem = JsonSerializer.Serialize(new AnoLetivoDTO(DateTimeExtension.HorarioBrasilia().Year));
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));

            // assert
            retorno.ShouldBeTrue();
            ValidacaoAnoTurmaComponente(componenteCurricularAnoTurmaMock);
        }

       [Fact(DisplayName = "Componente Curricular e Ano da Turma - Deve atualizar nome do componente curricular e a descrição do ano da turma")]
        public async Task Deve_alterar_nome_do_componentes_e_descricao_do_ano_turma()
        {
            // arrange
            var mapper = ObterCasoDeUso<IMapper>();
            var faker = new Faker();
            var componenteCurricularAnoTurmaMock = ComponenteCurricularAnoTurmaMock.GerarLista();
            var anoTurmaId = 1;
            
            foreach (var componenteAnoTurmaEol in componenteCurricularAnoTurmaMock)
            {
                var anoTurma = mapper.Map<Dominio.Entidades.AnoTurma>(componenteAnoTurmaEol);
                anoTurma.AnoLetivo = DateTimeExtension.HorarioBrasilia().Year;
                GerarAuditoria(anoTurma);
                await InserirNaBase(anoTurma);
                
                var componente = mapper.Map<Dominio.Entidades.ComponenteCurricular>(componenteAnoTurmaEol);
                componente.AnoTurmaId = anoTurmaId;
                GerarAuditoria(componente);
                await InserirNaBase(componente);

                anoTurmaId++;
            }

            var anos = ObterTodos<Dominio.Entidades.AnoTurma>();
            var componentesAnosTurms = ObterTodos<Dominio.Entidades.ComponenteCurricular>();

            foreach (var componentesEAnoTurma in componenteCurricularAnoTurmaMock)
            {
                //Modificando o nome do componente - a descrição (eol) do componente_curricular (nome)
                componentesEAnoTurma.DescricaoComponenteCurricular = faker.Lorem.Text().Limite(70);
                
                //Modificando a série ensino (eol) do ano_turma (descrição) 
                componentesEAnoTurma.DescricaoSerieEnsino = $"{faker.Random.Int(min: 1, max: 9)}º {faker.Lorem.Text().Limite(15)}";
            }
            
            var casoDeUso = ObterCasoDeUso<IExecutarSincronizacaoComponentesCurricularesEAnosTurmaEOLUseCase>();

            // act 
            var mensagem = JsonSerializer.Serialize(new AnoLetivoDTO(DateTimeExtension.HorarioBrasilia().Year));
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));

            // assert
            retorno.ShouldBeTrue();
            ValidacaoAnoTurmaComponente(componenteCurricularAnoTurmaMock);
        }
       
        private void ValidacaoAnoTurmaComponente(IEnumerable<ComponenteCurricularAnoTurmaEOLDTO> componenteCurricularAnoTurmaMock)
        {
            var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
            var anosTurma = ObterTodos<Dominio.Entidades.AnoTurma>();
            var componentesAnoTurma = ObterTodos<Dominio.Entidades.ComponenteCurricular>();

            anosTurma.ShouldNotBeNull();
            anosTurma.Count.ShouldBe(componenteCurricularAnoTurmaMock.Count());
            componentesAnoTurma.ShouldNotBeNull();
            componentesAnoTurma.Count.ShouldBe(componenteCurricularAnoTurmaMock.Count());

            foreach (var ano in anosTurma)
            {
                componenteCurricularAnoTurmaMock.Any(a => a.CodigoAnoTurma.Equals(ano.CodigoEOL)).ShouldBeTrue();
                componenteCurricularAnoTurmaMock.Any(a => a.DescricaoSerieEnsino.Equals(ano.Descricao)).ShouldBeTrue();
                componenteCurricularAnoTurmaMock.Any(a => a.CodigoSerieEnsino.Equals(ano.CodigoSerieEnsino)).ShouldBeTrue();
                componenteCurricularAnoTurmaMock.Any(a => a.Modalidade.Equals(ano.Modalidade)).ShouldBeTrue();
                ano.AnoLetivo.ShouldBe(anoAtual);
            }

            foreach (var componenteAnoTurma in componentesAnoTurma)
            {
                componenteCurricularAnoTurmaMock.Any(a => a.CodigoComponenteCurricular.Equals(componenteAnoTurma.CodigoEOL)).ShouldBeTrue();
                componenteCurricularAnoTurmaMock.Any(a => a.DescricaoComponenteCurricular.Equals(componenteAnoTurma.Nome)).ShouldBeTrue();
            }
        }

        private void GerarAuditoria<T>(T entidade) where T: EntidadeBaseAuditavel
        {
            entidade.CriadoEm = DateTimeExtension.HorarioBrasilia();
            entidade.CriadoPor = "Sistema";
            entidade.CriadoLogin = "Sistema";
        }
    }
}
