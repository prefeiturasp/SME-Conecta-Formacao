using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Propostas
{
    public class Ao_Salvar_Encontro_Proposta(CollectionFixture collectionFixture) : 
        TestePropostaBase(collectionFixture)
    {
        [Fact(DisplayName = "Proposta Encontro - Deve inserir encontro da proposta válido")]
        public async Task Deve_inserir_encontro_proposta_valido()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var encontroDTO = PropostaSalvarMock.GerarEncontro(proposta.QuantidadeTurmas.GetValueOrDefault());

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarPropostaEncontro>();

            // act
            await casoDeUso.Executar(proposta.Id, encontroDTO);

            // assert
            ValidarPropostaEncontro(encontroDTO, proposta.Id);
        }

        [Fact(DisplayName = "Proposta Encontro - Deve alterar encontros da proposta válido")]
        public async Task Deve_alterar_encontros_proposta_valido()
        {
            // arrange            
            var proposta = await InserirNaBaseProposta();

            var encontroDTO = PropostaSalvarMock.GerarEncontro(proposta.QuantidadeTurmas.GetValueOrDefault());
            encontroDTO.Id = proposta.Encontros.First().Id;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarPropostaEncontro>();

            // act
            await casoDeUso.Executar(proposta.Id, encontroDTO);

            // assert
            ValidarPropostaEncontro(encontroDTO, proposta.Id);
        }

        protected void ValidarPropostaEncontro(PropostaEncontroDto encontroDTO, long id)
        {
            var encontros = ObterTodos<PropostaEncontro>();
            var datas = ObterTodos<PropostaEncontroData>();

            var encontro = encontros.FirstOrDefault(t =>
                t.PropostaId == id &&
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
