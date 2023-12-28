using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
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

        [Fact(DisplayName = "Inscrição - Deve retornar os dados do usuario logado para inscrição via cargo")]
        public async Task Deve_retornar_os_dados_do_usuario_para_inscricao_via_cargo()
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

            var inscricaoDto = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                CargoDreCodigo = 1,
                CargoUeCodigo = 2,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault(),
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            await casoDeUso.Executar(inscricaoDto);
            var inscricaoInserida = ObterTodos<Dominio.Entidades.Inscricao>().Where(t => !t.Excluido).FirstOrDefault();
            
            inscricaoInserida.PropostaTurmaId.ShouldBe(inscricaoDto.PropostaTurmaId);
            inscricaoInserida.CargoDreCodigo.ShouldBe(inscricaoDto.CargoDreCodigo.Value);
            inscricaoInserida.CargoUeCodigo.ShouldBe(inscricaoDto.CargoUeCodigo.Value);
            inscricaoInserida.CargoCodigo.ShouldBe(inscricaoDto.CargoCodigo.Value);
            inscricaoInserida.CargoId.ShouldNotBeNull();
            inscricaoInserida.CargoId.Value.ShouldBeGreaterThan(0);
            
            inscricaoInserida.FuncaoId.ShouldBeNull();
            inscricaoInserida.FuncaoCodigo.ShouldBeNull();
            inscricaoInserida.FuncaoDreCodigo.ShouldBeNull();
            inscricaoInserida.FuncaoUeCodigo.ShouldBeNull();
        }
        
        [Fact(DisplayName = "Inscrição - Deve retornar os dados do usuario logado para inscrição via função")]
        public async Task Deve_retornar_os_dados_do_usuario_para_inscricao_via_funcao()
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

            var inscricaoDto = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                FuncaoDreCodigo = 1,
                FuncaoUeCodigo = 2,
                FuncaoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoFuncoes.FirstOrDefault(),
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            await casoDeUso.Executar(inscricaoDto);
            var inscricaoInserida = ObterTodos<Dominio.Entidades.Inscricao>().Where(t => !t.Excluido).FirstOrDefault();
            
            inscricaoInserida.PropostaTurmaId.ShouldBe(inscricaoDto.PropostaTurmaId);
            inscricaoInserida.FuncaoDreCodigo.ShouldBe(inscricaoDto.FuncaoDreCodigo.Value);
            inscricaoInserida.FuncaoUeCodigo.ShouldBe(inscricaoDto.FuncaoUeCodigo.Value);
            inscricaoInserida.FuncaoCodigo.ShouldBe(inscricaoDto.FuncaoCodigo.Value);
            inscricaoInserida.FuncaoId.ShouldNotBeNull();
            inscricaoInserida.FuncaoId.Value.ShouldBeGreaterThan(0);
            
            inscricaoInserida.CargoId.ShouldBeNull();
            inscricaoInserida.CargoCodigo.ShouldBeNull();
            inscricaoInserida.CargoDreCodigo.ShouldBeNull();
            inscricaoInserida.CargoUeCodigo.ShouldBeNull();
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

            var inscricaoDto = new InscricaoDTO
            {
                PropostaTurmaId = proposta.Turmas.FirstOrDefault().Id,
                CargoDreCodigo = 1,
                CargoUeCodigo = 2,
                CargoCodigo = AoObterDadosUsuarioInscricaoMock.CodigoCargos.FirstOrDefault(),
                Email = usuario.Email,
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricao>();

            // act
            var excecao = await Should.ThrowAsync<NegocioException>( () => casoDeUso.Executar(inscricaoDto));

            // assert
            excecao.Mensagens.Contains(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA).ShouldBeTrue();
        }
    }
}
