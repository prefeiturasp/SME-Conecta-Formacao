using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.AreaPromotora.Mock;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.CargoFuncao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_alterar_proposta : TesteBase
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

            var proposta = PropostaMock.GerarPropostaRascunho(areaPromotora.Id);
            await InserirNaBase(proposta);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Modalidade.Presencial,
                publicosAlvoDTO,
                funcoesEspecificaDTO,
                criteriosDTO,
                vagasRemanecentesDTO,
                SituacaoRegistro.Rascunho);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecifica(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecente(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricao(propostaDTO.CriteriosValidacaoInscricao, id);
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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
                TipoFormacao.Curso,
                Modalidade.Presencial,
                cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id }),
                cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id }),
                criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id }),
                cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id }),
                SituacaoRegistro.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecifica(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecente(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricao(propostaDTO.CriteriosValidacaoInscricao, id);
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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoRegistro.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É nescessário informar o tipo de formação para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar a modalidade para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar o tipo de inscrição para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar o público alvo para alterar a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar os critérios de validação das inscrições para alterar a proposta").ShouldBeTrue();
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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Evento,
               Modalidade.Hibrido,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoRegistro.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(propostaDTO.PublicosAlvo, id);
            ValidarPropostaFuncaoEspecifica(propostaDTO.FuncoesEspecificas, id);
            ValidarPropostaVagaRemanecente(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricao(propostaDTO.CriteriosValidacaoInscricao, id);
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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Hibrido,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoRegistro.Ativo);

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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = (long)OpcaoListagem.Outros } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Hibrido,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoRegistro.Ativo);

            propostaDTO.FuncaoEspecificaOutros = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É nescessário informar função específicas outros para alterar a proposta").ShouldBeTrue();
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

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = (long)OpcaoListagem.Outros } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Distancia,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoRegistro.Ativo, gerarFuncaoEspecificaOutros: true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(publicosAlvoDTO, id);
            ValidarPropostaVagaRemanecente(vagasRemanecentesDTO, id);
            ValidarPropostaCriterioValidacaoInscricao(criteriosDTO, id);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção quando critério validação inscrição outros estiver habilitado")]
        public async Task Deve_retornar_excecao_criterios_validacao_inscricao_outros_habilitado_vazio()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = (long)OpcaoListagem.Outros } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Hibrido,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoRegistro.Ativo);

            propostaDTO.FuncaoEspecificaOutros = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(proposta.Id, propostaDTO));

            // assert
            excecao.Mensagens.Contains("É nescessário informar critérios de validação das inscrições outros para alterar a proposta").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve alterar quando critério validação inscrição outros for válido")]
        public async Task Deve_alterar_proposta_criterio_validacao_inscricao_outros_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, null);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = (long)OpcaoListagem.Outros } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Presencial,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoRegistro.Ativo, gerarCriterioValidacaoInscricaoOutros: true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoAlterarProposta>();

            // act 
            var id = await casoDeUso.Executar(proposta.Id, propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(publicosAlvoDTO, id);
            ValidarPropostaVagaRemanecente(vagasRemanecentesDTO, id);
            ValidarPropostaCriterioValidacaoInscricao(criteriosDTO, id);
        }

        private async Task<Dominio.Entidades.Proposta> InserirNaBaseProposta(Dominio.Entidades.AreaPromotora areaPromotora, IEnumerable<Dominio.Entidades.CargoFuncao> cargosFuncoes, IEnumerable<CriterioValidacaoInscricao> criteriosValidacaoInscricao)
        {
            var proposta = PropostaMock.GerarPropostaValida(
                areaPromotora.Id,
                TipoFormacao.Curso,
                Modalidade.Presencial,
                SituacaoRegistro.Ativo,
                false, false);

            await InserirNaBase(proposta);


            var publicosAlvo = PropostaMock.GerarPublicoAlvo(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo));
            if (publicosAlvo != null)
                await InserirNaBase(publicosAlvo);

            var funcoesEspecifica = PropostaMock.GerarFuncoesEspecificas(proposta.Id, cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao));
            if (funcoesEspecifica != null)
                await InserirNaBase(funcoesEspecifica);

            var vagasRemanecentes = PropostaMock.GerarVagasRemanecentes(proposta.Id, cargosFuncoes);
            if (vagasRemanecentes != null)
                await InserirNaBase(vagasRemanecentes);

            var criterios = PropostaMock.GerarCritariosValidacaoInscricao(proposta.Id, criteriosValidacaoInscricao);
            if (criterios != null)
                await InserirNaBase(criterios);

            return proposta;
        }

        private void ValidarProposta(PropostaDTO propostaDTO, long id)
        {
            var proposta = ObterPorId<Dominio.Entidades.Proposta, long>(id);

            proposta.TipoFormacao.ShouldBe(propostaDTO.TipoFormacao);
            proposta.Modalidade.ShouldBe(propostaDTO.Modalidade);
            proposta.TipoInscricao.ShouldBe(propostaDTO.TipoInscricao);
            proposta.NomeFormacao.ShouldBe(propostaDTO.NomeFormacao);
            proposta.QuantidadeTurmas.ShouldBe(propostaDTO.QuantidadeTurmas);
            proposta.QuantidadeVagasTurma.ShouldBe(propostaDTO.QuantidadeVagasTurma);

            if (!string.IsNullOrEmpty(propostaDTO.FuncaoEspecificaOutros))
                proposta.FuncaoEspecificaOutros.ShouldBe(propostaDTO.FuncaoEspecificaOutros);

            if (!string.IsNullOrEmpty(propostaDTO.CriterioValidacaoInscricaoOutros))
                proposta.CriterioValidacaoInscricaoOutros.ShouldBe(propostaDTO.CriterioValidacaoInscricaoOutros);

            proposta.Situacao.ShouldBe(propostaDTO.Situacao);
        }

        private void ValidarPropostaCriterioValidacaoInscricao(IEnumerable<PropostaCriterioValidacaoInscricaoDTO> criteriosDTO, long id)
        {
            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>().Where(t => !t.Excluido);
            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
            {
                criterioValidacaoInscricao.PropostaId.ShouldBe(id);
                criteriosDTO.FirstOrDefault(t => t.CriterioValidacaoInscricaoId == criterioValidacaoInscricao.CriterioValidacaoInscricaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaVagaRemanecente(IEnumerable<PropostaVagaRemanecenteDTO> vagasRemanecentesDTO, long id)
        {
            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>().Where(t => !t.Excluido);
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                vagaRemanecente.PropostaId.ShouldBe(id);
                vagasRemanecentesDTO.FirstOrDefault(t => t.CargoFuncaoId == vagaRemanecente.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaFuncaoEspecifica(IEnumerable<PropostaFuncaoEspecificaDTO> funcoesEspecificaDTO, long id)
        {
            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>().Where(t => !t.Excluido);
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                funcaoEspecifica.PropostaId.ShouldBe(id);
                funcoesEspecificaDTO.FirstOrDefault(t => t.CargoFuncaoId == funcaoEspecifica.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaPublicoAlvo(IEnumerable<PropostaPublicoAlvoDTO> publicosAlvoDTO, long id)
        {
            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>().Where(t => !t.Excluido);
            foreach (var publicoAlvo in publicosAlvo)
            {
                publicoAlvo.PropostaId.ShouldBe(id);
                publicosAlvoDTO.FirstOrDefault(t => t.CargoFuncaoId == publicoAlvo.CargoFuncaoId).ShouldNotBeNull();
            }
        }
    }
}
