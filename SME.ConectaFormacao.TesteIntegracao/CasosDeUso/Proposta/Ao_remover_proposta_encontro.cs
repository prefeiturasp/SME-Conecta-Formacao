using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_remover_proposta_encontro : TestePropostaBase
    {
        public Ao_remover_proposta_encontro(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta Encontro - deve remover encontro da proposta")]
        public async Task Deve_remover_encontro_proposta_valida()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoRemoverPropostaEncontro>();

            var id = proposta.Encontros.FirstOrDefault().Id;

            // act
            var retorno = await casoDeUso.Executar(id);

            // assert
            var encontro = ObterPorId<PropostaEncontro, long>(id);
            encontro.Excluido.ShouldBeTrue();

            var datas = ObterTodos<PropostaEncontroData>();
            datas.Any(t => t.PropostaEncontroId == id && !t.Excluido).ShouldBeFalse();

            var turmas = ObterTodos<PropostaEncontroTurma>();
            turmas.Any(t => t.PropostaEncontroId == id && !t.Excluido).ShouldBeFalse();
        }
    }
}
