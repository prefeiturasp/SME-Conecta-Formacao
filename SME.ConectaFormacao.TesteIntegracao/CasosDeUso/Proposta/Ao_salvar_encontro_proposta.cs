using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_salvar_encontro_proposta : TestePropostaBase
    {
        public Ao_salvar_encontro_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta Encontro - Deve inserir encontro da proposta válido")]
        public async Task Deve_inserir_encontro_proposta_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var encontroDTO = PropostaSalvarMock.GerarEncontro(proposta.QuantidadeTurmas.GetValueOrDefault());

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarPropostaEncontro>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, encontroDTO);

            // assert
            ValidarPropostaEncontro(encontroDTO, proposta.Id);
        }

        [Fact(DisplayName = "Proposta Encontro - Deve alterar encontros da proposta válido")]
        public async Task Deve_alterar_encontros_proposta_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var encontroDTO = PropostaSalvarMock.GerarEncontro(proposta.QuantidadeTurmas.GetValueOrDefault());
            encontroDTO.Id = proposta.Encontros.FirstOrDefault().Id;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarPropostaEncontro>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, encontroDTO);

            // assert
            ValidarPropostaEncontro(encontroDTO, proposta.Id);
        }

        protected void ValidarPropostaEncontro(PropostaEncontroDTO encontroDTO, long id)
        {
            var encontros = ObterTodos<PropostaEncontro>();
            var turmas = ObterTodos<PropostaEncontroTurma>();
            var datas = ObterTodos<PropostaEncontroData>();

            var encontro = encontros.FirstOrDefault(t =>
                t.PropostaId == id &&
                t.HoraInicio == encontroDTO.HoraInicio &&
                t.HoraFim == encontroDTO.HoraFim &&
                t.Local == encontroDTO.Local
                );
            encontro.ShouldNotBeNull();

            foreach (var dataDTO in encontroDTO.Datas)
            {
                var data = datas.FirstOrDefault(t =>
                    t.PropostaEncontroId == encontro.Id &&
                    t.DataInicio.GetValueOrDefault().Date == dataDTO.DataInicio.Date &&
                    t.DataFim.GetValueOrDefault().Date == dataDTO.DataFim.GetValueOrDefault().Date);
                data.ShouldNotBeNull();
            }
        }
    }
}
