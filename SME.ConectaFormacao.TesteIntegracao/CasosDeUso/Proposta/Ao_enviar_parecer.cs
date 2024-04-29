﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Contexto;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta
{
    public class Ao_enviar_parecer : TestePropostaBase
    {
        public Ao_enviar_parecer(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        [Fact(DisplayName = "Proposta - Deve enviar o parecer do parecerista")]
        public async Task Ao_enviar_parecer_parecerista()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.PARECERISTA, usuario.Login);

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseParecerista);

            var usuarioParecer = PropostaParecerMock.GerarPropostaParecer();
            usuarioParecer.UsuarioPareceristaId = usuario.Id;
            usuarioParecer.Situacao = SituacaoParecer.PendenteEnvioParecerPeloParecerista;
            usuarioParecer.PropostaId = proposta.Id;

            await InserirNaBase(usuarioParecer);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecer>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceres = ObterTodos<PropostaParecer>();

            pareceres.Any(parecer => parecer.Situacao == SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF).ShouldBeTrue();

            var propostaConsulta = ObterTodos< Dominio.Entidades.Proposta>().FirstOrDefault();

            propostaConsulta.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseDf);
        }

        [Fact(DisplayName = "Proposta - Deve enviar o parecer do parecerista com parecerista pendente")]
        public async Task Ao_enviar_parecer_parecerista_com_parecerista_pendente()
        {
            // arrange
            var usuario1 = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario1);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.PARECERISTA, usuario1.Login);

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseParecerista);

            var usuarioParecer = PropostaParecerMock.GerarPropostaParecer();
            usuarioParecer.UsuarioPareceristaId = usuario1.Id;
            usuarioParecer.Situacao = SituacaoParecer.PendenteEnvioParecerPeloParecerista;
            usuarioParecer.PropostaId = proposta.Id;

            await InserirNaBase(usuarioParecer);

            var usuario2 = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario2);

            var usuarioParecer2 = PropostaParecerMock.GerarPropostaParecer();
            usuarioParecer2.UsuarioPareceristaId = usuario2.Id;
            usuarioParecer2.Situacao = SituacaoParecer.PendenteEnvioParecerPeloParecerista;
            usuarioParecer2.PropostaId = proposta.Id;

            await InserirNaBase(usuarioParecer2);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecer>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceres = ObterTodos<PropostaParecer>();

            pareceres.Any(parecer => parecer.Situacao == SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF).ShouldBeTrue();

            var propostaConsulta = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();

            propostaConsulta.Situacao.ShouldBe(SituacaoProposta.AguardandoAnaliseParecerista);
        }

        [Fact(DisplayName = "Proposta - Deve enviar o parecer do admin DF")]
        public async Task Ao_enviar_parecer_admin_DF()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            AdicionarPerfilUsuarioContextoAplicacao(Perfis.ADMIN_DF, usuario.Login);

            var proposta = await InserirNaBaseProposta(situacao: SituacaoProposta.AguardandoAnaliseDf);

            var usuarioParecer = PropostaParecerMock.GerarPropostaParecer();
            usuarioParecer.UsuarioPareceristaId = usuario.Id;
            usuarioParecer.Situacao = SituacaoParecer.AguardandoAnaliseParecerPeloAdminDF;
            usuarioParecer.PropostaId = proposta.Id;

            await InserirNaBase(usuarioParecer);

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoEnviarPropostaParecer>();

            // act 
            await casoDeUso.Executar(proposta.Id);

            // assert
            var pareceres = ObterTodos<PropostaParecer>();

            pareceres.Any(parecer => parecer.Situacao == SituacaoParecer.AguardandoAnaliseParecerPelaAreaPromotora).ShouldBeTrue();

            var propostaConsulta = ObterTodos<Dominio.Entidades.Proposta>().FirstOrDefault();

            propostaConsulta.Situacao.ShouldBe(SituacaoProposta.AnaliseParecerAreaPromotora);
        }


        private void AdicionarPerfilUsuarioContextoAplicacao(Guid perfil, string login)
        {
            var contextoAplicacao = ServiceProvider.GetService<IContextoAplicacao>();
            var variaveis = new Dictionary<string, object>
                {
                    { "PerfilUsuario", perfil.ToString() },
                    { "UsuarioLogado",  login }
                };

            contextoAplicacao.AdicionarVariaveis(variaveis);
        }
    }


}