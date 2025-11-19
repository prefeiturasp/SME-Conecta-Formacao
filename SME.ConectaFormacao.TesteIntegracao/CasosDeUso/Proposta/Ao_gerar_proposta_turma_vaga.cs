using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_gerar_proposta_turma_vaga : TestePropostaBase
    {
        public Ao_gerar_proposta_turma_vaga(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve gerar proposta turma vagas para proposta na situação Publicada e não homologada")]
        public async Task Deve_gerar_proposta_turma_vaga_proposta_publicada_e_nao_homologada()
        {
            // arrange
            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.Publicada, formacaoHomologada: FormacaoHomologada.NaoCursosPorIN);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoGerarPropostaTurmaVaga>();

            var mensagem = new MensagemRabbit(proposta.Id.ToString());

            // act
            await casoDeUso.Executar(mensagem);

            // assert
            var vagas = ObterTodos<Dominio.Entidades.PropostaTurmaVaga>();

            vagas.ShouldNotBeEmpty();

            var totalVagas = proposta.QuantidadeTurmas.GetValueOrDefault() * proposta.QuantidadeVagasTurma.GetValueOrDefault();
            vagas.Count.ShouldBe(totalVagas);
        }
    }
}
