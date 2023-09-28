using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
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

        [Fact(DisplayName = "Proposta - Deve alterar rascunho sem informação preenchida")]
        public async Task Deve_alterar_proposta_rascunho_sem_informacao_preenchida()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = PropostaMock.GerarPropostaRascunho(areaPromotora.Id);
            await InserirNaBase(proposta);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Modalidade.Presencial,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                palavrasChavesDTO,
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
        }

        [Fact(DisplayName = "Proposta - Deve alterar proposta válida")]
        public async Task Deve_alterar_proposta_valida()
        {
            //arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Modalidade.Presencial,
                cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id }),
                cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id }),
                criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id }),
                cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id }),
                palavrasChaves.Select(t => new PropostaPalavraChaveDTO { PalavraChaveId = t.Id }),
                SituacaoProposta.Ativo);

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
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção para campos obrigatórios")]
        public async Task Deve_retornar_excecao_campos_obrigatorios()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoProposta.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É necessário informar o tipo de formação para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar a modalidade para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o tipo de inscrição para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o público alvo para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o tipo de inscrição para alterar a propostaproposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar a justificativa para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar os objetivos para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar o conteúdo programático para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar os procedimentos metadológicos para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar a referência para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É necessário informar as palavras-chaves para alterar a proposta").ShouldBeTrue();
             // excecao.Mensagens.Contains("É necessário informar no mínimo 3 palavras-chaves para alterar a proposta").ShouldBeTrue();
             // excecao.Mensagens.Contains("É necessário informar no máximo 5 palavras-chaves para alterar a proposta").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando o tipo de formação for evento e modalidade hibrida")]
        public async Task Deve_alterar_proposta_tipo_formacao_evento_e_modalidade_hibrido_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO { PalavraChaveId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Evento,
               Modalidade.Hibrido,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               palavrasChavesDTO,
               SituacaoProposta.Ativo);

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
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando o tipo de formação for curso e modalidade hibrida")]
        public async Task Deve_retornar_excecao_tipo_formacao_curso_e_modalidade_hibrido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Hibrido,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               palavrasChavesDTO,
               SituacaoProposta.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É permitido a modalidade Híbrido somente para o tipo de formação evento").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando função especificas outros estiver habilitado e vazio")]
        public async Task Deve_retornar_excecao_funcoes_especificas_outros_habilitado_vazio()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var funcaoEspecifica = CargoFuncaoMock.GerarCargoFuncao(CargoFuncaoTipo.Funcao, true);
            await InserirNaBase(funcaoEspecifica);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = funcaoEspecifica.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Distancia,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               palavrasChavesDTO,
               SituacaoProposta.Ativo);

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

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var funcaoEspecifica = CargoFuncaoMock.GerarCargoFuncao(CargoFuncaoTipo.Funcao, true);
            await InserirNaBase(funcaoEspecifica);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao,palavrasChaves);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = funcaoEspecifica.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            
            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Distancia,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               palavrasChavesDTO,
               SituacaoProposta.Ativo, gerarFuncaoEspecificaOutros: true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(publicosAlvoDTO, id);
            ValidarPropostaVagaRemanecenteDTO(vagasRemanecentesDTO, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(criteriosDTO, id);
            ValidarPropostaPalavrasChavesDTO(palavrasChavesDTO, id);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando critério validação inscrição outros estiver habilitado")]
        public async Task Deve_retornar_excecao_criterios_validacao_inscricao_outros_habilitado_vazio()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoInscricao);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null, palavrasChaves);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoInscricao.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            
            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Presencial,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               palavrasChavesDTO,
               SituacaoProposta.Ativo);

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

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoOutro = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoOutro);
            
            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(100);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null,palavrasChaves);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoOutro.Id } };
            var palavrasChavesDTO = palavrasChaves.Select(t => new PropostaPalavraChaveDTO() { PalavraChaveId = t.Id });
            
            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Presencial,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               palavrasChavesDTO,
               SituacaoProposta.Ativo, gerarCriterioValidacaoInscricaoOutros: true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarPropostaDTO(propostaDTO, id);
            ValidarPropostaPublicoAlvoDTO(publicosAlvoDTO, id);
            ValidarPropostaVagaRemanecenteDTO(vagasRemanecentesDTO, id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(criteriosDTO, id);
            ValidarPropostaPalavrasChavesDTO(palavrasChavesDTO, id);
        }
    }
}
