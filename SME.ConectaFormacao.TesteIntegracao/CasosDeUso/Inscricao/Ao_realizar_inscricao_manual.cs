using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes;
using SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta;
using SME.ConectaFormacao.TesteIntegracao.Mocks;
using SME.ConectaFormacao.TesteIntegracao.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Cpf = usuario.Cpf
            };

            var casoDeUso = ObterCasoDeUso<ICasoDeUsoSalvarInscricaoManual>();

            // act
            var retorno = await casoDeUso.Executar(inscricao);

            // assert
            retorno.Mensagem.ShouldBe(MensagemNegocio.INSCRICAO_MANUAL_REALIZADA_COM_SUCESSO);
        }
    }
}
