﻿using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_listagem_proposta : TestePropostaBase
    {
        public Ao_obter_listagem_proposta(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta sem filtro")]
        public async Task Deve_obter_listagem_proposta_sem_filtro()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var filtro = new FiltroListagemFormacaoDTO();
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();

            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro por titulo")]
        public async Task Deve_obter_listagem_proposta_com_filtro_por_titulo()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                Titulo = propostas.LastOrDefault().NomeFormacao.Substring(0,3)
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro de público alvo")]
        public async Task Deve_obter_listagem_proposta_com_filtro_de_publico_alvo()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var propostaPublicoAlvos = ObterTodos<Dominio.Entidades.PropostaPublicoAlvo>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                PublicosAlvosIds = propostaPublicoAlvos.Take(2).Select(s=> s.CargoFuncaoId).Take(2).ToArray()
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro de áreas promotoras")]
        public async Task Deve_obter_listagem_proposta_com_filtro_de_areas_promotoras()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var areaPromotoras = ObterTodos<Dominio.Entidades.AreaPromotora>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                AreasPromotorasIds = areaPromotoras.Take(2).Select(s=> s.Id).Take(2).ToArray()
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro de data inicial")]
        public async Task Deve_obter_listagem_proposta_com_filtro_de_data_inicial()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                DataInicial = propostas.FirstOrDefault().DataRealizacaoInicio
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro de data final")]
        public async Task Deve_obter_listagem_proposta_com_filtro_de_data_final()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                DataFinal = propostas.FirstOrDefault().DataRealizacaoFim
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro de formatos")]
        public async Task Deve_obter_listagem_proposta_com_filtro_de_formatos()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var propostas = ObterTodos<Dominio.Entidades.Proposta>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                FormatosIds = propostas.Take(2).Select(s=> (int)s.Formato).ToArray()
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
        
        [Fact(DisplayName = "Proposta - Deve obter a listagem de proposta com filtro de palavras chaves")]
        public async Task Deve_obter_listagem_proposta_com_filtro_de_palavras_chaves()
        {
            // arrange
            for (int i = 0; i < 10; i++)
                await InserirNaBaseProposta(ehTipoInscricaoOptativa:true);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterListagemFormacaoPaginada>();
            var propostaPalavraChaves = ObterTodos<Dominio.Entidades.PropostaPalavraChave>();
            var filtro = new FiltroListagemFormacaoDTO()
            {
                PalavrasChavesIds = propostaPalavraChaves.Take(2).Select(s=> s.PalavraChaveId).ToArray()
            };
                
            // act 
            var listagemFormacaoDtos = await casoDeUso.Executar(filtro);

            // assert 
            listagemFormacaoDtos.ShouldNotBeNull();
            
            foreach (var formacao in listagemFormacaoDtos)
            {
                formacao.Id.ShouldBeGreaterThan(0);
                formacao.Titulo.ShouldNotBeEmpty();
                formacao.Periodo.ShouldNotBeEmpty();
                formacao.AreaPromotora.ShouldNotBeEmpty();
                formacao.TipoFormacao.ShouldNotBeEmpty();
                formacao.Formato.ShouldNotBeEmpty();
                formacao.InscricaoEncerrada.ShouldBeTrue();
                formacao.ImagemUrl.ShouldBeNull();
            }
        }
    }
}
