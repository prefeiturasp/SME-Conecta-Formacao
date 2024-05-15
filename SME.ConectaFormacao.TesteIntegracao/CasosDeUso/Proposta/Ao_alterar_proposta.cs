using Bogus;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
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
            await InserirParametrosProposta();
            
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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta válida")]
        public async Task Deve_alterar_proposta_valida()
        {
            //arrange
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta publicada válida sem inscrição automática ")]
        public async Task Deve_alterar_proposta_publicada_valida_sem_inscricao_automatica()
        {
            //arrange
            await InserirParametrosProposta();

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
                modalidades, anosTurmas, componentesCurriculares, situacao: SituacaoProposta.Publicada, tipoInscricao: TipoInscricao.Optativa);

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
                SituacaoProposta.Publicada, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.Turmas.FirstOrDefault().Id = proposta.Turmas.FirstOrDefault().Id;
            propostaDTO.TiposInscricao = new List<PropostaTipoInscricaoDTO>() { new PropostaTipoInscricaoDTO() { TipoInscricao = TipoInscricao.Optativa } };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);
            retornoDto.Mensagem.Contains(MensagemNegocio.PROPOSTA_PUBLICADA_ALTERADA).ShouldBeTrue();
            retornoDto.Mensagem.Contains(MensagemNegocio.PROPOSTA_PUBLICADA_ALTERADA_COM_INSCRICAO_AUTOMATICA).ShouldBeFalse();

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta publicada válida com inscrição automática ")]
        public async Task Deve_alterar_proposta_publicada_valida_com_inscricao_automatica()
        {
            //arrange
            await InserirParametrosProposta();

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
                modalidades, anosTurmas, componentesCurriculares, situacao: SituacaoProposta.Publicada, tipoInscricao: TipoInscricao.Automatica);

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
                SituacaoProposta.Publicada, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.Turmas.FirstOrDefault().Id = proposta.Turmas.FirstOrDefault().Id;
            propostaDTO.TiposInscricao = new List<PropostaTipoInscricaoDTO>() { new PropostaTipoInscricaoDTO() { TipoInscricao = TipoInscricao.Automatica } };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);
            retornoDto.Mensagem.Contains(MensagemNegocio.PROPOSTA_PUBLICADA_ALTERADA).ShouldBeTrue();
            retornoDto.Mensagem.Contains(MensagemNegocio.PROPOSTA_PUBLICADA_ALTERADA_COM_INSCRICAO_AUTOMATICA).ShouldBeTrue();

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
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
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
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
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaFuncaoEspecificaDTO(propostaDTO.FuncoesEspecificas, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
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
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
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
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando for preenchido somente Funções Específicas e os campos: Público Alvo, Modalidade, Ano da Turma e Componente Curricular omitidos")]
        public async Task Deve_alterar_quando_for_preenchido_somente_funcoes_especificas_e_os_campos_publico_alvo_modalidade_ano_da_turma_e_componente_curricular_omitidos()
        {
            // arrange
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando for preenchido somente Modalidade, Ano Turma e Componente Curricular e os campos: Público Alvo e Funções Específicas omitidos")]
        public async Task Deve_alterar_quando_for_preenchido_somente_modalidade_ano_da_turma_e_componente_curricular_e_os_campos_publico_alvo_e_funcoes_especificas_omitidos()
        {
            // arrange
            await InserirParametrosProposta();

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
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
            ValidarPropostaPublicoAlvoDTO(propostaDTO.PublicosAlvo, retornoDto.EntidadeId);
            ValidarPropostaVagaRemanecenteDTO(propostaDTO.VagasRemanecentes, retornoDto.EntidadeId);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaDTO.CriteriosValidacaoInscricao, retornoDto.EntidadeId);
            ValidarPropostaCriterioCertificacaoDTO(propostaDTO.CriterioCertificacao, retornoDto.EntidadeId);
            ValidarPropostaPalavrasChavesDTO(propostaDTO.PalavrasChaves, retornoDto.EntidadeId);
            ValidarPropostaTurmasDTO(propostaDTO.Turmas, retornoDto.EntidadeId);
            ValidarPropostaTurmasDresDTO(propostaDTO.Turmas);
            ValidarPropostaModalidadesDTO(propostaDTO.Modalidades, retornoDto.EntidadeId);
            ValidarPropostaAnosTurmasDTO(propostaDTO.AnosTurmas, retornoDto.EntidadeId);
            ValidarPropostaComponentesCurricularesDTO(propostaDTO.ComponentesCurriculares, retornoDto.EntidadeId);
            ValidarPropostaTipoInscricaoDTO(propostaDTO.TiposInscricao, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção para campo obrigatório tipo externo")]
        public async Task Deve_retornar_excecao_campo_obrigatorio_tipo_externo()
        {
            //arrange
            await InserirParametrosProposta();

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

            propostaDTO.TiposInscricao = new List<PropostaTipoInscricaoDTO>() { new PropostaTipoInscricaoDTO { TipoInscricao = TipoInscricao.Externa } };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É necessário informar o link para inscrições  para inserir a proposta").ShouldBeTrue();
        }


        [Fact(DisplayName = "Proposta - Deve alterar proposta válida do tipo inscrição externo")]
        public async Task Deve_alterar_proposta_valida_tipo_inscricao_externo()
        {
            //arrange
            await InserirParametrosProposta();

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

            propostaDTO.TiposInscricao = new List<PropostaTipoInscricaoDTO>() { new PropostaTipoInscricaoDTO { TipoInscricao = TipoInscricao.Externa } };

            propostaDTO.LinkParaInscricoesExterna = new Faker().Lorem.Sentence(25);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, retornoDto.EntidadeId);
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta aprovada homologada em publicada")]
        public async Task Deve_alterar_proposta_aprovada_homologada_em_publicada()
        {
            //arrange
            await InserirParametrosProposta();

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
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.Aprovada);

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
                SituacaoProposta.Aprovada, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            propostaDTO.NumeroHomologacao =  new Random().NextInt64(100000, 9999999999);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            var propostaAlterada = ObterPorId<Dominio.Entidades.Proposta, long>(retornoDto.EntidadeId);

            propostaAlterada.Situacao.ShouldBe(SituacaoProposta.Publicada);
            propostaAlterada.NumeroHomologacao.ShouldBe(propostaDTO.NumeroHomologacao);
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta aprovada homologada sem número de homologação")]
        public async Task Deve_alterar_proposta_aprovada_homologada_sem_numero_homologacao()
        {
            //arrange
            await InserirParametrosProposta();

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
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.Aprovada);

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
                SituacaoProposta.Aprovada, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            var propostaAlterada = ObterPorId<Dominio.Entidades.Proposta, long>(retornoDto.EntidadeId);

            propostaAlterada.Situacao.ShouldBe(SituacaoProposta.Aprovada);
            propostaAlterada.NumeroHomologacao.ShouldBeNull();
        }
        
        [Fact(DisplayName = "Proposta - Deve alterar proposta quando adicionado parecistas")]
        public async Task Deve_alterar_proposta_quando_adicionado_parecistas()
        {
            //arrange
            await InserirParametrosProposta();

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
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.Aprovada);

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
                SituacaoProposta.AguardandoAnaliseDf, quantidadeTurmas: proposta.QuantidadeTurmas, quantidadePareceristas:3);

            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var retornoDto = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retornoDto.EntidadeId.ShouldBeGreaterThan(0);

            var propostaAlterada = ObterPorId<Dominio.Entidades.Proposta, long>(retornoDto.EntidadeId);
            propostaAlterada.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseDf);
            
            var pareceristasInseridos = ObterTodos<PropostaParecerista>();
            pareceristasInseridos.All(a=> a.NomeParecerista.EstaPreenchido()).ShouldBeTrue();
            pareceristasInseridos.All(a=> a.Situacao.EstaAguardandoValidacao()).ShouldBeTrue();
            pareceristasInseridos.All(a=> a.Justificativa.NaoEstaPreenchido()).ShouldBeTrue();
            pareceristasInseridos.All(a=> a.RegistroFuncional.EstaPreenchido()).ShouldBeTrue();
            pareceristasInseridos.All(a=> a.NomeParecerista.EstaPreenchido()).ShouldBeTrue();
            pareceristasInseridos.All(a=> a.PropostaId == 1).ShouldBeTrue();
            pareceristasInseridos.Count().ShouldBe(3);
        }
        
        [Fact(DisplayName = "Proposta - Não deve alterar proposta quando adicionado mais pareceristas que o permitido conforme parâmetro")]
        public async Task Deve_alterar_proposta_quando_adicionado_mais_parecistas_que_o_permitido_conforme_parametro()
        {
            //arrange
            await InserirParametrosProposta();

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
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.Aprovada);

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
                SituacaoProposta.Aprovada, quantidadeTurmas: proposta.QuantidadeTurmas,quantidadePareceristas:4);
            
            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains(string.Format(MensagemNegocio.LIMITE_PARECERISTAS_EXCEDIDO_LIMITE_X, 3)).ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Proposta - Deve desativar parecerista que foram excluídos e que possuem considerações quando a situação da proposta está aguardando análise do parecerista")]
        public async Task Deve_desativar_parecerista_que_foram_excluidos_e_que_possuem_consideracoes_quando_a_situacao_da_proposta_esta_aguardando_analise_do_parecerista()
        {
            //arrange
            await InserirParametrosProposta();

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
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Parecerista3");

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AguardandoAnalisePeloParecerista);

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1", "Parecerista1", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.AguardandoValidacao));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));
            
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
                SituacaoProposta.AguardandoAnalisePeloParecerista, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.Pareceristas = new List<PropostaPareceristaDTO>() { new () { Id = 1, RegistroFuncional = "1", NomeParecerista = "Parecerista1" } };
            
            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retorno.ShouldNotBeNull();

            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.Count(a=> a.Situacao.EstaDesativado()).ShouldBe(2);
            pareceristas.Count(a=> a.Situacao.EstaAguardandoValidacao()).ShouldBe(1);
        }
        
        [Fact(DisplayName = "Proposta - Deve desativar e excluir pareceristas que foram excluídos e que não possuem considerações quando a situação da proposta está aguardando análise do parecerista")]
        public async Task Deve_desativar_e_excluir_pareceristas_que_foram_excluidos_e_que_nao_possuem_consideracoes_quando_a_situacao_da_proposta_esta_aguardando_analise_do_parecerista()
        {
            //arrange
            await InserirParametrosProposta();

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
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Parecerista3");

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AguardandoAnalisePeloParecerista);

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1", "Parecerista1", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.AguardandoValidacao));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.AguardandoValidacao));
            
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
                SituacaoProposta.AguardandoAnalisePeloParecerista, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.Pareceristas = new List<PropostaPareceristaDTO>() { new () { Id = 1, RegistroFuncional = "1", NomeParecerista = "Parecerista1" } };
            
            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retorno.ShouldNotBeNull();
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.Count(a=> a.Situacao.EstaDesativado() && a.Excluido).ShouldBe(2);
            pareceristas.Count(a=> a.Situacao.EstaAguardandoValidacao()).ShouldBe(1);
        }
        
        [Fact(DisplayName = "Proposta - Deve remover e desativar pareceristas que possuem considerações, adicionar novo parecerista como reavalidação e parecerista está aguardando revalidação")]
        public async Task Deve_remover_e_desativar_pareceristas_que_possuem_consideracoes_adicionar_novo_parecerista_como_reavalidacao_e_parecerista_esta_aguardando_revalidacao()
        {
            //arrange
            await InserirParametrosProposta();

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
            
            await InserirUsuario("1", "Parecerista1");
            await InserirUsuario("2", "Parecerista2");
            await InserirUsuario("3", "Parecerista3");
            await InserirUsuario("4", "Parecerista4");

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves,
                modalidades, anosTurmas, componentesCurriculares, SituacaoProposta.AguardandoReanalisePeloParecerista);

            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "1", "Parecerista1", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "2","Parecerista2", SituacaoParecerista.Enviada));
            await InserirNaBase(PropostaPareceristaMock.GerarPropostaParecerista(proposta.Id, "3","Parecerista3", SituacaoParecerista.Enviada));
            
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.Formato, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.FormacaoHomologada, "1"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(1,CampoConsideracao.TipoFormacao, "1"));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.TiposInscricao, "2"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(2,CampoConsideracao.IntegrarNoSGA, "2"));

            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.Dres, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.NomeFormacao, "3"));
            await InserirNaBase(PropostaPareceristaConsideracaoMock.GerarPropostaPareceristaConsideracao(3,CampoConsideracao.PublicosAlvo, "3"));
            
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
                SituacaoProposta.AguardandoReanalisePeloParecerista, quantidadeTurmas: proposta.QuantidadeTurmas);

            propostaDTO.Pareceristas = new List<PropostaPareceristaDTO>()
            {
                new () { Id = 1, RegistroFuncional = "1", NomeParecerista = "Parecerista1" },
                new () { RegistroFuncional = "4", NomeParecerista = "Parecerista4" }
            };
            
            propostaDTO.FormacaoHomologada = FormacaoHomologada.Sim;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var retorno = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            retorno.ShouldNotBeNull();
            var pareceristas = ObterTodos<PropostaParecerista>();
            pareceristas.Count(a=> a.Situacao.EstaDesativado()).ShouldBe(2);
            pareceristas.Count(a=> a.Situacao.EstaAguardandoRevalidacao()).ShouldBe(1);
            pareceristas.Count(a=> a.Situacao.EstaEnviada()).ShouldBe(1);
        }
    }
}