using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.AreaPromotora;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora
{
    public class Ao_Remover_area_promotora : TesteBase
    {
        public Ao_Remover_area_promotora(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Área promotora - Deve remover área promotora válida")]
        public async Task Deve_remover_area_promotora_valida()
        {
            // arrange 
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora();
            await InserirNaBase(areaPromotora);

            var areaPromotoraTelefone = AreaPromotoraMock.GerarAreaTelefone(areaPromotora.Id);
            await InserirNaBase(areaPromotoraTelefone);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverAreaPromotora>();

            // act
            await casoDeUso.Executar(areaPromotora.Id);

            // assert
            var areaPromotoraRetorno = ObterPorId<Dominio.Entidades.AreaPromotora, long>(areaPromotora.Id);
            areaPromotoraRetorno.ShouldNotBeNull();

            areaPromotoraRetorno.Excluido.ShouldBeTrue();

            var telefones = ObterTodos<AreaPromotoraTelefone>();
            telefones.ShouldNotBeNull();

            telefones.FirstOrDefault().Excluido.ShouldBeTrue();
        }

        [Fact(DisplayName = "Área promotora - Deve retornar exceções area promotora não encontrada")]
        public async Task Deve_retornar_excecoes_area_promotora_nao_encontrada()
        {
            // arrange 
            var id = AreaPromotoraSalvarMock.GerarIdAleatorio();
            var areaPromotoraDTO = AreaPromotoraSalvarMock.GerarAreaPromotoraDTOInvalido();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverAreaPromotora>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(id));

            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA).ShouldBeTrue();
        }
    }
}
