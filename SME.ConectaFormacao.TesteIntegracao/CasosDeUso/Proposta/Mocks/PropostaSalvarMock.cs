﻿using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.PalavraChave;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks
{
    public class PropostaSalvarMock
    {
        private static IEnumerable<PropostaEncontroTurmaDTO> GerarPropostaEncontroTurmas(int quantidadeTurmas)
        {
            var quantidade = new Randomizer().Number(1, quantidadeTurmas);

            for (short i = 1; i <= quantidade; i++)
                yield return new PropostaEncontroTurmaDTO { Turma = i };
        }

        private static IEnumerable<PropostaEncontroDataDTO> GerarPropostaEncontroDatas()
        {
            var quantidade = new Randomizer().Number(1, 99);

            var faker = new Faker<PropostaEncontroDataDTO>();
            faker.RuleFor(x => x.DataInicio, DateTime.Now);
            faker.RuleFor(x => x.DataFim, f => f.Random.Bool() ? f.Date.Future() : null);

            return faker.Generate(quantidade);
        }

        private static Faker<PropostaEncontroDTO> GeradorEncontro(int quantidadeTurmas)
        {
            var faker = new Faker<PropostaEncontroDTO>();
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<TipoEncontro>());
            faker.RuleFor(x => x.HoraInicio, f => DateTimeExtension.HorarioBrasilia().ToString("HH:mm"));
            faker.RuleFor(x => x.HoraFim, f => DateTimeExtension.HorarioBrasilia().AddMinutes(f.Random.Short(0, 60)).ToString("HH:mm"));
            faker.RuleFor(x => x.Local, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.Turmas, GerarPropostaEncontroTurmas(quantidadeTurmas));
            faker.RuleFor(x => x.Datas, GerarPropostaEncontroDatas());

            return faker;
        }

        public static IEnumerable<PropostaEncontroDTO> GerarEncontro(int quantidade, int quantidadeTurmas)
        {
            return GeradorEncontro(quantidadeTurmas).Generate(quantidade);
        }

        private static Faker<PropostaDTO> Gerador(
            TipoFormacao tipoFormacao,
            Modalidade modalidade,
            IEnumerable<PropostaPublicoAlvoDTO> propostaPublicoAlvos,
            IEnumerable<PropostaFuncaoEspecificaDTO> propostaFuncaoEspecificas,
            IEnumerable<PropostaCriterioValidacaoInscricaoDTO> propostaCriterioValidacaoInscricaos,
            IEnumerable<PropostaVagaRemanecenteDTO> propostaVagaRemanecentes,
            IEnumerable<PropostaPalavraChaveDTO> palavrasChaves,
            SituacaoProposta situacao,
            bool gerarFuncaoEspecificaOutros,
            bool gerarCriterioValidacaoInscricaoOutros,
            long? arquivoImagemDivulgacaoId)
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
            faker.RuleFor(x => x.QuantidadeTurmas, f => f.Random.Short(1, 99));
            faker.RuleFor(x => x.QuantidadeVagasTurma, f => f.Random.Short(1, 99));
            faker.RuleFor(x => x.PalavrasChaves, f => palavrasChaves.Take(5)); //Melhorar isso
            faker.RuleFor(x => x.Justificativa, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Objetivos, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.ConteudoProgramatico, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.ProcedimentoMetadologico, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Referencia, f => f.Lorem.Sentence(100));

            if (gerarFuncaoEspecificaOutros)
                faker.RuleFor(x => x.FuncaoEspecificaOutros, f => f.Lorem.Sentence(3));

            if (gerarCriterioValidacaoInscricaoOutros)
                faker.RuleFor(x => x.CriterioValidacaoInscricaoOutros, f => f.Lorem.Sentence(3));

            if (arquivoImagemDivulgacaoId.HasValue)
                faker.RuleFor(x => x.ArquivoImagemDivulgacaoId, arquivoImagemDivulgacaoId);

            faker.RuleFor(x => x.Situacao, situacao);

            return faker;
        }


        public static PropostaDTO GerarPropostaDTOVazio(SituacaoProposta situacaoRegistro)
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
            IEnumerable<PropostaPalavraChaveDTO> propostaPalavrasChaves,
            SituacaoProposta situacao, bool gerarFuncaoEspecificaOutros = false, bool gerarCriterioValidacaoInscricaoOutros = false,
            long? arquivoImagemDivulgacaoId = null)
        {
            var propostaDTO = Gerador(tipoFormacao, modalidade, propostaPublicoAlvos, propostaFuncaoEspecificas, 
                propostaCriterioValidacaoInscricaos, propostaVagaRemanecentes, propostaPalavrasChaves,
                situacao, gerarFuncaoEspecificaOutros, 
                gerarCriterioValidacaoInscricaoOutros, arquivoImagemDivulgacaoId).Generate();
            propostaDTO.Encontros = PropostaSalvarMock.GerarEncontro(2, propostaDTO.QuantidadeTurmas.GetValueOrDefault());

            return propostaDTO;
        }

        public static long GerarIdAleatorio()
        {
            return new Faker().Random.Long(1);
        }

        public static Guid GrupoUsuarioLogadoId { get; set; }

    }
}
