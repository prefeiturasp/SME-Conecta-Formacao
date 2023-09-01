using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class PropostaSalvarMock
    {
        private static Faker<PropostaDTO> Gerador(
            TipoFormacao tipoFormacao,
            Modalidade modalidade,
            IEnumerable<PropostaPublicoAlvoDTO> propostaPublicoAlvos,
            IEnumerable<PropostaFuncaoEspecificaDTO> propostaFuncaoEspecificas,
            IEnumerable<PropostaCriterioValidacaoInscricaoDTO> propostaCriterioValidacaoInscricaos,
            IEnumerable<PropostaVagaRemanecenteDTO> propostaVagaRemanecentes,
            SituacaoRegistro situacao,
            bool gerarFuncaoEspecificaOutros,
            bool gerarCriterioValidacaoInscricaoOutros)
        {
            var faker = new Faker<PropostaDTO>();
            faker.RuleFor(x => x.TipoFormacao, tipoFormacao);
            faker.RuleFor(x => x.Modalidade, modalidade);
            faker.RuleFor(x => x.TipoInscricao, f => f.PickRandom<TipoInscricao>());
            faker.RuleFor(x => x.NomeFormacao, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.PublicosAlvo, f => new PropostaPublicoAlvoDTO[] { f.PickRandom(propostaPublicoAlvos) });
            faker.RuleFor(x => x.FuncoesEspecificas, f => new PropostaFuncaoEspecificaDTO[] { f.PickRandom(propostaFuncaoEspecificas) });
            faker.RuleFor(x => x.CriteriosValidacaoInscricao, f => new PropostaCriterioValidacaoInscricaoDTO[] { f.PickRandom(propostaCriterioValidacaoInscricaos) });
            faker.RuleFor(x => x.VagasRemanecentes, f => new PropostaVagaRemanecenteDTO[] { f.PickRandom(propostaVagaRemanecentes) });
            faker.RuleFor(x => x.QuantidadeTurmas, f => f.Random.Number(1, 999));
            faker.RuleFor(x => x.QuantidadeVagasTurma, f => f.Random.Number(1, 999));

            if (gerarFuncaoEspecificaOutros)
                faker.RuleFor(x => x.FuncaoEspecificaOutros, f => f.Lorem.Sentence(3));

            if (gerarCriterioValidacaoInscricaoOutros)
                faker.RuleFor(x => x.CriterioValidacaoInscricaoOutros, f => f.Lorem.Sentence(3));

            faker.RuleFor(x => x.Situacao, situacao);

            return faker;
        }


        public static PropostaDTO GerarPropostaDTOVazio(SituacaoRegistro situacaoRegistro)
        {
            return new PropostaDTO { Situacao = situacaoRegistro };
        }

        internal static PropostaDTO GerarPropostaDTOValida(
            TipoFormacao tipoFormacao,
            Modalidade modalidade,
            IEnumerable<PropostaPublicoAlvoDTO> propostaPublicoAlvos,
            IEnumerable<PropostaFuncaoEspecificaDTO> propostaFuncaoEspecificas,
            IEnumerable<PropostaCriterioValidacaoInscricaoDTO> propostaCriterioValidacaoInscricaos,
            IEnumerable<PropostaVagaRemanecenteDTO> propostaVagaRemanecentes,
            SituacaoRegistro situacao, bool gerarFuncaoEspecificaOutros = false, bool gerarCriterioValidacaoInscricaoOutros = false)
        {
            return Gerador(tipoFormacao, modalidade, propostaPublicoAlvos, propostaFuncaoEspecificas, propostaCriterioValidacaoInscricaos, propostaVagaRemanecentes, situacao, gerarFuncaoEspecificaOutros, gerarCriterioValidacaoInscricaoOutros).Generate();
        }

        public static Guid GrupoUsuarioLogadoId { get; set; }

    }
}
