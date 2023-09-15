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
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_inserir_proposta : TesteBase
    {
        public Ao_inserir_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterGrupoUsuarioLogadoQuery, Guid>), typeof(ObterGrupoUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Proposta - Deve inserir rascunho sem nenhuma informação preenchida")]
        public async Task Deve_inserir_proposta_rascunho_sem_informacao_preenchida()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoProposta.Rascunho);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act 
            var id = await casoDeUso.Executar(propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
        }

        [Fact(DisplayName = "Proposta - Deve inserir proposta válida")]
        public async Task Deve_inserir_proposta_valida()
        {
            //arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

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
                SituacaoProposta.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act 
            var id = await casoDeUso.Executar(propostaDTO);

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

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOVazio(SituacaoProposta.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(propostaDTO));

            // assert
            excecao.Mensagens.Contains("É nescessário informar o tipo de formação para inserir a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar a modalidade para inserir a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar o tipo de inscrição para inserir a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar o público alvo para inserir a proposta").ShouldBeTrue();
            excecao.Mensagens.Contains("É nescessário informar os critérios de validação das inscrições para inserir a proposta").ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve inserir quando o tipo de formação for evento e modalidade hibrida")]
        public async Task Deve_inserir_proposta_tipo_formacao_evento_e_modalidade_hibrido_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

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
               SituacaoProposta.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act 
            var id = await casoDeUso.Executar(propostaDTO);

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
               SituacaoProposta.Ativo);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(propostaDTO));

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

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = funcaoEspecifica.Id } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Distancia,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoProposta.Ativo);

            propostaDTO.FuncaoEspecificaOutros = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(propostaDTO));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_FUNCAO_ESPECIFICA_OUTROS).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve inserir quando função especificas outros for válido")]
        public async Task Deve_inserir_proposta_funcao_especifica_outros_valido()
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

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var criteriosDTO = criteriosValidacaoInscricao.Select(t => new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var funcoesEspecificaDTO = new PropostaFuncaoEspecificaDTO[] { new PropostaFuncaoEspecificaDTO { CargoFuncaoId = funcaoEspecifica.Id } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Distancia,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoProposta.Ativo, gerarFuncaoEspecificaOutros: true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act 
            var id = await casoDeUso.Executar(propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(propostaDTO.PublicosAlvo, id);
            ValidarPropostaVagaRemanecente(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricao(propostaDTO.CriteriosValidacaoInscricao, id);
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

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoInscricao.Id } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Distancia,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoProposta.Ativo);

            propostaDTO.FuncaoEspecificaOutros = string.Empty;

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(propostaDTO));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_CRITERIO_VALIDACAO_INSCRICAO_OUTROS).ShouldBeTrue();
        }

        [Fact(DisplayName = "Proposta - Deve inserir quando critério validação inscrição outros for válido")]
        public async Task Deve_inserir_proposta_criterio_validacao_inscricao_outros_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criterioValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(false, true);
            await InserirNaBase(criterioValidacaoInscricao);

            var publicosAlvoDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Cargo).Select(t => new PropostaPublicoAlvoDTO { CargoFuncaoId = t.Id });
            var funcoesEspecificaDTO = cargosFuncoes.Where(t => t.Tipo == CargoFuncaoTipo.Funcao).Select(t => new PropostaFuncaoEspecificaDTO { CargoFuncaoId = t.Id });
            var vagasRemanecentesDTO = cargosFuncoes.Select(t => new PropostaVagaRemanecenteDTO { CargoFuncaoId = t.Id });

            var criteriosDTO = new PropostaCriterioValidacaoInscricaoDTO[] { new PropostaCriterioValidacaoInscricaoDTO { CriterioValidacaoInscricaoId = criterioValidacaoInscricao.Id } };

            var propostaDTO = PropostaSalvarMock.GerarPropostaDTOValida(
               TipoFormacao.Curso,
               Modalidade.Presencial,
               publicosAlvoDTO,
               funcoesEspecificaDTO,
               criteriosDTO,
               vagasRemanecentesDTO,
               SituacaoProposta.Ativo, gerarCriterioValidacaoInscricaoOutros: true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoInserirProposta>();

            // act 
            var id = await casoDeUso.Executar(propostaDTO);

            // assert
            id.ShouldBeGreaterThan(0);

            ValidarProposta(propostaDTO, id);
            ValidarPropostaPublicoAlvo(propostaDTO.PublicosAlvo, id);
            ValidarPropostaVagaRemanecente(propostaDTO.VagasRemanecentes, id);
            ValidarPropostaCriterioValidacaoInscricao(propostaDTO.CriteriosValidacaoInscricao, id);
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
            var criterioValidacaoInscricaos = ObterTodos<PropostaCriterioValidacaoInscricao>();
            foreach (var criterioValidacaoInscricao in criterioValidacaoInscricaos)
            {
                criterioValidacaoInscricao.PropostaId.ShouldBe(id);
                criteriosDTO.FirstOrDefault(t => t.CriterioValidacaoInscricaoId == criterioValidacaoInscricao.CriterioValidacaoInscricaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaVagaRemanecente(IEnumerable<PropostaVagaRemanecenteDTO> vagasRemanecentesDTO, long id)
        {
            var vagasRemanecentes = ObterTodos<PropostaVagaRemanecente>();
            foreach (var vagaRemanecente in vagasRemanecentes)
            {
                vagaRemanecente.PropostaId.ShouldBe(id);
                vagasRemanecentesDTO.FirstOrDefault(t => t.CargoFuncaoId == vagaRemanecente.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaFuncaoEspecifica(IEnumerable<PropostaFuncaoEspecificaDTO> funcoesEspecificaDTO, long id)
        {
            var funcoesEspecificas = ObterTodos<PropostaFuncaoEspecifica>();
            foreach (var funcaoEspecifica in funcoesEspecificas)
            {
                funcaoEspecifica.PropostaId.ShouldBe(id);
                funcoesEspecificaDTO.FirstOrDefault(t => t.CargoFuncaoId == funcaoEspecifica.CargoFuncaoId).ShouldNotBeNull();
            }
        }

        private void ValidarPropostaPublicoAlvo(IEnumerable<PropostaPublicoAlvoDTO> publicosAlvoDTO, long id)
        {
            var publicosAlvo = ObterTodos<PropostaPublicoAlvo>();
            foreach (var publicoAlvo in publicosAlvo)
            {
                publicoAlvo.PropostaId.ShouldBe(id);
                publicosAlvoDTO.FirstOrDefault(t => t.CargoFuncaoId == publicoAlvo.CargoFuncaoId).ShouldNotBeNull();
            }
        }
    }
}
