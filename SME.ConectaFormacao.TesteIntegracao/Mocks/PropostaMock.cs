using Bogus;
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
           Formato formato,
           SituacaoProposta situacao,
           bool gerarFuncaoEspecificaOutros,
           bool gerarCriterioValidacaoInscricaoOutros,
           FormacaoHomologada formacaoHomologada,
           bool integrarNoSga = true,
           bool dataInscricaoForaPeriodo = false)
        {
            var faker = new Faker<Proposta>();
            faker.RuleFor(x => x.AreaPromotoraId, areaPromotoraId);
            faker.RuleFor(x => x.FormacaoHomologada, formacaoHomologada);
            faker.RuleFor(x => x.TipoFormacao, tipoFormacao);
            faker.RuleFor(x => x.Formato, formato);
            faker.RuleFor(x => x.NomeFormacao, f => f.Lorem.Sentence(3));
            faker.RuleFor(x => x.QuantidadeTurmas, f => f.Random.Short(1, 50));
            faker.RuleFor(x => x.QuantidadeVagasTurma, f => f.Random.Short(1, 30));
            faker.RuleFor(x => x.Excluido, false);
            faker.RuleFor(x => x.CargaHorariaPresencial, f => string.Format("{0}:{1}", f.Random.Short(100, 999).ToString(), f.Random.Short(10, 99).ToString()));
            faker.RuleFor(x => x.CargaHorariaSincrona, f => string.Format("{0}:{1}", f.Random.Short(100, 999).ToString(), f.Random.Short(10, 99).ToString()));
            faker.RuleFor(x => x.CargaHorariaSincrona, f => string.Format("{0}:{1}", f.Random.Short(100, 999).ToString(), f.Random.Short(10, 99).ToString()));
            faker.RuleFor(x => x.Justificativa, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.Objetivos, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.ConteudoProgramatico, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.ProcedimentoMetadologico, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.Referencia, f => f.Lorem.Sentence(200));
            faker.RuleFor(x => x.DataInscricaoInicio, dataInscricaoForaPeriodo ? DateTimeExtension.HorarioBrasilia().AddMonths(-2) : DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.DataInscricaoFim, dataInscricaoForaPeriodo ? DateTimeExtension.HorarioBrasilia().AddMonths(-1) : DateTimeExtension.HorarioBrasilia().AddMonths(1));
            faker.RuleFor(x => x.DataRealizacaoInicio, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.DataRealizacaoFim, DateTimeExtension.HorarioBrasilia());
            faker.RuleFor(x => x.IntegrarNoSGA, integrarNoSga);
            faker.RuleFor(x => x.CodigoEventoSigpec, f => f.Random.Long(100000, 9999999999));
            faker.RuleFor(x => x.NumeroHomologacao, f => f.Random.Long(100000, 9999999999));

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
            Formato formato,
            SituacaoProposta situacao,
            bool gerarFuncaoEspecificaOutros,
            bool gerarCriterioValidacaoInscricaoOutros,
            FormacaoHomologada formacaoHomologada,
             bool integrarNoSga = true,
             bool dataInscricaoForaPeriodo = false
            )
        {
            return Gerador(areaPromotoraId, tipoFormacao, formato, situacao, gerarFuncaoEspecificaOutros, gerarCriterioValidacaoInscricaoOutros, formacaoHomologada, integrarNoSga, dataInscricaoForaPeriodo);
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

        public static IEnumerable<PropostaEncontroTurma> GerarPropostaEncontroTurmas(long propostaEncontroId, IEnumerable<PropostaTurma> turmas)
        {
            foreach (var turma in turmas)
            {
                var faker = new Faker<PropostaEncontroTurma>();
                faker.RuleFor(x => x.PropostaEncontroId, propostaEncontroId);
                faker.RuleFor(x => x.TurmaId, turma.Id);
                faker.RuleFor(x => x.Excluido, false);
                AuditoriaFaker(faker);

                yield return faker.Generate();
            }
        }

        public static IEnumerable<PropostaTurmaDre> GerarPropostaTurmasDres(long propostaTurmaId, IEnumerable<Dre> dres)
        {
            foreach (var dre in dres)
            {
                var faker = new Faker<PropostaTurmaDre>();
                faker.RuleFor(x => x.PropostaTurmaId, propostaTurmaId);
                faker.RuleFor(x => x.DreId, dre.Id);
                faker.RuleFor(x => x.Dre, dre);
                faker.RuleFor(x => x.Excluido, false);
                AuditoriaFaker(faker);

                yield return faker.Generate();
            }
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

        public static IEnumerable<PropostaModalidade> GerarModalidades(long propostaId, IEnumerable<Modalidade> modalidades)
        {
            if (modalidades != null && modalidades.Any())
            {
                var quantidade = new Randomizer().Number(1, modalidades.Count());
                return modalidades
                    .Select(t => new PropostaModalidade
                    {
                        PropostaId = propostaId,
                        Modalidade = t,
                        CriadoEm = DateTimeExtension.HorarioBrasilia(),
                        CriadoPor = "Sistema",
                        CriadoLogin = "Sistema",
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaAnoTurma> GerarAnosTurmas(long propostaId, IEnumerable<AnoTurma> anosTurmas)
        {
            if (anosTurmas != null && anosTurmas.Any())
            {
                var quantidade = new Randomizer().Number(1, anosTurmas.Count());
                return anosTurmas
                    .Select(t => new PropostaAnoTurma
                    {
                        PropostaId = propostaId,
                        AnoTurmaId = t.Id,
                        CriadoEm = t.CriadoEm,
                        CriadoPor = t.CriadoPor,
                        CriadoLogin = t.CriadoLogin,
                    }).Take(quantidade);
            }

            return default;
        }

        public static IEnumerable<PropostaComponenteCurricular> GerarComponentesCurriculares(long propostaId, IEnumerable<ComponenteCurricular> componentesCurriculares)
        {
            if (componentesCurriculares != null && componentesCurriculares.Any())
            {
                var quantidade = new Randomizer().Number(1, componentesCurriculares.Count());
                return componentesCurriculares
                    .Select(t => new PropostaComponenteCurricular
                    {
                        PropostaId = propostaId,
                        ComponenteCurricularId = t.Id,
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

        public static IEnumerable<PropostaTutorTurma> GerarTutorTurmas(long propostaTutorId, IEnumerable<PropostaTurma> turmas)
        {
            foreach (var propostaTurma in turmas)
            {
                var turma = new PropostaTutorTurma
                {
                    PropostaTutorId = propostaTutorId,
                    TurmaId = propostaTurma.Id
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

        public static IEnumerable<PropostaRegenteTurma> GerarRegenteTurmas(long propostaRegenteId, IEnumerable<PropostaTurma> turmas)
        {
            foreach (var propostaTurma in turmas)
            {
                var turma = new PropostaRegenteTurma
                {
                    PropostaRegenteId = propostaRegenteId,
                    TurmaId = propostaTurma.Id
                };

                Auditoria(turma);

                yield return turma;
            }
        }

        public static IEnumerable<PropostaTurma> GerarTurmas(long propostaId, short quantidadeTurmas)
        {
            var faker = new Faker<PropostaTurma>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.Nome, f => f.Name.FirstName());
            AuditoriaFaker(faker);
            return faker.Generate(quantidadeTurmas);
        }

        public static IEnumerable<PropostaTurmaVaga> GerarTurmaVagas(IEnumerable<PropostaTurma> turmas, short quantidadeVagasTurmas)
        {
            var retorno = new List<PropostaTurmaVaga>();
            foreach (var turma in turmas)
            {
                for (int i = 0; i < quantidadeVagasTurmas; i++)
                {
                    var vaga = new PropostaTurmaVaga() { PropostaTurmaId = turma.Id };
                    Auditoria(vaga);
                    retorno.Add(vaga);
                }
            }

            return retorno;
        }
        public static PropostaTurmaVaga GerarTurmaVaga(long propostaTurmaId, long inscricaoId)
        {
            var vaga = new PropostaTurmaVaga() { PropostaTurmaId = propostaTurmaId, InscricaoId = inscricaoId };
            Auditoria(vaga);

            return vaga;
        }

        public static PropostaTipoInscricao GerarTiposInscricao(long propostaId, TipoInscricao tipoInscricao)
        {
            var faker = new Faker<PropostaTipoInscricao>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.TipoInscricao, tipoInscricao);

            AuditoriaFaker(faker);

            return faker.Generate();
        }

        public static IEnumerable<PropostaParecerista> GerarPareceristas(long propostaId, int quantidade = 1, string rf = "")
        {
            var faker = new Faker<PropostaParecerista>();
            faker.RuleFor(x => x.PropostaId, propostaId);
            faker.RuleFor(x => x.NomeParecerista, f => f.Person.FullName);
            faker.RuleFor(x => x.Situacao, f => f.PickRandom<SituacaoParecerista>());
            faker.RuleFor(x => x.RegistroFuncional, f => string.IsNullOrEmpty(rf) ? f.Random.Number(10000, 99999).ToString() : rf);
            faker.RuleFor(x => x.Justificativa, f => f.Lorem.Word());

            AuditoriaFaker(faker);

            return faker.Generate(quantidade);
        }
    }
}
