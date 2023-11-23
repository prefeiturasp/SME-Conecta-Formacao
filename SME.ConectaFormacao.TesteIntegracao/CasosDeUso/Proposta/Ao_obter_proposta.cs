﻿using Shouldly;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta : TestePropostaBase
    {
        public Ao_obter_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve obter por id válido")]
        public async Task Deve_obter_por_id_valido()
        {
            // arrange
            var areaPromotora = AreaPromotoraMock.GerarAreaPromotora(PropostaSalvarMock.GrupoUsuarioLogadoId);
            await InserirNaBase(areaPromotora);

            var cargosFuncoes = CargoFuncaoMock.GerarCargoFuncao(10);
            await InserirNaBase(cargosFuncoes);

            var criteriosValidacaoInscricao = CriterioValidacaoInscricaoMock.GerarCriterioValidacaoInscricao(5);
            await InserirNaBase(criteriosValidacaoInscricao);

            var palavrasChaves = PalavraChaveMock.GerarPalavrasChaves(10);
            await InserirNaBase(palavrasChaves);

            var proposta = await InserirNaBaseProposta(areaPromotora, cargosFuncoes, criteriosValidacaoInscricao, palavrasChaves);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var propostaCompletoDTO = await casoDeUso.Executar(proposta.Id);

            // assert 
            propostaCompletoDTO.ShouldNotBeNull();
            ValidarPropostaCompletoDTO(propostaCompletoDTO, proposta.Id);
            ValidarPropostaPublicoAlvoDTO(propostaCompletoDTO.PublicosAlvo, proposta.Id);
            ValidarPropostaFuncaoEspecificaDTO(propostaCompletoDTO.FuncoesEspecificas, proposta.Id);
            ValidarPropostaVagaRemanecenteDTO(propostaCompletoDTO.VagasRemanecentes, proposta.Id);
            ValidarPropostaCriterioValidacaoInscricaoDTO(propostaCompletoDTO.CriteriosValidacaoInscricao, proposta.Id);
            ValidarPropostaPalavrasChavesDTO(propostaCompletoDTO.PalavrasChaves, proposta.Id);
            ValidarAuditoriaDTO(proposta, propostaCompletoDTO.Auditoria);
        }

        [Fact(DisplayName = "Proposta - Deve retornar exceção ao obter por id inválido")]
        public async Task Deve_retornar_excecao_ao_obter_por_id_invalido()
        {
            // arrange 
            var idAleatorio = PropostaSalvarMock.GerarIdAleatorio();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPorId>();

            // act 
            var excecao = await Should.ThrowAsync<NegocioException>(casoDeUso.Executar(idAleatorio));

            // assert 
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA).ShouldBeTrue();
        }
    }
}
