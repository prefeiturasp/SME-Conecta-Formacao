using Bogus;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public abstract class TestePropostaBase : TesteBase
    {
        protected TestePropostaBase(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }
        protected async Task<IEnumerable<Dominio.Entidades.Proposta>> InserirNaBaseProposta(int quantidade)
        {
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            return await InserirNaBaseProposta(quantidade, areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
            modalidades, anosTurmas, componentesCurriculares);
        }

        protected async Task<Dominio.Entidades.Proposta> InserirNaBaseProposta(SituacaoProposta situacao = SituacaoProposta.Cadastrada,
            FormacaoHomologada formacaoHomologada = FormacaoHomologada.Sim, TipoInscricao tipoInscricao = TipoInscricao.Automatica, bool vincularUltimoCargoAoPublicoAlvo = false,
            bool vincularUltimoFuncaoAoPublicoAlvo = false, bool integrarNoSga = true, bool dataInscricaoForaPeriodo = false, bool numeroHomologacao = false, int quantidadeParecerista = 0)
        {
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            if (vincularUltimoCargoAoPublicoAlvo)
                cargosFuncoes = new List<Dominio.Entidades.CargoFuncao>() { cargosFuncoes.LastOrDefault(w => w.Tipo == CargoFuncaoTipo.Cargo) };

            if (vincularUltimoFuncaoAoPublicoAlvo)
                cargosFuncoes = new List<Dominio.Entidades.CargoFuncao>() { cargosFuncoes.LastOrDefault(w => w.Tipo == CargoFuncaoTipo.Funcao) };

            return await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, situacao, formacaoHomologada, tipoInscricao, integrarNoSga, dataInscricaoForaPeriodo, numeroHomologacao, quantidadeParecerista);
        }

        protected async Task<Dominio.Entidades.Proposta> InserirNaBaseProposta(Dominio.Entidades.AreaPromotora areaPromotora,
            IEnumerable<Dominio.Entidades.CargoFuncao> cargosFuncoes, IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao,
            IEnumerable<PalavraChave> palavrasChaves, IEnumerable<Dominio.Enumerados.Modalidade> modalidades, IEnumerable<Dominio.Entidades.AnoTurma> anosTurmas,
            IEnumerable<Dominio.Entidades.ComponenteCurricular> componentesCurriculares, SituacaoProposta situacao = SituacaoProposta.Cadastrada,
            FormacaoHomologada formacaoHomologada = FormacaoHomologada.Sim, TipoInscricao tipoInscricao = TipoInscricao.Automatica, 
            bool integrarNoSga = true, bool dataInscricaoForaPeriodo = false, bool numeroHomologacao = false,            int quantidadeParecerista = 0)
        {
            var proposta = PropostaMock.GerarPropostaValida(
                areaPromotora.Id,
                TipoFormacao.Curso,
                Formato.Presencial,
                situacao,
                false, false, formacaoHomologada, integrarNoSga, dataInscricaoForaPeriodo);

            proposta.AreaPromotora = areaPromotora;

            if (tipoInscricao == TipoInscricao.Externa)
                proposta.LinkParaInscricoesExterna = new Faker().Lorem.Sentence(20);

            if (numeroHomologacao)
                proposta.NumeroHomologacao = new Random().NextInt64(100000, 9999999999);

            await InserirNaBase(proposta);

            var publicosAlvo = PropostaMock.GerarPublicoAlvo(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo));
            if (publicosAlvo != null)
            {
                await InserirNaBase(publicosAlvo);
                proposta.PublicosAlvo = publicosAlvo;
            }

            var funcoesEspecifica = PropostaMock.GerarFuncoesEspecificas(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao));
            if (funcoesEspecifica != null)
            {
                await InserirNaBase(funcoesEspecifica);
                proposta.FuncoesEspecificas = funcoesEspecifica;
            }

            var vagasRemanecentes = PropostaMock.GerarVagasRemanecentes(proposta.Id, cargosFuncoes);
            if (vagasRemanecentes != null)
            {
                await InserirNaBase(vagasRemanecentes);
                proposta.VagasRemanecentes = vagasRemanecentes;
            }

            var criterios = PropostaMock.GerarCritariosValidacaoInscricao(proposta.Id, criteriosValidacaoInscricao);
            if (criterios != null)
            {
                await InserirNaBase(criterios);
                proposta.CriteriosValidacaoInscricao = criterios;
            }

            var dres = DreMock.GerarDreValida(10);
            await InserirNaBase(dres);

            var turmas = PropostaMock.GerarTurmas(proposta.Id, proposta.QuantidadeTurmas.GetValueOrDefault());
            if (turmas != null)
            {
                await InserirNaBase(turmas);

                foreach (var turma in turmas)
                {
                    var turmaDre = PropostaMock.GerarPropostaTurmasDres(turma.Id, dres);
                    await InserirNaBase(turmaDre);
                    turma.Dres = turmaDre;
                }

                proposta.Turmas = turmas;
            }

            var encontros = PropostaMock.GerarEncontros(proposta.Id);
            if (encontros != null)
            {
                await InserirNaBase(encontros);

                foreach (var encontro in encontros)
                {
                    var encontroTurmas = PropostaMock.GerarPropostaEncontroTurmas(encontro.Id, turmas);
                    await InserirNaBase(encontroTurmas);
                    encontro.Turmas = encontroTurmas;

                    var datas = PropostaMock.GerarPropostaEncontroDatas(encontro.Id);
                    await InserirNaBase(datas);
                    encontro.Datas = datas;
                }

                proposta.Encontros = encontros;
            }

            var palavrasChavesDaProposta = PropostaMock.GerarPalavrasChaves(proposta.Id, palavrasChaves);
            if (palavrasChavesDaProposta != null)
            {
                await InserirNaBase(palavrasChavesDaProposta);
                proposta.PalavrasChaves = palavrasChavesDaProposta;
            }

            var modalidadesDaProposta = PropostaMock.GerarModalidades(proposta.Id, modalidades);
            if (modalidadesDaProposta != null)
            {
                await InserirNaBase(modalidadesDaProposta);
                proposta.Modalidades = modalidadesDaProposta;
            }

            var anosTurmasDaProposta = PropostaMock.GerarAnosTurmas(proposta.Id, anosTurmas);
            if (anosTurmasDaProposta != null)
            {
                await InserirNaBase(anosTurmasDaProposta);
                proposta.AnosTurmas = anosTurmasDaProposta;
            }

            var componentesCurricularesDaProposta = PropostaMock.GerarComponentesCurriculares(proposta.Id, componentesCurriculares);
            if (componentesCurricularesDaProposta != null)
            {
                await InserirNaBase(componentesCurricularesDaProposta);
                proposta.ComponentesCurriculares = componentesCurricularesDaProposta;
            }

            if (quantidadeParecerista > 0)
                await InserirNaBase(PropostaMock.GerarPareceristas(proposta.Id, quantidadeParecerista));

            var tutores = PropostaMock.GerarTutor(proposta.Id);
            if (tutores != null)
            {
                foreach (var tutor in tutores)
                {
                    await InserirNaBase(tutor);

                    var tutorTurmas = PropostaMock.GerarTutorTurmas(tutor.Id, turmas);
                    await InserirNaBase(tutorTurmas);

                    tutor.Turmas = tutorTurmas;
                }

                proposta.Tutores = tutores;
            }

            var regentes = PropostaMock.GerarRegente(proposta.Id);
            if (regentes != null)
            {
                foreach (var regente in regentes)
                {
                    await InserirNaBase(regente);

                    var regenteTurmas = PropostaMock.GerarRegenteTurmas(regente.Id, turmas);
                    await InserirNaBase(regenteTurmas);

                    regente.Turmas = regenteTurmas;
                }

                proposta.Regentes = regentes;
            }

            var propostaTiposInscricao = PropostaMock.GerarTiposInscricao(proposta.Id, tipoInscricao);
            await InserirNaBase(propostaTiposInscricao);

            return proposta;
        }

        protected async Task<IEnumerable<Dominio.Entidades.Proposta>> InserirNaBaseProposta(int quantidade,
            Dominio.Entidades.AreaPromotora areaPromotora, IEnumerable<Dominio.Entidades.CargoFuncao> cargosFuncoes,
            IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao, IEnumerable<PalavraChave> palavrasChaves,
            IEnumerable<Dominio.Enumerados.Modalidade> modalidades, IEnumerable<Dominio.Entidades.AnoTurma> anosTurmas,
            IEnumerable<Dominio.Entidades.ComponenteCurricular> componentesCurriculares)
        {
            var lista = new List<Dominio.Entidades.Proposta>();

            for (int i = 0; i < quantidade; i++)
                lista.Add(await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,
                    palavrasChaves, modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.Cadastrada,
                    FormacaoHomologada.Sim, TipoInscricao.Automatica, true, false, true));

            return lista;
        }

        protected void ValidarPropostaRegenteDTO(PropostaRegenteDTO regenteDto, long id)
        {
            var propostaRegente = ObterPorId<PropostaRegente, long>(id);
            propostaRegente.NomeRegente.ShouldBe(regenteDto.NomeRegente.ToUpper());
            propostaRegente.RegistroFuncional.ShouldBe(regenteDto.RegistroFuncional);
            propostaRegente.MiniBiografia.ShouldBe(regenteDto.MiniBiografia);
            propostaRegente.ProfissionalRedeMunicipal.ShouldBe(regenteDto.ProfissionalRedeMunicipal);
        }

        protected void ValidarPropostaTutorDTO(PropostaTutorDTO tutorDto, long id)
        {
            var propostaTutor = ObterPorId<PropostaTutor, long>(id);
            propostaTutor.NomeTutor.ShouldBe(tutorDto.NomeTutor.ToUpper());
            propostaTutor.RegistroFuncional.ShouldBe(tutorDto.RegistroFuncional);
            propostaTutor.ProfissionalRedeMunicipal.ShouldBe(tutorDto.ProfissionalRedeMunicipal);
        }

        protected void ValidarPropostaDTO(PropostaDTO propostaDTO, long id)
        {
            var proposta = ObterPorId<Dominio.Entidades.Proposta, long>(id);

            proposta.TipoFormacao.ShouldBe(propostaDTO.TipoFormacao);
            proposta.Formato.ShouldBe(propostaDTO.Formato);
            proposta.NomeFormacao.ShouldBe(propostaDTO.NomeFormacao);
            proposta.QuantidadeTurmas.ShouldBe(propostaDTO.QuantidadeTurmas);
            proposta.QuantidadeVagasTurma.ShouldBe(propostaDTO.QuantidadeVagasTurma);
            proposta.Justificativa.ShouldBe(propostaDTO.Justificativa);
            proposta.Objetivos.ShouldBe(propostaDTO.Objetivos);
            proposta.ConteudoProgramatico.ShouldBe(propostaDTO.ConteudoProgramatico);
            proposta.ProcedimentoMetadologico.ShouldBe(propostaDTO.ProcedimentoMetadologico);
            proposta.Referencia.ShouldBe(propostaDTO.Referencia);
            proposta.FormacaoHomologada.ShouldBe(propostaDTO.FormacaoHomologada);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaPresencial))
                proposta.CargaHorariaPresencial.ShouldBe(propostaDTO.CargaHorariaPresencial);

            proposta.DataRealizacaoInicio?.Date.ShouldBeEquivalentTo(propostaDTO.DataRealizacaoInicio?.Date);
            proposta.DataRealizacaoFim?.Date.ShouldBeEquivalentTo(propostaDTO.DataRealizacaoFim?.Date);

            proposta.DataInscricaoInicio?.Date.ShouldBeEquivalentTo(propostaDTO.DataInscricaoInicio?.Date);
            proposta.DataInscricaoFim?.Date.ShouldBeEquivalentTo(propostaDTO.DataInscricaoFim?.Date);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaSincrona))
                proposta.CargaHorariaSincrona.ShouldBe(propostaDTO.CargaHorariaSincrona);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaDistancia))
                proposta.CargaHorariaDistancia.ShouldBe(propostaDTO.CargaHorariaDistancia);

            if (!string.IsNullOrEmpty(propostaDTO.FuncaoEspecificaOutros))
                proposta.FuncaoEspecificaOutros.ShouldBe(propostaDTO.FuncaoEspecificaOutros);

            if (!string.IsNullOrEmpty(propostaDTO.CriterioValidacaoInscricaoOutros))
                proposta.CriterioValidacaoInscricaoOutros.ShouldBe(propostaDTO.CriterioValidacaoInscricaoOutros);

            proposta.Situacao.ShouldBe(propostaDTO.Situacao);

            if (propostaDTO.TiposInscricao.NaoEhNulo() && 
                propostaDTO.TiposInscricao.Any(tipo => tipo.TipoInscricao == TipoInscricao.Externa))
                proposta.LinkParaInscricoesExterna.ShouldBe(propostaDTO.LinkParaInscricoesExterna);

            proposta.CodigoEventoSigpec.ShouldBe(propostaDTO.CodigoEventoSigpec);
        }

        protected void ValidarPropostaCompletoDTO(PropostaCompletoDTO propostaDTO, long id)
        {
            var proposta = ObterPorId<Dominio.Entidades.Proposta, long>(id);

            proposta.TipoFormacao.ShouldBe(propostaDTO.TipoFormacao);
            proposta.Formato.ShouldBe(propostaDTO.Formato);
            proposta.NomeFormacao.ShouldBe(propostaDTO.NomeFormacao);
            proposta.QuantidadeTurmas.ShouldBe(propostaDTO.QuantidadeTurmas);
            proposta.QuantidadeVagasTurma.ShouldBe(propostaDTO.QuantidadeVagasTurma);
            proposta.Justificativa.ShouldBe(propostaDTO.Justificativa);
            proposta.Objetivos.ShouldBe(propostaDTO.Objetivos);
            proposta.ConteudoProgramatico.ShouldBe(propostaDTO.ConteudoProgramatico);
            proposta.ProcedimentoMetadologico.ShouldBe(propostaDTO.ProcedimentoMetadologico);
            proposta.Referencia.ShouldBe(propostaDTO.Referencia);
            proposta.FormacaoHomologada.ShouldBe(propostaDTO.FormacaoHomologada);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaPresencial))
                proposta.CargaHorariaPresencial.ShouldBe(propostaDTO.CargaHorariaPresencial);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaSincrona))
                proposta.CargaHorariaSincrona.ShouldBe(propostaDTO.CargaHorariaSincrona);

            if (!string.IsNullOrEmpty(propostaDTO.CargaHorariaDistancia))
                proposta.CargaHorariaDistancia.ShouldBe(propostaDTO.CargaHorariaDistancia);

            if (!string.IsNullOrEmpty(propostaDTO.FuncaoEspecificaOutros))
                proposta.FuncaoEspecificaOutros.ShouldBe(propostaDTO.FuncaoEspecificaOutros);

            if (!string.IsNullOrEmpty(propostaDTO.CriterioValidacaoInscricaoOutros))
                proposta.CriterioValidacaoInscricaoOutros.ShouldBe(propostaDTO.CriterioValidacaoInscricaoOutros);

            proposta.Situacao.ShouldBe(propostaDTO.Situacao);


            if (propostaDTO.TiposInscricao.NaoEhNulo() &&
                propostaDTO.TiposInscricao.Any(tipo => tipo.TipoInscricao == TipoInscricao.Externa))
                proposta.LinkParaInscricoesExterna.ShouldBe(propostaDTO.LinkParaInscricoesExterna);

            proposta.CodigoEventoSigpec.ShouldBe(propostaDTO.CodigoEventoSigpec);
            proposta.NumeroHomologacao.ShouldBe(propostaDTO.NumeroHomologacao);
        }

        protected void ValidarPropostaCriterioValidacaoInscricaoDTO(IEnumerable<PropostaCriterioValidacaoInscricaoDTO> criteriosDTO, long id)
        {
            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>().Where(t => !t.Excluido);

            if (criteriosDTO.PossuiElementos() && criterioValidacaoInscricaos.PossuiElementos())
                criteriosDTO.Count().ShouldBe(criterioValidacaoInscricaos.Count());

            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
            {
                criterioValidacaoInscricao.PropostaId.ShouldBe(id);
                criteriosDTO.FirstOrDefault(t => t.CriterioValidacaoInscricaoId == criterioValidacaoInscricao.CriterioValidacaoInscricaoId).ShouldNotBeNull();
            }
        }


        protected void ValidarPropostaCriterioCertificacaoDTO(IEnumerable<CriterioCertificacaoDTO> criteriosCertificacoesDTO, long id)
        {
            var criterioCertificoes = ObterTodos<PropostaCriterioCertificacao>().Where(t => !t.Excluido);

            if (criterioCertificoes.PossuiElementos() && criteriosCertificacoesDTO.PossuiElementos())
                criterioCertificoes.Count().ShouldBe(criteriosCertificacoesDTO.Count());

            foreach (var criterioCertificacao in criterioCertificoes)
            {
                criterioCertificacao.PropostaId.ShouldBe(id);
                criteriosCertificacoesDTO.FirstOrDefault(t => t.CriterioCertificacaoId == criterioCertificacao.CriterioCertificacaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaVagaRemanecenteDTO(IEnumerable<PropostaVagaRemanecenteDTO> vagasRemanecentesDTO, long id)
        {
            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>().Where(t => !t.Excluido);

            if (vagasRemanecentesDTO.PossuiElementos() && vagasRemanecentes.PossuiElementos())
                vagasRemanecentesDTO.Count().ShouldBe(vagasRemanecentes.Count());

            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                vagaRemanecente.PropostaId.ShouldBe(id);
                vagasRemanecentesDTO.FirstOrDefault(t => t.CargoFuncaoId == vagaRemanecente.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaFuncaoEspecificaDTO(IEnumerable<PropostaFuncaoEspecificaDTO> funcoesEspecificaDTO, long id)
        {
            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>().Where(t => !t.Excluido);
            funcoesEspecificaDTO.Count().ShouldBe(funcoesEspecificas.Count());

            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                funcaoEspecifica.PropostaId.ShouldBe(id);
                funcoesEspecificaDTO.FirstOrDefault(t => t.CargoFuncaoId == funcaoEspecifica.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        protected void ValidarPropostaPublicoAlvoDTO(IEnumerable<PropostaPublicoAlvoDTO> publicosAlvoDTO, long id)
        {
            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>().Where(t => !t.Excluido);

            if (publicosAlvoDTO.PossuiElementos() && publicosAlvo.PossuiElementos())
                publicosAlvoDTO.Count().ShouldBe(publicosAlvo.Count());

            foreach (var publicoAlvo in publicosAlvo)
            {
                publicoAlvo.PropostaId.ShouldBe(id);
                publicosAlvoDTO.FirstOrDefault(t => t.CargoFuncaoId == publicoAlvo.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        protected static void ValidarAuditoriaDTO(Dominio.EntidadeBaseAuditavel entidadeAuditavel, AuditoriaDTO auditoriaDTO)
        {
            auditoriaDTO.CriadoEm.Date.ShouldBe(entidadeAuditavel.CriadoEm.Date);
            auditoriaDTO.CriadoEm.Hour.ShouldBe(entidadeAuditavel.CriadoEm.Hour);
            auditoriaDTO.CriadoPor.ShouldBe(entidadeAuditavel.CriadoPor);
            auditoriaDTO.CriadoLogin.ShouldBe(entidadeAuditavel.CriadoLogin);

            auditoriaDTO.AlteradoEm.GetValueOrDefault().Date.ShouldBe(entidadeAuditavel.AlteradoEm.GetValueOrDefault().Date);
            auditoriaDTO.AlteradoEm.GetValueOrDefault().Hour.ShouldBe(entidadeAuditavel.AlteradoEm.GetValueOrDefault().Hour);
            auditoriaDTO.AlteradoPor.ShouldBe(entidadeAuditavel.AlteradoPor);
            auditoriaDTO.AlteradoLogin.ShouldBe(entidadeAuditavel.AlteradoLogin);
        }

        protected void ValidarPropostaPalavrasChavesDTO(IEnumerable<PropostaPalavraChaveDTO> propostaPalavrasChavesDTO, long id)
        {
            var propostaPalavraChaves = ObterTodos<PropostaPalavraChave>().Where(t => !t.Excluido);

            if (propostaPalavrasChavesDTO.PossuiElementos() && propostaPalavraChaves.PossuiElementos())
                propostaPalavrasChavesDTO.Count().ShouldBe(propostaPalavraChaves.Count());

            foreach (var palavraChave in propostaPalavraChaves)
            {
                palavraChave.PropostaId.ShouldBe(id);
                propostaPalavrasChavesDTO.Any(t => t.PalavraChaveId == palavraChave.PalavraChaveId).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaTurmasDTO(IEnumerable<PropostaTurmaDTO> propostaTurmaDTO, long id)
        {
            var propostaTurmas = ObterTodos<PropostaTurma>().Where(t => !t.Excluido);

            if (propostaTurmas.PossuiElementos() && propostaTurmaDTO.PossuiElementos())
                propostaTurmas.Count().ShouldBe(propostaTurmaDTO.Count());

            foreach (var propostaTurma in propostaTurmas)
            {
                propostaTurma.PropostaId.ShouldBe(id);
                propostaTurmaDTO.Any(t => t.Nome.ToUpper() == propostaTurma.Nome.ToUpper()).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaTurmasDTO(IEnumerable<PropostaTurmaCompletoDTO> propostaTurmaCompletoDTO, long id)
        {
            var propostaTurmas = ObterTodos<PropostaTurma>().Where(t => !t.Excluido);

            if (propostaTurmas.PossuiElementos() && propostaTurmaCompletoDTO.PossuiElementos())
                propostaTurmas.Count().ShouldBe(propostaTurmaCompletoDTO.Count());

            foreach (var propostaTurma in propostaTurmas)
            {
                propostaTurma.PropostaId.ShouldBe(id);
                propostaTurmaCompletoDTO.Any(t => t.Nome.ToUpper() == propostaTurma.Nome.ToUpper()).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaTurmasDresDTO(IEnumerable<PropostaTurmaDTO> propostaTurmaDTO)
        {
            var propostaTurmas = ObterTodos<PropostaTurma>().Where(t => !t.Excluido).ToList();
            var propostaTurmaDres = ObterTodos<PropostaTurmaDre>().Where(t => !t.Excluido);
            var propostaTurmaCandidatas = propostaTurmaDTO.ToList();

            for (int i = 0; i < propostaTurmaCandidatas.Count(); i++)
            {
                var propostaTurmasDresInseridas = propostaTurmaDres.Where(f => f.PropostaTurmaId == propostaTurmas[0].Id);
                propostaTurmasDresInseridas.Count().ShouldBe(propostaTurmaCandidatas[i].DresIds.Count());

                foreach (var dreInserida in propostaTurmasDresInseridas)
                    propostaTurmaCandidatas[i].DresIds.Any(a => a == dreInserida.DreId).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaModalidadesDTO(IEnumerable<PropostaModalidadeDTO> propostaModalidadeDTO, long id)
        {
            var propostaModalidades = ObterTodos<PropostaModalidade>().Where(t => !t.Excluido);

            if (propostaModalidades.PossuiElementos() && propostaModalidadeDTO.PossuiElementos())
                propostaModalidades.Count().ShouldBe(propostaModalidadeDTO.Count());

            foreach (var propostaModalidade in propostaModalidades)
            {
                propostaModalidade.PropostaId.ShouldBe(id);
                propostaModalidadeDTO.Any(t => t.Modalidade == propostaModalidade.Modalidade).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaAnosTurmasDTO(IEnumerable<PropostaAnoTurmaDTO> propostaAnosTurmasDTO, long id)
        {
            var propostaAnoTurmas = ObterTodos<PropostaAnoTurma>().Where(t => !t.Excluido);

            if (propostaAnoTurmas.PossuiElementos() && propostaAnosTurmasDTO.PossuiElementos())
                propostaAnoTurmas.Count().ShouldBe(propostaAnosTurmasDTO.Count());

            foreach (var propostaAnoTurma in propostaAnoTurmas)
            {
                propostaAnoTurma.PropostaId.ShouldBe(id);
                propostaAnosTurmasDTO.Any(t => t.AnoTurmaId == propostaAnoTurma.AnoTurmaId).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaComponentesCurricularesDTO(IEnumerable<PropostaComponenteCurricularDTO> propostaComponenteCurricularDTO, long id)
        {
            var propostaComponentesCurriculares = ObterTodos<PropostaComponenteCurricular>().Where(t => !t.Excluido);

            if (propostaComponenteCurricularDTO.PossuiElementos() && propostaComponentesCurriculares.PossuiElementos())
                propostaComponenteCurricularDTO.Count().ShouldBe(propostaComponentesCurriculares.Count());

            foreach (var propostaComponenteCurricular in propostaComponentesCurriculares)
            {
                propostaComponenteCurricular.PropostaId.ShouldBe(id);
                propostaComponenteCurricularDTO.Any(t => t.ComponenteCurricularId == propostaComponenteCurricular.ComponenteCurricularId).ShouldBeTrue();
            }
        }

        protected void ValidarPropostaTipoInscricaoDTO(IEnumerable<PropostaTipoInscricaoDTO> tiposInscricaoDto, long id)
        {
            var tiposInscricao = ObterTodos<PropostaTipoInscricao>().Where(t => !t.Excluido);

            if (tiposInscricaoDto.PossuiElementos() && tiposInscricao.PossuiElementos())
                tiposInscricaoDto.Count().ShouldBe(tiposInscricao.Count());

            foreach (var tipoInscricao in tiposInscricao)
            {
                tipoInscricao.PropostaId.ShouldBe(id);
                tiposInscricaoDto.FirstOrDefault(t => t.TipoInscricao == tipoInscricao.TipoInscricao).ShouldNotBeNull();
            }
        }
    }
}