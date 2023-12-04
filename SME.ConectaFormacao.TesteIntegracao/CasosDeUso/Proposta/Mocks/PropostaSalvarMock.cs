﻿using Bogus;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
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
        private static IEnumerable<PropostaRegenteTurmaDTO> GerarPropostaRegenteTurmas(int quantidadeTurmas)
        {
            var quantidade = new Randomizer().Number(1, quantidadeTurmas);

            for (short i = 1; i <= quantidade; i++)
                yield return new PropostaRegenteTurmaDTO { Turma = i };
        }
        private static IEnumerable<PropostaTutorTurmaDTO> GerarPropostaTutorTurmas(int quantidadeTurmas)
        {
            var quantidade = new Randomizer().Number(1, quantidadeTurmas);

            for (short i = 1; i <= quantidade; i++)
                yield return new PropostaTutorTurmaDTO { Turma = i };
        }

        private static IEnumerable<PropostaEncontroDataDTO> GerarPropostaEncontroDatas()
        {
            var quantidade = new Randomizer().Number(1, 99);

            var faker = new Faker<PropostaEncontroDataDTO>();
            faker.RuleFor(x => x.DataInicio, DateTime.Now);
            faker.RuleFor(x => x.DataFim, f => f.Random.Bool() ? f.Date.Future() : null);

            return faker.Generate(quantidade);
        }

        public static Faker<PropostaEncontroDTO> GeradorEncontro(int quantidadeTurmas)
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

        public static Faker<PropostaRegenteDTO> GeradorRegente(int quantidadeTurmas)
        {
            var faker = new Faker<PropostaRegenteDTO>();
            faker.RuleFor(x => x.ProfissionalRedeMunicipal, f => true);
            faker.RuleFor(x => x.RegistroFuncional, f => f.Random.Short(100, 1000).ToString());
            faker.RuleFor(x => x.NomeRegente, f => f.Person.FullName);
            faker.RuleFor(x => x.MiniBiografia, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.Turmas, GerarPropostaRegenteTurmas(quantidadeTurmas));

            return faker;
        }
        public static Faker<PropostaTutorDTO> GeradorTutor(int quantidadeTurmas)
        {
            var faker = new Faker<PropostaTutorDTO>();
            faker.RuleFor(x => x.ProfissionalRedeMunicipal, f => true);
            faker.RuleFor(x => x.RegistroFuncional, f => f.Random.Short(100, 1000).ToString());
            faker.RuleFor(x => x.NomeTutor, f => f.Person.FullName.ToUpper());
            faker.RuleFor(x => x.Turmas, GerarPropostaTutorTurmas(quantidadeTurmas));

            return faker;
        }

        public static PropostaTutorDTO GerarTutor(short quantidadeTurmas)
        {
            return GeradorTutor(quantidadeTurmas);
        }
        public static PropostaRegenteDTO GerarRegente(short quantidadeTurmas)
        {
            return GeradorRegente(quantidadeTurmas);
        }
        public static PropostaEncontroDTO GerarEncontro(short quantidadeTurmas)
        {
            return GeradorEncontro(quantidadeTurmas);
        }

        private static Faker<PropostaDTO> Gerador(
            TipoFormacao tipoFormacao,
            Formato formato,
            IEnumerable<PropostaPublicoAlvoDTO> propostaPublicoAlvos,
            IEnumerable<PropostaFuncaoEspecificaDTO> propostaFuncaoEspecificas,
            IEnumerable<PropostaCriterioValidacaoInscricaoDTO> propostaCriterioValidacaoInscricaos,
            IEnumerable<PropostaVagaRemanecenteDTO> propostaVagaRemanecentes,
            IEnumerable<PropostaPalavraChaveDTO> palavrasChaves,
            SituacaoProposta situacao,
            bool gerarFuncaoEspecificaOutros,
            bool gerarCriterioValidacaoInscricaoOutros,
            long? arquivoImagemDivulgacaoId,
            short? quantidadeTurmas)
        {
            var faker = new Faker<PropostaDTO>();
            faker.RuleFor(x => x.FormacaoHomologada, f => f.PickRandom<FormacaoHomologada>());
            faker.RuleFor(x => x.TipoFormacao, tipoFormacao);
            faker.RuleFor(x => x.Formato, formato);
            faker.RuleFor(x => x.TipoInscricao, f => f.PickRandom<TipoInscricao>());
            faker.RuleFor(x => x.NomeFormacao, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.PublicosAlvo, f => new PropostaPublicoAlvoDTO[] { f.PickRandom(propostaPublicoAlvos) });
            faker.RuleFor(x => x.FuncoesEspecificas, f => new PropostaFuncaoEspecificaDTO[] { f.PickRandom(propostaFuncaoEspecificas) });
            faker.RuleFor(x => x.CriteriosValidacaoInscricao, f => new PropostaCriterioValidacaoInscricaoDTO[] { f.PickRandom(propostaCriterioValidacaoInscricaos) });
            faker.RuleFor(x => x.VagasRemanecentes, f => new PropostaVagaRemanecenteDTO[] { f.PickRandom(propostaVagaRemanecentes) });
            faker.RuleFor(x => x.QuantidadeTurmas, f => quantidadeTurmas.HasValue ? quantidadeTurmas.Value : f.Random.Short(1, 99));
            faker.RuleFor(x => x.QuantidadeVagasTurma, f => f.Random.Short(1, 99));
            faker.RuleFor(x => x.PalavrasChaves, f => palavrasChaves.Take(5)); //Melhorar isso
            faker.RuleFor(x => x.Justificativa, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Objetivos, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.ConteudoProgramatico, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.ProcedimentoMetadologico, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.Referencia, f => f.Lorem.Sentence(100));
            faker.RuleFor(x => x.DataInscricaoInicio, f => DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.DataInscricaoFim, f => DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.DataRealizacaoInicio, f => DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.DataRealizacaoFim, f => DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.AcaoInformativa, true);
            faker.RuleFor(x => x.CargaHorariaPresencial, DateTimeExtension.HorarioBrasilia().ToString("HH:mm"));

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

        internal static PropostaRegenteDTO GeraPropostaRegenteDTOValida(PropostaRegente regente)
        {
            return new PropostaRegenteDTO
            {
                Id = regente.Id,
                ProfissionalRedeMunicipal = regente.ProfissionalRedeMunicipal,
                NomeRegente = regente.NomeRegente,
                RegistroFuncional = regente.RegistroFuncional,
                MiniBiografia = regente.MiniBiografia,
                Turmas = GeraPropostaRegenteTurmaDTOValida(regente.Turmas)
            };
        }
        internal static PropostaTutorDTO GeraPropostaTutorDTOValida(PropostaTutor tutor)
        {
            return new PropostaTutorDTO
            {
                Id = tutor.Id,
                ProfissionalRedeMunicipal = tutor.ProfissionalRedeMunicipal,
                NomeTutor = tutor.NomeTutor,
                RegistroFuncional = tutor.RegistroFuncional,
                Turmas = GeraPropostaTutorTurmaDTOValida(tutor.Turmas)
            };
        }
        internal static IEnumerable<PropostaRegenteTurma> GeraPropostaRegenteTurmaValida(long propostaRegenteId, IEnumerable<PropostaRegenteTurmaDTO> regenteTurmas)
        {
            var retorno = new List<PropostaRegenteTurma>();
            foreach (var regenteTurma in regenteTurmas)
                retorno.Add(new PropostaRegenteTurma() { Turma = regenteTurma.Turma, PropostaRegenteId = propostaRegenteId });
            return retorno;
        }
        internal static IEnumerable<PropostaTutorTurma> GeraPropostaTutorTurmaValida(long propostaTutorId, IEnumerable<PropostaTutorTurmaDTO> tutorTurmas)
        {
            var retorno = new List<PropostaTutorTurma>();
            foreach (var tutorTurma in tutorTurmas)
                retorno.Add(new PropostaTutorTurma() { Turma = tutorTurma.Turma, PropostaTutorId = propostaTutorId });
            return retorno;
        }
        internal static IEnumerable<PropostaRegenteTurmaDTO> GeraPropostaRegenteTurmaDTOValida(IEnumerable<PropostaRegenteTurma> regenteTurmas)
        {
            var retorno = new List<PropostaRegenteTurmaDTO>();
            foreach (var regenteTurma in regenteTurmas)
                retorno.Add(new PropostaRegenteTurmaDTO() { Turma = regenteTurma.Turma });
            return retorno;
        }
        internal static IEnumerable<PropostaTutorTurmaDTO> GeraPropostaTutorTurmaDTOValida(IEnumerable<PropostaTutorTurma> tutorTurmas)
        {
            var retorno = new List<PropostaTutorTurmaDTO>();
            foreach (var tutorTurma in tutorTurmas)
                retorno.Add(new PropostaTutorTurmaDTO() { Turma = tutorTurma.Turma });
            return retorno;
        }

        public static IEnumerable<CriterioCertificacaoDTO> GerarCriterioCertificacaoDTO(int quantidade)
        {
            var faker = new Faker<CriterioCertificacaoDTO>();
            faker.RuleFor(x => x.CriterioCertificacaoId, 1);
            return faker.Generate(quantidade);
        }
        internal static PropostaDTO GerarPropostaDTOValida(
            TipoFormacao tipoFormacao,
            Formato formato,
            IEnumerable<PropostaPublicoAlvoDTO> propostaPublicoAlvos,
            IEnumerable<PropostaFuncaoEspecificaDTO> propostaFuncaoEspecificas,
            IEnumerable<PropostaCriterioValidacaoInscricaoDTO> propostaCriterioValidacaoInscricaos,
            IEnumerable<PropostaVagaRemanecenteDTO> propostaVagaRemanecentes,
            IEnumerable<PropostaPalavraChaveDTO> propostaPalavrasChaves,
            SituacaoProposta situacao, bool gerarFuncaoEspecificaOutros = false, bool gerarCriterioValidacaoInscricaoOutros = false,
            long? arquivoImagemDivulgacaoId = null, short? quantidadeTurmas = null)
        {
            var propostaDTO = Gerador(tipoFormacao, formato, propostaPublicoAlvos, propostaFuncaoEspecificas,
                propostaCriterioValidacaoInscricaos, propostaVagaRemanecentes, propostaPalavrasChaves,
                situacao, gerarFuncaoEspecificaOutros,
                gerarCriterioValidacaoInscricaoOutros, arquivoImagemDivulgacaoId, quantidadeTurmas).Generate();

            return propostaDTO;
        }

        public static long GerarIdAleatorio()
        {
            return new Faker().Random.Long(1);
        }

        public static Guid GrupoUsuarioLogadoId { get; set; }


    }
}
