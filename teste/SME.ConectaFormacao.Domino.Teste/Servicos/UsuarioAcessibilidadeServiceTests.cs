using Bogus;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Dados.Servicos;
using System.Linq.Expressions;

namespace SME.ConectaFormacao.Domino.Teste.Servicos
{
    public class UsuarioAcessibilidadeServiceTests
    {
        private readonly Mock<IRepositorioUsuarioAcessibilidade> _mockRepositorio;
        private readonly UsuarioAcessibilidadeService _servico;
        private readonly Faker _faker;

        public UsuarioAcessibilidadeServiceTests()
        {
            var mocker = new AutoMocker();
            _mockRepositorio = mocker.GetMock<IRepositorioUsuarioAcessibilidade>();
            _servico = mocker.CreateInstance<UsuarioAcessibilidadeService>();
            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task DadoNenhumRegistroExistente_QuandoSalvarNovo_EntaoDeveInserirNaBaseEAtualizarInscricao()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var novoIdGerado = _faker.Random.Long(1);

            var novaAcessibilidade = GerarAcessibilidade(usuarioId);

            _mockRepositorio.Setup(r => r.Inserir(It.IsAny<UsuarioAcessibilidade>()))
                .ReturnsAsync(novoIdGerado);

            // Act
            var idGerado = await _servico.SalvarAcessibilidadeDaInscricaoAsync(novaAcessibilidade);

            // Assert
            _mockRepositorio.Verify(r => r.Inserir(novaAcessibilidade), Times.Once);
            idGerado.Should().Be(novoIdGerado);
        }

        [Fact]
        public async Task DadoRegistroExistenteIdentico_QuandoSalvar_EntaoNaoDeveFazerNadaEManterId()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var idExistente = _faker.Random.Long(1);

            var acessibilidadeExistente = GerarAcessibilidade(usuarioId);
            acessibilidadeExistente.Id = idExistente;
            acessibilidadeExistente.Excluido = false;

            var novaAcessibilidade = new UsuarioAcessibilidade
            {
                UsuarioId = usuarioId,
                PossuiDeficiencia = acessibilidadeExistente.PossuiDeficiencia,
                DescricaoDeficiencia = acessibilidadeExistente.DescricaoDeficiencia,
                NecessitaAdaptacao = acessibilidadeExistente.NecessitaAdaptacao,
                DescricaoAdaptacao = acessibilidadeExistente.DescricaoAdaptacao,
                Excluido = false
            };

