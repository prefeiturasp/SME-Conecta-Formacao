using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_sugestao_parecer_pareceristas : TestePropostaBase
    {
        public Ao_obter_sugestao_parecer_pareceristas(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - deve obter sugestões de parecer dos pareceristas aprovado")]
        public async Task Deve_obter_Sugestoes_Parecer_Pareceristas_aprovado()
        {
            // arrange 
            var proposta = await InserirNaBaseProposta(SituacaoProposta.AguardandoAnaliseParecerDF, quantidadeParecerista: 100);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterSugestaoParecerPareceristas>();

            // act 
            var retorno = await casoDeUso.Executar(proposta.Id, SituacaoParecerista.Aprovada);

            // assert
            var pareceristas = ObterTodos<PropostaParecerista>();
            var aprovadas = pareceristas.Where(w => w.PropostaId == proposta.Id && w.Situacao == SituacaoParecerista.Aprovada).OrderBy(o => o.Id).Select(t => t.Justificativa);
            var justificativas = string.Join('\n', aprovadas);

            retorno.ShouldBe(justificativas);
        }
    }
}
