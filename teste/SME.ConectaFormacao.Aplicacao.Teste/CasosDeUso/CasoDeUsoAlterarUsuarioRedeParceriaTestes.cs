using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuariosRedeParceria;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoAlterarUsuarioRedeParceriaTestes
    {
        private const long UsuarioId = 123;
        private const long AreaPromotoraAnteriorId = 10;
        private const long AreaPromotoraNovaId = 20;

        private static readonly Guid GrupoAnteriorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid GrupoNovoId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        private readonly Mock<IMediator> mediatorMock;
        private readonly CasoDeUsoAlterarUsuarioRedeParceria sut;

        public CasoDeUsoAlterarUsuarioRedeParceriaTestes()
        {
            mediatorMock = new Mock<IMediator>();
            sut = new CasoDeUsoAlterarUsuarioRedeParceria(mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_lancar_excecao_quando_nome_nao_possuir_sobrenome()
        {
            var dto = CriarDto();
            dto.Nome = "Maria";

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(UsuarioId, dto));

            Assert.Equal(MensagemNegocio.NOME_DEVE_TER_SOBRENOME, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterUsuarioPorIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_lancar_excecao_quando_email_for_invalido()
        {
            var dto = CriarDto();
            dto.Email = "email-invalido";

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(UsuarioId, dto));

            Assert.Equal(MensagemNegocio.EMAIL_INVALIDO, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterUsuarioPorIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_normalizar_documentos_antes_de_buscar_usuario()
        {
            var dto = CriarDto();

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorIdQuery>(q => q.Id == UsuarioId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            await Assert.ThrowsAsync<NegocioException>(() => sut.Executar(UsuarioId, dto));

            Assert.Equal("12345678900", dto.Cpf);
            Assert.Equal("11999999999", dto.Telefone);
        }

        [Fact]
        public async Task Executar_Deve_lancar_excecao_quando_usuario_nao_for_encontrado()
        {
            var dto = CriarDto();

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorIdQuery>(q => q.Id == UsuarioId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Usuario)null!);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(UsuarioId, dto));

            Assert.Equal(MensagemNegocio.USUARIO_NAO_ENCONTRADO, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterAreaPromotoraPorIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_lancar_excecao_quando_usuario_nao_for_da_rede_parceria()
        {
            var dto = CriarDto();
            var usuario = CriarUsuario(TipoUsuario.Interno);
            ConfigurarConsultaUsuario(usuario);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(UsuarioId, dto));

            Assert.Equal(MensagemNegocio.USUARIO_NAO_ENCONTRADO, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterAreaPromotoraPorIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_lancar_excecao_quando_area_promotora_nao_for_encontrada()
        {
            var dto = CriarDto();
            var usuario = CriarUsuario();
            ConfigurarConsultaUsuario(usuario);

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == dto.AreaPromotoraId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaPromotora)null!);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(UsuarioId, dto));

            Assert.Equal(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_alterar_usuario_ativo_sem_trocar_area_promotora()
        {
            var dto = CriarDto(AreaPromotoraAnteriorId, SituacaoUsuario.Ativo);
            var usuario = CriarUsuario(areaPromotoraId: AreaPromotoraAnteriorId);
            var areaPromotora = CriarAreaPromotora(AreaPromotoraAnteriorId, GrupoAnteriorId);

            ConfigurarConsultaUsuario(usuario);
            ConfigurarConsultaAreaPromotora(areaPromotora);
            ConfigurarIntegracoesCoreSso();
            ConfigurarPersistenciaECache();

            var retorno = await sut.Executar(UsuarioId, dto);

            Assert.NotNull(retorno);
            Assert.Equal(dto.Nome, usuario.Nome);
            Assert.Equal(dto.NomeSocial, usuario.NomeSocial);
            Assert.Equal(dto.Email, usuario.Email);
            Assert.Equal("11999999999", usuario.Telefone);
            Assert.Equal(dto.AreaPromotoraId, usuario.AreaPromotoraId);
            Assert.Equal(dto.Situacao, usuario.Situacao);
            Assert.Equal("12345678900", dto.Cpf);
            Assert.Equal("11999999999", dto.Telefone);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<AtualizarUsuarioServicoAcessoCommand>(c => c.NomeSocial == dto.NomeSocial),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<InativarUsuarioCoreSSOServicoAcessosCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<SalvarUsuarioCommand>(c => ReferenceEquals(c.Usuario, usuario)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_Deve_trocar_area_promotora_e_inativar_usuario_no_CoreSSO()
        {
            var dto = CriarDto(AreaPromotoraNovaId, SituacaoUsuario.Inativo);
            var usuario = CriarUsuario(areaPromotoraId: AreaPromotoraAnteriorId);
            var areaAnterior = CriarAreaPromotora(AreaPromotoraAnteriorId, GrupoAnteriorId);
            var areaNova = CriarAreaPromotora(AreaPromotoraNovaId, GrupoNovoId);

            ConfigurarConsultaUsuario(usuario);
            ConfigurarConsultaAreaPromotora(areaNova);
            ConfigurarConsultaAreaPromotora(areaAnterior);
            ConfigurarIntegracoesCoreSso();
            ConfigurarPersistenciaECache();

            var retorno = await sut.Executar(UsuarioId, dto);

            Assert.NotNull(retorno);
            Assert.Equal(AreaPromotoraNovaId, usuario.AreaPromotoraId);
            Assert.Equal(SituacaoUsuario.Inativo, usuario.Situacao);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == AreaPromotoraAnteriorId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<InativarUsuarioCoreSSOServicoAcessosCommand>(c => c.Login == usuario.Login),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Theory]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, true, false)]
        public async Task Executar_Deve_lancar_excecao_e_nao_salvar_quando_integracao_com_CoreSSO_falhar(
            bool usuarioAtualizado,
            bool vinculado,
            bool desvinculado,
            bool inativado)
        {
            var dto = CriarDto(AreaPromotoraNovaId, SituacaoUsuario.Inativo);
            var usuario = CriarUsuario(areaPromotoraId: AreaPromotoraAnteriorId);
            var areaAnterior = CriarAreaPromotora(AreaPromotoraAnteriorId, GrupoAnteriorId);
            var areaNova = CriarAreaPromotora(AreaPromotoraNovaId, GrupoNovoId);

            ConfigurarConsultaUsuario(usuario);
            ConfigurarConsultaAreaPromotora(areaNova);
            ConfigurarConsultaAreaPromotora(areaAnterior);
            ConfigurarIntegracoesCoreSso(usuarioAtualizado, vinculado, desvinculado, inativado);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(UsuarioId, dto));

            Assert.Equal(MensagemNegocio.ERRO_AO_CRIAR_ATUALIZAR_USUARIO_NO_CORESSO, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private void ConfigurarConsultaUsuario(Usuario usuario)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorIdQuery>(q => q.Id == UsuarioId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
        }

        private void ConfigurarConsultaAreaPromotora(AreaPromotora areaPromotora)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == areaPromotora.Id),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);
        }

        private void ConfigurarIntegracoesCoreSso(
            bool usuarioAtualizado = true,
            bool vinculado = true,
            bool desvinculado = true,
            bool inativado = true)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioAtualizado);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculado);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<DesvincularPerfilExternoCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(desvinculado);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<InativarUsuarioCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(inativado);
        }

        private void ConfigurarPersistenciaECache()
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<SalvarUsuarioCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RemoverCacheCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private static UsuarioRedeParceriaDTO CriarDto(
            long areaPromotoraId = AreaPromotoraAnteriorId,
            SituacaoUsuario situacao = SituacaoUsuario.Ativo)
        {
            return new UsuarioRedeParceriaDTO
            {
                AreaPromotoraId = areaPromotoraId,
                Nome = "Maria da Silva",
                NomeSocial = "Maria Silva",
                Cpf = "123.456.789-00",
                Email = "maria.silva@teste.com.br",
                Telefone = "(11) 99999-9999",
                Situacao = situacao
            };
        }

        private static Usuario CriarUsuario(
            TipoUsuario tipo = TipoUsuario.RedeParceria,
            long areaPromotoraId = AreaPromotoraAnteriorId)
        {
            return new Usuario
            {
                Id = UsuarioId,
                Login = "12345678900",
                Nome = "Nome Anterior",
                NomeSocial = "Nome Social Anterior",
                Email = "anterior@teste.com.br",
                Cpf = "12345678900",
                Telefone = "1188887777",
                Tipo = tipo,
                Situacao = SituacaoUsuario.Ativo,
                AreaPromotoraId = areaPromotoraId
            };
        }

        private static AreaPromotora CriarAreaPromotora(long id, Guid grupoId)
        {
            return new AreaPromotora
            {
                Id = id,
                Nome = $"Área Promotora {id}",
                Email = $"area{id}@teste.com.br",
                GrupoId = grupoId
            };
        }
    }
}