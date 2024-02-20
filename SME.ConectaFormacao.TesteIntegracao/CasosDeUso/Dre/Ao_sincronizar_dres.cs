using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre.ServicoFake;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System.Text.Json;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Dre
{
    public class Ao_sincronizar_dres : TesteBase
    {
        public Ao_sincronizar_dres(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterCodigosDresEOLQuery, IEnumerable<DreServicoEol>>), typeof(ObterCodigosDresQueryFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Area Promotora - Deve Inserir uma que não existe")]
        public async Task Deve_inserir_uma_Dre()
        {
            var dre = DreMock.GerarDreValida();
            await InserirNaBase(dre!.FirstOrDefault()!);
            var todosAreasAntes = ObterTodos<Dominio.Entidades.Dre>();
            todosAreasAntes.Count.ShouldBeEquivalentTo(1);

            var codigoDre = "67";
            var casoDeUso = ObterCasoDeUso<IExecutarSincronizacaoInstitucionalDreTratarUseCase>();
            var mensagem = JsonSerializer.Serialize(new DreServicoEol(codigoDre, "Nome da Dre", "abr Dre"));
            await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));

            var todosAreasDepois = ObterTodos<Dominio.Entidades.Dre>();
            todosAreasDepois.Count.ShouldBeEquivalentTo(2);
            todosAreasDepois.Count(x => x.Codigo == codigoDre).ShouldBeEquivalentTo(1);

        }

        [Fact(DisplayName = "Area Promotora - Deve Sincronizar e  retornar Sucesso")]
        public async Task Deve_sincronizar_e_retornar_sucesso()
        {
            var casoDeUso = ObterCasoDeUso<IExecutarSincronizacaoInstitucionalDreSyncUseCase>();
            var retorno = await casoDeUso.Executar(new Infra.MensagemRabbit());
            retorno.ShouldBeTrue();
        }

        [Fact(DisplayName = "Area Promotora - Deve atualizar uma dre")]
        public async Task Deve_atualizar_uma_Dre()
        {
            var dre = DreMock.GerarDreValida();
            await InserirNaBase(dre!.FirstOrDefault()!);
            var todosAreasAntes = ObterTodos<Dominio.Entidades.Dre>();
            todosAreasAntes.Count.ShouldBeEquivalentTo(1);

            var nomeDre = "Nome da Dre atualizado";
            var abreviacao = "XYZ DRE";

            var casoDeUso = ObterCasoDeUso<IExecutarSincronizacaoInstitucionalDreTratarUseCase>();
            var mensagem = JsonSerializer.Serialize(new DreServicoEol(dre!.FirstOrDefault()!.Codigo, nomeDre, abreviacao));
            await casoDeUso.Executar(new Infra.MensagemRabbit(mensagem));


            var todosAreasDepois = ObterTodos<Dominio.Entidades.Dre>();
            todosAreasDepois.Count.ShouldBeEquivalentTo(1);
            todosAreasDepois.Count(x => x.Nome == nomeDre && x.Abreviacao == abreviacao).ShouldBeEquivalentTo(1);
        }
    }
}
