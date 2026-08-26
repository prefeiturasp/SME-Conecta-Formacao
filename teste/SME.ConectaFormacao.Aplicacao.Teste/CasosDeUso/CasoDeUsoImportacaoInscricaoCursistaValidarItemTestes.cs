using AutoMapper;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using System.Text.Json;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoImportacaoInscricaoCursistaValidarItemTestes
    {
        private readonly AutoMocker _mocker;
        private readonly CasoDeUsoImportacaoInscricaoCursistaValidarItem _casoDeUso;

        public CasoDeUsoImportacaoInscricaoCursistaValidarItemTestes()
        {
            _mocker = new AutoMocker();
            _casoDeUso = _mocker.CreateInstance<CasoDeUsoImportacaoInscricaoCursistaValidarItem>();
        }

        [Fact]
        public async Task DadoTurmaNaoEncontradaQuandoExecutarDeveRegistrarErroNoItem()
        {
            // Arrange
            var (mensagemRabbit, _) = MontarMensagemRabbitPadrao();

            // Mock: Turma retornando nulo
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorNomeQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((PropostaTurma)null!);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarSituacaoImportacaoArquivoRegistroCommand>(c =>
                    c.Situacao == SituacaoImportacaoArquivoRegistro.Erro &&
                    c.Erro == MensagemNegocio.TURMA_NAO_ENCONTRADA
                ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DadoUsuarioInternoNovoQuandoExecutarDeveSalvarUsuarioEProcessarSucesso()
        {
            // Arrange
            var (mensagemRabbit, dto) = MontarMensagemRabbitPadrao(isRf: true);
            var propostaId = 1;
            var usuarioDto = new DadosUsuarioDTO { Login = dto.RegistroFuncional, Nome = dto.Nome, Email = "teste@teste.com" };
            var usuarioMapeado = new Usuario { Id = 99, Login = dto.RegistroFuncional, Tipo = TipoUsuario.Interno };

            ConfigurarMocksBasicos(propostaId, usuarioMapeado, encontrarUsuarioLocalmente: false);

            // Mock: Serviço externo encontra dados
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterMeusDadosServicoAcessosPorLoginQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioDto);

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<Usuario>(usuarioDto))
                .Returns(usuarioMapeado);

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            // Verifica se salvou o usuário novo
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<SalvarUsuarioCommand>(c => c.Usuario.Login == dto.RegistroFuncional),
                It.IsAny<CancellationToken>()), Times.Once);

            // Verifica se o registro foi validado com sucesso
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarImportacaoRegistroCommand>(c =>
                    c.AlterarImportacaoRegistroDto.Situacao == SituacaoImportacaoArquivoRegistro.Validado
                ), It.IsAny<CancellationToken>()), Times.Once);
        }

        // --- Métodos Auxiliares ---

        private (MensagemRabbit, InscricaoCursistaImportacaoDto) MontarMensagemRabbitPadrao(bool isRf = false)
        {
            var conteudoDto = new InscricaoCursistaImportacaoDto
            {
                Turma = "Turma A",
                ColaboradorRede = isRf ? "1" : "0",
                RegistroFuncional = isRf ? "1234567" : "",
                Cpf = isRf ? "" : "12345678901",
                Nome = "Fulano",
                Vinculo = isRf ? "1" : null
            };

            var registroDto = new ImportacaoArquivoRegistroDto
            {
                Id = 10,
                ImportacaoArquivoId = 100,
                PropostaId = 5,
                Conteudo = JsonSerializer.Serialize(conteudoDto),
                Situacao = SituacaoImportacaoArquivoRegistro.CarregamentoInicial
            };

            var mensagemRabbit = new MensagemRabbit
            {
                Mensagem = JsonSerializer.Serialize(registroDto)
            };

            return (mensagemRabbit, conteudoDto);
        }

        private void ConfigurarMocksBasicos(long propostaId, Usuario usuario, bool encontrarUsuarioLocalmente = true)
        {
            _mocker.GetMock<IMediator>()
               .Setup(m => m.Send(It.IsAny<ObterPropostaTurmaPorNomeQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new PropostaTurma { Id = 10, PropostaId = propostaId });

            if (encontrarUsuarioLocalmente)
            {
                _mocker.GetMock<IMediator>()
                    .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);
            }
            else
            {
                _mocker.GetMock<IMediator>()
                    .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((Usuario)null!);
            }

            // Mocks padrão para não quebrar fluxo se não for o foco do teste
            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPublicosAlvosPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaPublicoAlvo>());

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaFuncoesEspecificasPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaFuncaoEspecifica>());
        }

        [Fact]
        public async Task DadoUsuarioComCargosEFuncoes_QuandoMapear_DeveValidarCorretamente()
        {
            // Arrange
            var (mensagemRabbit, dto) = MontarMensagemRabbitPadrao(isRf: true);
            var propostaId = 5;
            var usuarioMapeado = new Usuario { Id = 99, Login = dto.RegistroFuncional, Tipo = TipoUsuario.Interno };

            ConfigurarMocksBasicos(propostaId, usuarioMapeado, encontrarUsuarioLocalmente: true);

            var cargosFuncoesEol = new List<SME.ConectaFormacao.Infra.Servicos.Eol.CursistaCargoServicoEol>
            {
                new SME.ConectaFormacao.Infra.Servicos.Eol.CursistaCargoServicoEol
                {
                    CdCargoSobreposto = 1,
                    CdDreCargoSobreposto = "DRE1",
                    CdUeCargoSobreposto = "UE1",
                    TipoVinculoCargoSobreposto = 1,
                    CdFuncaoAtividade = 2,
                    CdDreFuncaoAtividade = "DRE2",
                    CdUeFuncaoAtividade = "UE2",
                    TipoVinculoFuncaoAtividade = 1
                }
            };

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterCargosFuncoesDresFuncionarioServicoEolQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cargosFuncoesEol);

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaPublicosAlvosPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaPublicoAlvo> { new PropostaPublicoAlvo { CargoFuncaoId = 10 } });

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterPropostaFuncoesEspecificasPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PropostaFuncaoEspecifica> { new PropostaFuncaoEspecifica { CargoFuncaoId = 20 } });

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.IsAny<ObterCargoFuncaoOutrosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new CargoFuncao { Id = 999 });

            _mocker.GetMock<IMediator>()
                .Setup(m => m.Send(It.Is<ObterCargoFuncaoPorCodigoEolQuery>(q => q.CodigosCargosEol.Contains(1)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CargoFuncao> 
                { 
                    new CargoFuncao { Id = 10, Tipo = CargoFuncaoTipo.Cargo },
                    new CargoFuncao { Id = 20, Tipo = CargoFuncaoTipo.Funcao }
                });

            // Act
            await _casoDeUso.Executar(mensagemRabbit);

            // Assert
            _mocker.GetMock<IMediator>().Verify(m => m.Send(
                It.Is<AlterarImportacaoRegistroCommand>(c =>
                    c.AlterarImportacaoRegistroDto.Situacao == SituacaoImportacaoArquivoRegistro.Validado
                ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}