            _mockRepositorio.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId))
                .ReturnsAsync(acessibilidadeExistente);

            // Act
            var idGerado = await _servico.SalvarAcessibilidadeDaInscricaoAsync(novaAcessibilidade);

            // Assert
            _mockRepositorio
                .Verify(r => r.Atualizar(It.IsAny<UsuarioAcessibilidade>()), Times.Never);
            _mockRepositorio
                .Verify(r => r.Inserir(It.IsAny<UsuarioAcessibilidade>()), Times.Never);
            idGerado.Should().Be(idExistente);
        }

        [Fact]
        public async Task DadoRegistroExistenteMasUsuarioQuerExcluir_QuandoSalvar_EntaoDeveAtualizarParaExcluido()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var idExistente = _faker.Random.Long(1);

            var acessibilidadeExistente = GerarAcessibilidade(usuarioId);
            acessibilidadeExistente.Id = idExistente;

            var novaAcessibilidade = new UsuarioAcessibilidade
            {
                UsuarioId = usuarioId,
                PossuiDeficiencia = acessibilidadeExistente.PossuiDeficiencia,
                DescricaoDeficiencia = acessibilidadeExistente.DescricaoDeficiencia,
                NecessitaAdaptacao = acessibilidadeExistente.NecessitaAdaptacao,
                DescricaoAdaptacao = acessibilidadeExistente.DescricaoAdaptacao,
                Excluido = true
            };

            _mockRepositorio.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId))
                .ReturnsAsync(acessibilidadeExistente);

            // Act
            var idGerado = await _servico.SalvarAcessibilidadeDaInscricaoAsync(novaAcessibilidade);

            // Assert
            _mockRepositorio
                .Verify(r => r.Atualizar(It.Is<UsuarioAcessibilidade>(a => a.Id == idExistente && 
                                                                           a.Excluido))
                , Times.Once);
            idGerado.Should().Be(idExistente);
        }

        [Fact]
        public async Task DadoAlteracaoDeDados_QuandoSalvar_EntaoDeveExcluirAnteriorEInserirNovo()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var idAntigo = 100;
            var idNovo = 200;

            var atual = GerarAcessibilidade(usuarioId);
            atual.Id = idAntigo;
            atual.DescricaoDeficiencia = "Dados Antigos A";

            var novo = GerarAcessibilidade(usuarioId);
            novo.DescricaoDeficiencia = "Dados Novos B";

            _mockRepositorio.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId)).ReturnsAsync(atual);

            _mockRepositorio.Setup(r => r.Inserir(novo)).ReturnsAsync(idNovo);

            // Act
            var idGerado = await _servico.SalvarAcessibilidadeDaInscricaoAsync(novo);

            // Assert
            _mockRepositorio
                .Verify(r => r.Atualizar(It.Is<UsuarioAcessibilidade>(a => a.Id == idAntigo && 
                                                                           a.Excluido))
                , Times.Once);
            _mockRepositorio.Verify(r => r.Inserir(novo), Times.Once);
            idGerado.Should().Be(idNovo);
        }

        [Fact]
        public async Task DadoAlteracaoDeDadosComHistoricoExistente_QuandoSalvar_EntaoDeveReciclarRegistroAntigo()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var idAtualParaRemover = 10;
            var idAntigoParaReciclar = 20;

            // Cenário: Usuário tinha "Visual" (ID 20 - Excluido), mudou para "Auditiva" (ID 10 - Ativo).
            // Agora quer voltar para "Visual".

            var atual = GerarAcessibilidade(usuarioId);
            atual.Id = idAtualParaRemover;
            atual.DescricaoDeficiencia = "Auditiva";
            atual.Excluido = false;

            var novo = GerarAcessibilidade(usuarioId);
            novo.DescricaoDeficiencia = "Visual";
            novo.Excluido = false;

            var registroParaReciclar = GerarAcessibilidade(usuarioId);
            registroParaReciclar.Id = idAntigoParaReciclar;
            registroParaReciclar.DescricaoDeficiencia = "Visual";
            registroParaReciclar.Excluido = true;

            _mockRepositorio.Setup(r => r.ObterPorUsuarioIdAsync(usuarioId)).ReturnsAsync(atual);

            _mockRepositorio
                .Setup(r => r.ObterPorExpressaoAsync(It.IsAny<Expression<Func<UsuarioAcessibilidade, bool>>>()))
                .ReturnsAsync(registroParaReciclar);

            // Act
            var idGerado = await _servico.SalvarAcessibilidadeDaInscricaoAsync(novo);

            // Assert
            _mockRepositorio
                .Verify(r => r.Atualizar(It.Is<UsuarioAcessibilidade>(a => a.Id == idAtualParaRemover && 
                                                                           a.Excluido))
                , Times.Once);
            _mockRepositorio
                .Verify(r => r.Atualizar(It.Is<UsuarioAcessibilidade>(a => a.Id == idAntigoParaReciclar && 
                                                                          !a.Excluido))
                , Times.Once);
            _mockRepositorio.Verify(r => r.Inserir(It.IsAny<UsuarioAcessibilidade>()), Times.Never);
            idGerado.Should().Be(idAntigoParaReciclar);
        }

        [Fact]
        public async Task DadoNovoCadastroComHistorico_QuandoSalvar_EntaoDeveReativarRegistro()
        {
            // Arrange
            var usuarioId = _faker.Random.Long(1);
            var idReciclado = 55;
            var inscricao = new Inscricao { UsuarioId = usuarioId };

            var novo = GerarAcessibilidade(usuarioId);

            var registroHistorico = GerarAcessibilidade(usuarioId);
            registroHistorico.Id = idReciclado;
            registroHistorico.Excluido = true;

            _mockRepositorio
                .Setup(r => r.ObterPorExpressaoAsync(It.IsAny<Expression<Func<UsuarioAcessibilidade, bool>>>()))
                .ReturnsAsync(registroHistorico);

            // Act
            var idGerado = await _servico.SalvarAcessibilidadeDaInscricaoAsync(novo);

            // Assert
            _mockRepositorio
                .Verify(r => r.Atualizar(It.Is<UsuarioAcessibilidade>(a => a.Id == idReciclado && 
                                                                          !a.Excluido))
                , Times.Once);
            _mockRepositorio.Verify(r => r.Inserir(It.IsAny<UsuarioAcessibilidade>()), Times.Never);

            idGerado.Should().Be(idReciclado);
        }

        private UsuarioAcessibilidade GerarAcessibilidade(long usuarioId)
        {
            return new UsuarioAcessibilidade
            {
                UsuarioId = usuarioId,
                PossuiDeficiencia = _faker.Random.Bool(),
                DescricaoDeficiencia = _faker.Lorem.Sentence(),
                NecessitaAdaptacao = _faker.Random.Bool(),
                DescricaoAdaptacao = _faker.Lorem.Sentence(),
                Excluido = false
            };
        }
    }
}
