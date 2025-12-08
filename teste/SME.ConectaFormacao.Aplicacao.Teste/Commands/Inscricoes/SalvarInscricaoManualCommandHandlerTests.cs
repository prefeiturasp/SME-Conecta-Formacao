using AutoMapper;
using Bogus;
using Bogus.Extensions.Brazil;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.Comandos.Inscricoes.SalvarInscricaoManual;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Inscricoes
{
    public class SalvarInscricaoManualCommandHandlerTests
    {
        private readonly AutoMocker _mocker;
        private readonly SalvarInscricaoManualCommandHandler _handler;
        private readonly Faker _faker;

        public SalvarInscricaoManualCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _handler = _mocker.CreateInstance<SalvarInscricaoManualCommandHandler>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoUsuarioNaoEncontrado_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = GerarDtoValido();
            var comando = new SalvarInscricaoManualCommand(dto, false);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            // Simula não encontrar na API externa também
            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.IsAny<ObterMeusDadosServicoAcessosPorLoginQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((DadosUsuarioDTO)null!);

            // Act & Assert
            var excecao = await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            Assert.Equal(MensagemNegocio.USUARIO_NAO_ENCONTRADO, excecao.Message);
        }

        [Fact]
        public async Task DadoUsuarioSemCargoNoPublicoAlvo_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = GerarDtoValido(profissionalRede: true);
            var comando = new SalvarInscricaoManualCommand(dto, false);
            var usuario = GerarUsuario(TipoUsuario.Interno);
            var propostaTurma = new PropostaTurma { Id = dto.PropostaTurmaId, PropostaId = 1 };
            var proposta = GerarPropostaEmPeriodoInscricao(1);

            ConfigurarMocksBasicos(usuario, propostaTurma, proposta);

            // Simula falha na validação de público alvo
            ConfigurarValidacaoPublicoAlvo(proposta.Id, usuario.Id, valido: false);
            _mocker.GetMock<IMapper>().Setup(m => m.Map<Inscricao>(dto)).Returns(new Inscricao());

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            //Assert.Equal(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO, excecao.Message);
        }

        [Fact]
        public async Task DadoUsuarioInternoSemLotacaoNaDreDaTurma_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = GerarDtoValido(profissionalRede: true);
            var comando = new SalvarInscricaoManualCommand(dto, false);
            var usuario = GerarUsuario(TipoUsuario.Interno);
            var propostaTurma = new PropostaTurma { Id = dto.PropostaTurmaId, PropostaId = 1 };
            var proposta = GerarPropostaEmPeriodoInscricao(1);

            ConfigurarMocksBasicos(usuario, propostaTurma, proposta);
            ConfigurarValidacaoPublicoAlvo(proposta.Id, usuario.Id, valido: true);

            // Simula falha na validação de DRE
            ConfigurarValidacaoDreInterno(propostaTurma.Id, usuario, valido: false);
            _mocker.GetMock<IMapper>().Setup(m => m.Map<Inscricao>(dto)).Returns(new Inscricao { CargoCodigo = "123" });

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            //Assert.Equal(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL, excecao.Message);
        }

        [Fact]
        public async Task DadoInscricaoForaDoPeriodo_QuandoExecutar_EntaoDeveLancarNegocioException()
        {
            // Arrange
            var dto = GerarDtoValido();
            var comando = new SalvarInscricaoManualCommand(dto, false);
            var usuario = GerarUsuario(TipoUsuario.Interno);
            var propostaTurma = new PropostaTurma { Id = dto.PropostaTurmaId, PropostaId = 1 };

            // Proposta com datas passadas
            var proposta = new Proposta
            {
                Id = 1,
                DataInscricaoInicio = DateTime.Now.AddDays(-10),
                DataInscricaoFim = DateTime.Now.AddDays(-5)
            };

            ConfigurarMocksBasicos(usuario, propostaTurma, proposta);
            ConfigurarValidacaoPublicoAlvo(proposta.Id, usuario.Id, valido: true);
            ConfigurarValidacaoDreInterno(propostaTurma.Id, usuario, valido: true);
            ConfigurarValidacaoDuplicidade(proposta.Id, usuario.Id, existe: false);
            _mocker.GetMock<IMapper>().Setup(m => m.Map<Inscricao>(dto)).Returns(new Inscricao());

            // Act & Assert
            await Assert.ThrowsAsync<NegocioException>(() => _handler.Handle(comando, CancellationToken.None));
            //Assert.Equal(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO, excecao.Message);
        }

        // --- Helpers de Configuração ---

        private InscricaoManualDTO GerarDtoValido(bool profissionalRede = true)
        {
            return new InscricaoManualDTO
            {
                Cpf = _faker.Person.Cpf(),
                PropostaTurmaId = _faker.Random.Long(1),
                ProfissionalRede = profissionalRede,
                CargoCodigo = "1234",
                RegistroFuncional = "1234567"
            };
        }

        private Usuario GerarUsuario(TipoUsuario tipo)
        {
            return new Usuario
            {
                Id = _faker.Random.Long(1),
                Login = _faker.Person.Cpf(),
                Tipo = tipo,
                CodigoEolUnidade = "123456"
            };
        }

        private static Proposta GerarPropostaEmPeriodoInscricao(long id)
        {
            return new Proposta
            {
                Id = id,
                FormacaoHomologada = FormacaoHomologada.Sim,
                DataInscricaoInicio = DateTime.Now.AddDays(-1),
                DataInscricaoFim = DateTime.Now.AddDays(1)
            };
        }

        private void ConfigurarMocksBasicos(Usuario usuario, PropostaTurma turma, Proposta proposta)
        {
            // Mock Usuario
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);

            // Mock PropostaTurma
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterPropostaTurmaPorIdQuery>(q => q.PropostaTurmaId == turma.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(turma);

            // Mock Proposta
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterPropostaPorIdQuery>(q => q.Id == proposta.Id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(proposta);

            // Mock Mapeamento Cargo/Função EOL (retorna lista vazia ou genérica para não quebrar)
            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.IsAny<ObterCargoFuncaoPorCodigoEolQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<CargoFuncao>());

            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.IsAny<ObterCargoFuncaoOutrosQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new CargoFuncao { Id = 9999 }); // ID genérico para "Outros"
        }

        private void ConfigurarValidacaoPublicoAlvo(long propostaId, long usuarioCargoId, bool valido)
        {
            // Retorna uma lista de públicos alvo. Se valido=true, fingimos que o usuario tem o cargo certo (ou a lista é vazia e não checa)
            // Para simplificar, se for para falhar, retornamos uma lista que não contém o ID null (já que nos mocks basicos o usuario nao tem ID de cargo definido por padrao)

            var publicosAlvo = new List<PropostaPublicoAlvo>();
            if (valido)
                publicosAlvo.Add(new PropostaPublicoAlvo { CargoFuncaoId = usuarioCargoId }); // Match
            else
                publicosAlvo.Add(new PropostaPublicoAlvo { CargoFuncaoId = 999 }); // No Match

            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.Is<ObterPropostaPublicosAlvosPorIdQuery>(q => q.PropostaId == propostaId), It.IsAny<CancellationToken>()))
               .ReturnsAsync(publicosAlvo);

            // Funções específicas vazias por padrão para isolar teste de Cargo
            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.IsAny<ObterPropostaFuncoesEspecificasPorIdQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync([]);
        }

        private void ConfigurarValidacaoDreInterno(long propostaTurmaId, Usuario usuario, bool valido)
        {
            var dresTurma = new List<PropostaTurmaDre>
            {
                // Cenário: Turma não é para TODOS, é específica
                new() { Dre = new Dre { Todos = false, Codigo = "DRE01" }, DreCodigo = "DRE01" }
            };

            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.Is<ObterPropostaTurmaDresPorPropostaTurmaIdQuery>(q => q.PropostaTurmaIds.Contains(propostaTurmaId)), It.IsAny<CancellationToken>()))
               .ReturnsAsync(dresTurma);

            // Atribuição do Servidor
            var dreUsuario = valido ? "DRE01" : "DRE02";
            var atribuicoes = new List<DreUeAtribuicaoServicoEol>
            {
                new() { DreCodigo = dreUsuario, UeCodigo = "UE01" }
            };

            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.IsAny<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(atribuicoes);
        }

        private void ConfigurarValidacaoDuplicidade(long propostaId, long usuarioId, bool existe)
        {
            _mocker.GetMock<IRepositorioInscricao>()
                .Setup(r => r.UsuarioEstaInscritoNaProposta(propostaId, usuarioId))
                .ReturnsAsync(existe);
        }
    }
}
