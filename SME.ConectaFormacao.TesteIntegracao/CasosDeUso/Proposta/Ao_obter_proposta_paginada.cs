﻿using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_obter_proposta_paginada : TestePropostaBase
    {
        public Ao_obter_proposta_paginada(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada com filtro")]
        public async Task Deve_retornar_registros_com_filtro()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var propostas = await InserirNaBaseProposta(15);
            await InserirNaBase(propostas);

            var propostaFiltro = propostas.FirstOrDefault();

            var filtro = PropostaPaginacaoMock.GerarPropostaFiltrosDTOValido(propostas.FirstOrDefault().AreaPromotora, propostaFiltro);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(1);
            var propostaPesquisada = retorno.Items.FirstOrDefault();

            propostaPesquisada.TipoFormacao.ShouldBe(propostaFiltro.TipoFormacao.GetValueOrDefault().Nome());
            propostaPesquisada.AreaPromotora.ShouldBe(propostaFiltro.AreaPromotora.Nome);
            propostaPesquisada.Formato.ShouldBe(propostaFiltro.Formato.GetValueOrDefault().Nome());
            propostaPesquisada.NomeFormacao.ShouldBe(propostaFiltro.NomeFormacao);
            propostaPesquisada.NumeroHomologacao.ShouldBe(propostaFiltro.NumeroHomologacao.GetValueOrDefault());
            propostaPesquisada.DataRealizacaoInicio.ShouldBe(ObterDataFormatada(propostaFiltro?.DataRealizacaoInicio));
            propostaPesquisada.DataRealizacaoFim.ShouldBe(ObterDataFormatada(propostaFiltro?.DataRealizacaoFim));
            propostaPesquisada.Situacao.ShouldBe(propostaFiltro.Situacao.Nome());
            propostaPesquisada.FormacaoHomologada.ShouldBe(propostaFiltro.FormacaoHomologada.GetValueOrDefault());
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada sem filtro")]
        public async Task Deve_retornar_registros_sem_filtros()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            await InserirNaBaseProposta(15);

            var filtro = new PropostaFiltrosDTO();

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeTrue();
            retorno.TotalPaginas.ShouldBe(2);
        }

        [Fact(DisplayName = "Proposta - Não deve retornar registros consulta paginada filtros invalidos")]
        public async Task Nao_deve_retornar_registros_filtros_invalidos()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            await InserirNaBaseProposta(15);

            var filtro = PropostaPaginacaoMock.GerarPropostaFiltrosDTOInvalido();
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();

            // act
            var retorno = await casoDeUso.Executar(filtro);

            // assert
            retorno.Items.Any().ShouldBeFalse();
        }

        [Fact(DisplayName = "Proposta - Deve retornar registros consulta paginada para perfil logado parecerista")]
        public async Task Deve_retornar_registros_para_perfil_logado_parecerista()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.PARECERISTA, usuario.Login);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.AguardandoAnalisePeloParecerista);

            var parecerista = new PropostaParecerista
            {
                PropostaId = proposta.Id,
                NomeParecerista = $"Parecerista {usuario.Nome}",
                RegistroFuncional = usuario.Login,
                CriadoPor = proposta.CriadoPor,
                CriadoEm = proposta.CriadoEm,
                CriadoLogin = proposta.CriadoLogin
            };

            await InserirNaBase(parecerista);

            // act
            var casoDeUso = ObterCasoDeUso<ICasoDeUsoObterPropostaPaginacao>();
            var retorno = await casoDeUso.Executar(new PropostaFiltrosDTO());

            // assert
            retorno.TotalRegistros.ShouldBe(1);
            retorno.Items.FirstOrDefault().Id.ShouldBe(proposta.Id);
        }

        private void AdicionarPerfilUsuarioContextoAplicacao(Guid perfil, string login)
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
                {
                    { "PerfilUsuario", perfil.ToString() },
                    { "UsuarioLogado", login },
                };

            contextoAplicacao.AdicionarVariaveis(variaveis);
        }

        private string ObterDataFormatada(DateTime? data)
        {
            return data.HasValue ? data.Value.ToString("dd/MM/yyyy") : string.Empty;
        }
    }
}
