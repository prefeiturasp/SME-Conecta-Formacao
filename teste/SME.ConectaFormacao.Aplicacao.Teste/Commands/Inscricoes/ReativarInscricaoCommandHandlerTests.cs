using Bogus;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.ReativarInscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class ReativarInscricaoCommandHandlerTestes
    {
        private readonly AutoMocker _mocker;
        private readonly ReativarInscricaoCommandHandler _handler;
        private readonly Faker _faker;

        public ReativarInscricaoCommandHandlerTestes()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<ReativarInscricaoCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoInscricaoNaoEncontrada_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = new ReativarInscricaoCommand(1);
            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync((Inscricao)null!);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            Assert.Equal(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA, excecao.Message);
        }

        [Fact]
        public async Task DadoInscricaoNaoCancelada_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = new ReativarInscricaoCommand(1);
            var inscricao = new Inscricao { Id = comando.Id, Situacao = SituacaoInscricao.Confirmada };

            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ObterPorId(comando.Id))
                .ReturnsAsync(inscricao);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            Assert.Equal(MensagemNegocio.INSCRICAO_SO_PODE_REATIVAR_CANCELADAS, excecao.Message);
        }

        [Fact]
        public async Task DadoCargoNaoPermitidoNoPublicoAlvo_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = new ReativarInscricaoCommand(1);
            var cargoId = 99;
            var inscricao = CriarInscricaoCancelada(comando.Id, cargoId, "DRE01");
            var propostaTurma = new PropostaTurma { Id = inscricao.PropostaTurmaId, PropostaId = 10 };
            var proposta = CriarPropostaValida(10);

            ConfigurarMocksPrincipais(inscricao, propostaTurma, proposta);

            // Simula Publico Alvo não contendo o cargoId da inscrição
            ConfigurarValidacoes(proposta.Id, inscricao.PropostaTurmaId, cargoId: 50, dreCodigo: "DRE01", cargoValido: false, dreValida: true);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            Assert.Equal(MensagemNegocio.INSCRICAO_CARGO_NAO_PERMITIDO, excecao.Message);
        }

        [Fact]
        public async Task DadoDreNaoPermitida_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = new ReativarInscricaoCommand(1);
            var dreCodigo = "DRE_INVALIDA";
            var inscricao = CriarInscricaoCancelada(comando.Id, 1, dreCodigo);
            var propostaTurma = new PropostaTurma { Id = inscricao.PropostaTurmaId, PropostaId = 10 };
            var proposta = CriarPropostaValida(10);

            ConfigurarMocksPrincipais(inscricao, propostaTurma, proposta);

            // Simula DRE da turma diferente da DRE da inscrição
            ConfigurarValidacoes(proposta.Id, inscricao.PropostaTurmaId, cargoId: 1, dreCodigo: "DRE_VALIDA", cargoValido: true, dreValida: false);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            Assert.Equal(MensagemNegocio.INSCRICAO_DRE_NAO_PERMITIDA, excecao.Message);
        }

        [Fact]
        public async Task DadoPropostaForaDoPeriodoDeInscricao_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var comando = new ReativarInscricaoCommand(1);
            var inscricao = CriarInscricaoCancelada(comando.Id, 1, "DRE01");
            var propostaTurma = new PropostaTurma { Id = inscricao.PropostaTurmaId, PropostaId = 10 };

            // Proposta com datas passadas
            var proposta = new Proposta
            {
                Id = 10,
                DataInscricaoInicio = DateTime.Now.AddDays(-10),
                DataInscricaoFim = DateTime.Now.AddDays(-5)
            };

            ConfigurarMocksPrincipais(inscricao, propostaTurma, proposta);
            ConfigurarValidacoes(proposta.Id, inscricao.PropostaTurmaId, 1, "DRE01", true, true);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            Assert.Equal(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO, excecao.Message);
        }

        // --- Métodos Auxiliares ---

        private Inscricao CriarInscricaoCancelada(long id, long cargoId, string dreCodigo)
        {
            return new Inscricao
            {
                Id = id,
                Situacao = SituacaoInscricao.Cancelada,
                PropostaTurmaId = _faker.Random.Long(1),
                CargoId = cargoId,
                CargoDreCodigo = dreCodigo,
                MotivoCancelamento = "Desistência"
            };
        }

        private static Proposta CriarPropostaValida(long id)
        {
            return new Proposta
            {
                Id = id,
                DataInscricaoInicio = DateTime.Now.AddDays(-1),
                DataInscricaoFim = DateTime.Now.AddDays(1),
                FormacaoHomologada = FormacaoHomologada.NaoCursosPorIN
            };
        }

        private void ConfigurarMocksPrincipais(Inscricao inscricao, PropostaTurma propostaTurma, Proposta proposta)
        {
            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.ObterPorId(inscricao.Id))
                .ReturnsAsync(inscricao);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterPropostaTurmaPorIdQuery>(q => q.PropostaTurmaId == inscricao.PropostaTurmaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(propostaTurma);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == propostaTurma.PropostaId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);
        }

        private void ConfigurarValidacoes(long propostaId, long propostaTurmaId, long cargoId, string dreCodigo, bool cargoValido, bool dreValida)
        {
            // Configura Cargo
            var listaPublicoAlvo = new List<PropostaPublicoAlvo>();
            if (cargoValido)
                listaPublicoAlvo.Add(new PropostaPublicoAlvo { CargoFuncaoId = cargoId });

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPublicoAlvoPorId(propostaId))
                .ReturnsAsync(listaPublicoAlvo);

            // Configura DRE
            var listaTurmaDres = new List<PropostaTurmaDre>();
            if (dreValida)
            {
                listaTurmaDres.Add(new PropostaTurmaDre { DreCodigo = dreCodigo, Dre = new Dre { Todos = false } });
            }
            else
            {
                // Adiciona uma DRE diferente para falhar a validação
                listaTurmaDres.Add(new PropostaTurmaDre { DreCodigo = "OUTRA_DRE", Dre = new Dre { Todos = false } });
            }

            _mocker.GetMock<IRepositorioProposta>()
                .Setup(r => r.ObterPropostaTurmasDresPorPropostaTurmaId(propostaTurmaId))
                .ReturnsAsync(listaTurmaDres);
        }
    }
}
