﻿using Bogus;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.TesteIntegracao.Mocks
{
    public class PropostaMock : BaseMock
    {
        private static Faker<Proposta> Gerador(
            long areaPromotoraId,
           TipoFormacao tipoFormacao,
           Modalidade modalidade,
           SituacaoProposta situacao,
           bool gerarFuncaoEspecificaOutros,
           bool gerarCriterioValidacaoInscricaoOutros)
        {
            var faker = new Faker<Proposta>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.TipoFormacao, tipoFormacao);
            faker.RuleFor(x => x.Modalidade, modalidade);
            faker.RuleFor(x => x.TipoInscricao, f => f.PickRandom<TipoInscricao>());
            faker.RuleFor(x => x.NomeFormacao, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.QuantidadeTurmas, f => f.Random.Short(1, 999));
            faker.RuleFor(x => x.QuantidadeVagasTurma, f => f.Random.Short(1, 999));
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CargaHorariaPresencial, f => string.Format("{0}:{1}", f.Random.Short(100, 999).ToString(), f.Random.Short(10, 99).ToString()));
            faker.RuleFor(x => x.CargaHorariaSincrona, f => string.Format("{0}:{1}", f.Random.Short(100, 999).ToString(), f.Random.Short(10, 99).ToString()));
            faker.RuleFor(x => x.CargaHorariaSincrona, f => string.Format("{0}:{1}", f.Random.Short(100, 999).ToString(), f.Random.Short(10, 99).ToString()));
            faker.RuleFor(x => x.Justificativa, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.Objetivos, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.ConteudoProgramatico, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.ProcedimentoMetadologico, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.Referencia, f => f.Lorem.Sentence(200));
            AuditoriaFaker(faker);

            if (gerarFuncaoEspecificaOutros)
                faker.RuleFor(x => x.FuncaoEspecificaOutros, f => f.Lorem.Sentence(3));

            if (gerarCriterioValidacaoInscricaoOutros)
                faker.RuleFor(x => x.CriterioValidacaoInscricaoOutros, f => f.Lorem.Sentence(3));

            faker.RuleFor(x => x.Situacao, situacao);

            return faker;
        }

        public static Proposta GerarPropostaValida(long areaPromotoraId,
            TipoFormacao tipoFormacao,
            Modalidade modalidade,
            SituacaoProposta situacao,
            bool gerarFuncaoEspecificaOutros,
            bool gerarCriterioValidacaoInscricaoOutros)
        {
            return Gerador(areaPromotoraId, tipoFormacao, modalidade, situacao, gerarFuncaoEspecificaOutros, gerarCriterioValidacaoInscricaoOutros);
        }

        public static Proposta GerarPropostaRascunho(long areaPromotoraId)
        {
            var faker = new Faker<Proposta>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);

            return faker.Generate();
        }

        public static IEnumerable<PropostaPublicoAlvo> GerarPublicoAlvo(long propostaId, IEnumerable<CargoFuncao> cargosFuncoes)
        {
            if (cargosFuncoes != null && cargosFuncoes.Any())
            {
                var quantidade = new Randomizer().Number(1, cargosFuncoes.Count());
                return cargosFuncoes
                    .Select(t => new PropostaPublicoAlvo
                    {
                        PropostaId = propostaId,
                        CargoFuncaoId = t.Id,
                        CriadoEm = t.CriadoEm,
                        CriadoPor = t.CriadoPor,
                        CriadoLogin = t.CriadoLogin,
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaFuncaoEspecifica> GerarFuncoesEspecificas(long propostaId, IEnumerable<CargoFuncao> cargosFuncoes)
        {
            if (cargosFuncoes != null && cargosFuncoes.Any())
            {
                var quantidade = new Randomizer().Number(1, cargosFuncoes.Count());
                return cargosFuncoes
                    .Select(t => new PropostaFuncaoEspecifica
                    {
                        PropostaId = propostaId,
                        CargoFuncaoId = t.Id,
                        CriadoEm = t.CriadoEm,
                        CriadoPor = t.CriadoPor,
                        CriadoLogin = t.CriadoLogin,
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaVagaRemanecente> GerarVagasRemanecentes(long propostaId, IEnumerable<CargoFuncao> cargosFuncoes)
        {
            if (cargosFuncoes != null && cargosFuncoes.Any())
            {
                var quantidade = new Randomizer().Number(1, cargosFuncoes.Count());
                return cargosFuncoes
                    .Select(t => new PropostaVagaRemanecente
                    {
                        PropostaId = propostaId,
                        CargoFuncaoId = t.Id,
                        CriadoEm = t.CriadoEm,
                        CriadoPor = t.CriadoPor,
                        CriadoLogin = t.CriadoLogin,
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaCriterioValidacaoInscricao> GerarCritariosValidacaoInscricao(long propostaId, IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            if (criteriosValidacaoInscricao != null && criteriosValidacaoInscricao.Any())
            {
                var quantidade = new Randomizer().Number(1, criteriosValidacaoInscricao.Count());
                return criteriosValidacaoInscricao
                    .Select(t => new PropostaCriterioValidacaoInscricao
                    {
                        PropostaId = propostaId,
                        CriterioValidacaoInscricaoId = t.Id,
                        CriadoEm = t.CriadoEm,
                        CriadoPor = t.CriadoPor,
                        CriadoLogin = t.CriadoLogin,
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaEncontro> GerarEncontros(long propostaId)
        {
            var quantidade = new Randomizer().Number(1, 9);

            var faker = new Faker<PropostaEncontro>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.Tipo, f => f.PickRandom<TipoEncontro>());
            faker.RuleFor(x => x.HoraInicio, f => DateTimeExtension.HorarioBrasilia().ToString("HH:mm"));
            faker.RuleFor(x => x.HoraFim, f => DateTimeExtension.HorarioBrasilia().AddMinutes(quantidade).ToString("HH:mm"));
            faker.RuleFor(x => x.Local, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.Excluido, false);

            AuditoriaFaker(faker);

            return faker.Generate(quantidade);
        }

        public static IEnumerable<PropostaEncontroTurma> GerarPropostaEncontroTurmas(long propostaEncontroId, short quantidadeTurmas)
        {
            var quantidade = new Randomizer().Number(1, 9);

            var faker = new Faker<PropostaEncontroTurma>();
            faker.RuleFor(x => x.PropostaEncontroId, propostaEncontroId);
            faker.RuleFor(x => x.Turma, f => f.Random.Short(1, quantidadeTurmas));
            faker.RuleFor(x => x.Excluido, false);
            AuditoriaFaker(faker);

            return faker.Generate(quantidade);
        }

        public static IEnumerable<CriterioCertificacao> GerarCriteriosCertificacao()
        {
            var quantidade = new Randomizer().Number(1, 9);
            var faker = new Faker<CriterioCertificacao>();
            faker.RuleFor(x => x.Descricao, f => f.Name.FirstName());
            AuditoriaFaker(faker);
            return faker.Generate(quantidade);
        }
        public static IEnumerable<PropostaEncontroData> GerarPropostaEncontroDatas(long propostaEncontroId)
        {
            var quantidade = new Randomizer().Number(1, 9);

            var faker = new Faker<PropostaEncontroData>();
            faker.RuleFor(x => x.PropostaEncontroId, propostaEncontroId);
            faker.RuleFor(x => x.DataInicio, DateTime.Now);
            faker.RuleFor(x => x.DataFim, f => f.Random.Bool() ? f.Date.Future() : null);
            AuditoriaFaker(faker);

            return faker.Generate(quantidade);
        }

        public static IEnumerable<PropostaPalavraChave> GerarPalavrasChaves(long propostaId, IEnumerable<PalavraChave> palavrasChaves)
        {
            if (palavrasChaves != null && palavrasChaves.Any())
            {
                var quantidade = new Randomizer().Number(1, palavrasChaves.Count());
                return palavrasChaves
                    .Select(t => new PropostaPalavraChave
                    {
                        PropostaId = propostaId,
                        PalavraChaveId = t.Id,
                        CriadoEm = t.CriadoEm,
                        CriadoPor = t.CriadoPor,
                        CriadoLogin = t.CriadoLogin,
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaTutor> GerarTutor(long propostaId)
        {
            var faker = new Faker<PropostaTutor>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.ProfissionalRedeMunicipal, f => true);
            faker.RuleFor(x => x.RegistroFuncional, f => f.Random.Short(100, 1000).ToString());
            faker.RuleFor(x => x.NomeTutor, f => f.Person.FullName);

            AuditoriaFaker(faker);

            return faker.Generate(1);
        }

        public static IEnumerable<PropostaTutorTurma> GerarTutorTurmas(long propostaTutorId, short quantidadeTurmas)
        {
            var random = new Random().Next(1, quantidadeTurmas);

            for (int i = 1; i <= random; i++)
            {
                var turma = new PropostaTutorTurma
                {
                    PropostaTutorId = propostaTutorId,
                    Turma = i
                };

                Auditoria(turma);

                yield return turma;
            }
        }

        public static IEnumerable<PropostaRegente> GerarRegente(long propostaId)
        {
            var faker = new Faker<PropostaRegente>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.ProfissionalRedeMunicipal, f => true);
            faker.RuleFor(x => x.RegistroFuncional, f => f.Random.Short(100, 1000).ToString());
            faker.RuleFor(x => x.NomeRegente, f => f.Person.FullName);

            AuditoriaFaker(faker);

            return faker.Generate(1);
        }

        public static IEnumerable<PropostaRegenteTurma> GerarRegenteTurmas(long propostaRegenteId, short quantidadeTurmas)
        {
            var random = new Random().Next(1, quantidadeTurmas);

            for (int i = 1; i <= random; i++)
            {
                var turma = new PropostaRegenteTurma
                {
                    PropostaRegenteId = propostaRegenteId,
                    Turma = i
                };

                Auditoria(turma);

                yield return turma;
            }
        }
    }
}
