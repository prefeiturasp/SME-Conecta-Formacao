using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using Xunit;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao
{
    public class Ao_realizar_inscricao_manual : TestePropostaBase
    {
        public Ao_realizar_inscricao_manual(CollectionFixture collectionFixture) : base(collectionFixture)
        {
        }

        protected override void RegistrarQueryFakes(IServiceCollection services)
        {
            base.RegistrarQueryFakes(services);
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterCargosFuncoesDresFuncionarioServicoEolQuery, IEnumerable<CursistaCargoServicoEol>>), typeof(ObterCargosFuncoesDresFuncionarioServicoEolQueryHandlerFaker), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IRequestHandler<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery, IEnumerable<DreUeAtribuicaoServicoEol>>), typeof(ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryHandlerFake), ServiceLifetime.Scoped));
        }

        [Fact(DisplayName = "Inscrição - Deve realizar inscrição manual com sucesso")]
        public async Task Deve_realizar_inscricao_manual_com_sucesso()
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

            var inscricao = new InscricaoManualDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                ProfissionalRede = true,
                RegistroFuncional = usuario.Login,
                Cpf = usuario.Cpf,
                PodeContinuar = true,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault().ToString(),
                CargoDreCodigo = proposta.Turmas.FirstOrDefault().Dres.FirstOrDefault().Dre.Codigo,
                CargoUeCodigo = "094765",
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricaoManual>();

            // act
            var retorno = await casoDeUso.Executar(inscricao);

            // assert
            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICAO_MANUAL_REALIZADA_COM_SUCESSO);
        }

        [Fact(DisplayName = "Inscrição - Inscrição fora do período de inscrição")]
        public async Task Ao_realizar_inscricao_fora_do_periodo()
        {
            // arrange
            var usuario = UsuarioMock.GerarUsuario();
            await InserirNaBase(usuario);

            var proposta = await InserirNaBaseProposta(Dominio.Enumerados.SituacaoProposta.Publicada, Dominio.Enumerados.FormacaoHomologada.NaoCursosPorIN, Dominio.Enumerados.TipoInscricao.Automatica, false, true, true, true);

            var CargosFuncoes = ObterTodos<Dominio.Entidades.CargoFuncao>();

            var depara = CargoFuncaoDeparaEolMock.GerarCargoFuncaoDeparaEol(CargosFuncoes);
            await InserirNaBase(depara);

            var vagas = PropostaMock.GerarTurmaVagas(proposta.Turmas, proposta.QuantidadeVagasTurma.GetValueOrDefault());
            await InserirNaBase(vagas);

            AoObterDadosUsuarioInscricaoMock.Usuario = usuario;
            AoObterDadosUsuarioInscricaoMock.CodigoCargos = depara.Where(t => t.CodigoCargoEol.HasValue).Select(s => s.CodigoCargoEol.GetValueOrDefault()).ToArray();
            AoObterDadosUsuarioInscricaoMock.CodigoFuncoes = depara.Where(t => t.CodigoFuncaoEol.HasValue).Select(s => s.CodigoFuncaoEol.GetValueOrDefault()).ToArray();

            var inscricao = new InscricaoManualDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                ProfissionalRede = true,
                RegistroFuncional = usuario.Login,
                Cpf = usuario.Cpf,
                PodeContinuar = true,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault().ToString(),
                CargoDreCodigo = proposta.Turmas.FirstOrDefault().Dres.FirstOrDefault().Dre.Codigo,
                CargoUeCodigo = "094765",
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricaoManual>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>(() => casoDeUso.Executar(inscricao));

            // assert
            excecao.ShouldNotBeNull();
            excecao.Mensagens.Contains(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO).ShouldBeTrue();
        }
    }
}
