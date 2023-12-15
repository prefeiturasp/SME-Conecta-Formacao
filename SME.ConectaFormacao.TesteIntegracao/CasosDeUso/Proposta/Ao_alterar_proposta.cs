using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_alterar_proposta : TestePropostaBase
    {
        public Ao_alterar_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarFakes(IServiceCollection services)
        {
            base.RegistrarFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve alterar rascunho sem informação preenchida")]
        public async Task Deve_alterar_proposta_rascunho_sem_informacao_preenchida()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

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

            var proposta = PropostaMock.GerarPropostaRascunho(areaPromotora.Id);
            await InserirNaBase(proposta);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Rascunho);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta válida")]
        public async Task Deve_alterar_proposta_valida()
        {
            //arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dres.Select(t => new PropostaDreDTO { DreId = t.Id }),
                cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id }),
                cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id }),
                criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id }),
                cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id }),
                palavrasChaves.Select(t => new PropostaPalavraChaveDTO { PalavraChaveId = t.Id }),
                modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t }),
                anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id }),
                componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO { ComponenteCurricularId = t.Id }),
                SituacaoProposta.Cadastrada, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.Turmas.FirstOrDefault().Id = proposta.Turmas.FirstOrDefault().Id;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção para campos obrigatórios")]
        public async Task Deve_retornar_excecao_campos_obrigatorios()
        {
            // arrange
            var proposta = await InserirNaBaseProposta();

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoProposta.Cadastrada);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É necessário informar o tipo de formação para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o formato para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar a dre para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o tipo de inscrição para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o tipo de inscrição para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar a justificativa para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar os objetivos para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o conteúdo programático para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar os procedimentos metadológicos para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar a referência para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar as palavras-chaves para alterar a proposta").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando o tipo de formação for evento e formato hibrida")]
        public async Task Deve_alterar_proposta_tipo_formacao_evento_e_formato_hibrido_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Evento,
                Formato.Hibrido,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando o tipo de formação for curso e formato hibrido")]
        public async Task Deve_retornar_excecao_tipo_formacao_curso_e_formato_hibrido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Hibrido,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É permitido o formato Híbrido somente para o tipo de formação evento").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando função especificas outros estiver habilitado e vazio")]
        public async Task Deve_retornar_excecao_funcoes_especificas_outros_habilitado_vazio()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var funcaoEspecifica = CargoFuncaoMock.GerarCargoFuncao(CargoFuncaoTipo.Funcao, true);
            await InserirNaBase(funcaoEspecifica);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = funcaoEspecifica.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Distancia,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada);

            propostaDTO.FuncaoEspecificaOutros = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_FUNCAO_ESPECIFICA_OUTROS).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando função especificas outros for válido")]
        public async Task Deve_alterar_proposta_funcao_especifica_outros_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var funcaoEspecifica = CargoFuncaoMock.GerarCargoFuncao(CargoFuncaoTipo.Funcao, true);
            await InserirNaBase(funcaoEspecifica);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = funcaoEspecifica.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Distancia,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarFuncaoEspecificaOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando critério validação inscrição outros estiver habilitado")]
        public async Task Deve_retornar_excecao_criterios_validacao_inscricao_outros_habilitado_vazio()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoInscricao.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada);

            propostaDTO.FuncaoEspecificaOutros = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_CRITERIO_VALIDACAO_INSCRICAO_OUTROS).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando critério validação inscrição outros for válido")]
        public async Task Deve_alterar_proposta_criterio_validacao_inscricao_outros_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarCriterioValidacaoInscricaoOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Não deve alterar quando os campos Público Alvo, Funções Específicas, Modalidade, Ano Turma e Componente Curricular não forem preenchidos")]
        public async Task Nao_deve_alterar_proposta_criterio_validacao_publico_alvo_funcoes_especificas_modalidade_ano_turma_componente_curricular_não_forem_preenchidos()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = Enumerable.Empty<PropostaPublicoAlvoDTO>();
            var funcoesEspecificaDTO = Enumerable.Empty<PropostaFuncaoEspecificaDTO>();
            var vagasRemanecentesDTO = Enumerable.Empty<PropostaVagaRemanecenteDTO>();
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = Enumerable.Empty<PropostaModalidadeDTO>();
            var anosTurmasDTO = Enumerable.Empty<PropostaAnoTurmaDTO>();
            var componentesCurricularesDTO = Enumerable.Empty<PropostaComponenteCurricularDTO>();

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarCriterioValidacaoInscricaoOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É necessário informar Público Alvo ou Função Específica ou Modalidade, Ano da Turma e Componente Curricular da proposta").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Não deve alterar quando os campos Modalidade, Ano Turma e Componente Curricular estiverem parcialmente preenchidos")]
        public async Task Nao_deve_alterar_quando_os_campos_modalidade_ano_turma_e_componente_estiverem_parcialmente_preenchidos()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = Enumerable.Empty<PropostaPublicoAlvoDTO>();
            var funcoesEspecificaDTO = Enumerable.Empty<PropostaFuncaoEspecificaDTO>();
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = Enumerable.Empty<PropostaComponenteCurricularDTO>();

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarCriterioValidacaoInscricaoOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É necessário informar Público Alvo ou Função Específica ou Modalidade, Ano da Turma e Componente Curricular da proposta").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando for preenchido somente Público Alvo e os campos: Funções Específicas, Modalidade, Ano da Turma e Componente Curricular omitidos")]
        public async Task Deve_alterar_quando_for_preenchido_somente_publico_alvo_e_os_campos_fucoes_especificas_modalidade_ano_da_turma_e_componente_curricular_omitidos()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = Enumerable.Empty<PropostaFuncaoEspecificaDTO>();
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = Enumerable.Empty<PropostaModalidadeDTO>();
            var anosTurmasDTO = Enumerable.Empty<PropostaAnoTurmaDTO>();
            var componentesCurricularesDTO = Enumerable.Empty<PropostaComponenteCurricularDTO>();

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarCriterioValidacaoInscricaoOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando for preenchido somente Funções Específicas e os campos: Público Alvo, Modalidade, Ano da Turma e Componente Curricular omitidos")]
        public async Task Deve_alterar_quando_for_preenchido_somente_funcoes_especificas_e_os_campos_publico_alvo_modalidade_ano_da_turma_e_componente_curricular_omitidos()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = Enumerable.Empty<PropostaPublicoAlvoDTO>();
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = Enumerable.Empty<PropostaModalidadeDTO>();
            var anosTurmasDTO = Enumerable.Empty<PropostaAnoTurmaDTO>();
            var componentesCurricularesDTO = Enumerable.Empty<PropostaComponenteCurricularDTO>();

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarCriterioValidacaoInscricaoOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando for preenchido somente Modalidade, Ano Turma e Componente Curricular e os campos: Público Alvo e Funções Específicas omitidos")]
        public async Task Deve_alterar_quando_for_preenchido_somente_modalidade_ano_da_turma_e_componente_curricular_e_os_campos_publico_alvo_e_funcoes_especificas_omitidos()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var dres = DreMock.GerarDreValida(5);
            await InserirNaBase(dres);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var modalidades = Enum.GetValues(typeof(Dominio.Enumerados.Modalidade)).Cast<Dominio.Enumerados.Modalidade>();

            var anosTurmas = AnoTurmaMock.GerarAnoTurma(1);
            await InserirNaBase(anosTurmas);

            var componentesCurriculares = ComponenteCurricularMock.GerarComponenteCurricular(10, anosTurmas.FirstOrDefault().Id);
            await InserirNaBase(componentesCurriculares);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves
                , modalidades, anosTurmas, componentesCurriculares);

            var dreDTO = dres.Select(t => new PropostaDreDTO { DreId = t.Id });
            var publicosAlvoDTO = Enumerable.Empty<PropostaPublicoAlvoDTO>();
            var funcoesEspecificaDTO = Enumerable.Empty<PropostaFuncaoEspecificaDTO>();
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            var modalidadesDTO = modalidades.Select(t => new PropostaModalidadeDTO { Modalidade = t });
            var anosTurmasDTO = anosTurmas.Select(t => new PropostaAnoTurmaDTO { AnoTurmaId = t.Id });
            var componentesCurricularesDTO = componentesCurriculares.Select(t => new PropostaComponenteCurricularDTO() { ComponenteCurricularId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Formato.Presencial,
                dreDTO,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
                modalidadesDTO,
                anosTurmasDTO,
                componentesCurricularesDTO,
                SituacaoProposta.Cadastrada, gerarCriterioValidacaoInscricaoOutros: true, quantidadeTurmas: proposta.QuantidadeTurmas);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, id);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, id);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, id);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, id);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, id);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, id);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, id);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, id);
        }
    }
}