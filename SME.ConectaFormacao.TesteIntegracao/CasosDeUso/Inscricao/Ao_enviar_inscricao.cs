﻿using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_enviar_inscricao : TestePropostaBase
    {
        public Ao_enviar_inscricao(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterUsuarioLogadoQuery, Dominio.Entidades.Usuario>), typeof(ObterUsuarioLogadoQueryHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CargoFuncionarioConectaDTO>>), typeof(ObterCargosFuncoesDresFuncionarioServicoEolQueryHandlerFaker), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Inscrição - Deve retornar os dados do usuario logado para inscrição")]
        public async Task Deve_retornar_os_dados_do_usuario_para_inscricao()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var inscricao = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault().ToString(),
                CargoDreCodigo = proposta.Turmas.FirstOrDefault().Dres.FirstOrDefault().Dre.Codigo,
                CargoUeCodigo = "094765",
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            await casoDeUso.Executar(inscricao);
            var inscricaoInserida = (ObterTodos<Dominio.Entidades.Inscricao>()).FirstOrDefault();
            inscricaoInserida.CargoCodigo.ShouldBe(inscricao.CargoCodigo);
            inscricaoInserida.CargoDreCodigo.ShouldBe(inscricao.CargoDreCodigo);
            inscricaoInserida.CargoUeCodigo.ShouldBe(inscricao.CargoUeCodigo);
        }

        [Fact(DisplayName = "Inscrição - Deve retornar exceção vagas indisponivel")]
        public async Task Deve_retornar_excecao_vagas_indisponivel()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var inscricao = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault().ToString(),
                CargoDreCodigo = string.Empty,
                CargoUeCodigo = string.Empty,
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(inscricao));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA).ShouldBeTrue();
        }

        [Fact(DisplayName = "Inscrição - Deve permitir inscrição para código cargo eol com mais de um mapeamento DePara")]
        public async Task Deve_permitir_inscricao_para_codigo_cargo_eol_com_mais_de_um_mapeamento_de_para()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN, vincularUltimoCargoAoPublicoAlvo: true);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            var codigoCargoEolPrimeiro = depara.FirstOrDefault(s => s.CodigoCargoEol.NaoEhNulo()).CodigoCargoEol;
            var codigoCargoEolDuplicado = depara.LastOrDefault(s => s.CodigoCargoEol.NaoEhNulo());
            codigoCargoEolDuplicado.CodigoCargoEol = codigoCargoEolPrimeiro;
            await InserirNaBase(depara);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var inscricao = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault().ToString(),
                CargoDreCodigo = proposta.Turmas.FirstOrDefault().Dres.FirstOrDefault().Dre.Codigo,
                CargoUeCodigo = "094765",
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            await casoDeUso.Executar(inscricao);
            var inscricaoInserida = (ObterTodos<Dominio.Entidades.Inscricao>()).FirstOrDefault();
            inscricaoInserida.CargoCodigo.ShouldBe(inscricao.CargoCodigo);
            inscricaoInserida.CargoDreCodigo.ShouldBe(inscricao.CargoDreCodigo);
            inscricaoInserida.CargoUeCodigo.ShouldBe(inscricao.CargoUeCodigo);
            inscricaoInserida.CargoId.ShouldBe(codigoCargoEolDuplicado.CargoFuncaoId);

            inscricaoInserida.FuncaoCodigo.ShouldBeNull();
            inscricaoInserida.FuncaoDreCodigo.ShouldBeNull();
            inscricaoInserida.FuncaoUeCodigo.ShouldBeNull();
            inscricaoInserida.FuncaoId.ShouldBeNull();
        }

        [Fact(DisplayName = "Inscrição - Deve permitir inscrição para código função eol com mais de um mapeamento DePara")]
        public async Task Deve_permitir_inscricao_para_codigo_funcao_eol_com_mais_de_um_mapeamento_de_para()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN, vincularUltimoFuncaoAoPublicoAlvo: true);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            var codigoFuncaoEolPrimeiro = depara.FirstOrDefault(s => s.CodigoFuncaoEol.NaoEhNulo()).CodigoFuncaoEol;
            var codigoFuncaoEolDuplicado = depara.LastOrDefault(s => s.CodigoFuncaoEol.NaoEhNulo());
            codigoFuncaoEolDuplicado.CodigoFuncaoEol = codigoFuncaoEolPrimeiro;
            await InserirNaBase(depara);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var inscricao = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                FuncaoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoFuncoes.FirstOrDefault().ToString(),
                FuncaoDreCodigo = proposta.Turmas.FirstOrDefault().Dres.FirstOrDefault().Dre.Codigo,
                FuncaoUeCodigo = "094765",
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            await casoDeUso.Executar(inscricao);
            var inscricaoInserida = (ObterTodos<Dominio.Entidades.Inscricao>()).FirstOrDefault();

            inscricaoInserida.FuncaoCodigo.ShouldBe(inscricao.FuncaoCodigo);
            inscricaoInserida.FuncaoDreCodigo.ShouldBe(inscricao.FuncaoDreCodigo);
            inscricaoInserida.FuncaoUeCodigo.ShouldBe(inscricao.FuncaoUeCodigo);
            inscricaoInserida.FuncaoId.ShouldBe(codigoFuncaoEolDuplicado.CargoFuncaoId);

            inscricaoInserida.CargoCodigo.ShouldBeNull();
            inscricaoInserida.CargoDreCodigo.ShouldBeNull();
            inscricaoInserida.CargoUeCodigo.ShouldBeNull();
            inscricaoInserida.CargoId.ShouldBeNull();
        }
    }
}
