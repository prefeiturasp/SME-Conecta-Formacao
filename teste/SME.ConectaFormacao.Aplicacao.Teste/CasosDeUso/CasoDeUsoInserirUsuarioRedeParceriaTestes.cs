using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.UsuariosRedeParceria;
using SME.ConectaFormacao.Aplicacao.Dtos.UsuarioRedeParceria;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoInserirUsuarioRedeParceriaTestes
    {
        private const long AreaPromotoraId = 10;
        private const string CpfComMascara = "529.982.247-25";
        private const string CpfSemMascara = "52998224725";
        private const string TelefoneComMascara = "(11) 99999-9999";
        private const string TelefoneSemMascara = "11999999999";
        private const string SenhaPadrao = "Sgp4725";

        private static readonly Guid GrupoId =
            Guid.Parse("11111111-1111-1111-1111-111111111111");

        private readonly Mock<IMediator> mediatorMock;
        private readonly CasoDeUsoInserirUsuarioRedeParceria sut;

        public CasoDeUsoInserirUsuarioRedeParceriaTestes()
        {
            mediatorMock = new Mock<IMediator>();
            sut = new CasoDeUsoInserirUsuarioRedeParceria(mediatorMock.Object);
        }

        [Fact]
        public async Task Executar_Deve_acumular_erros_de_cpf_nome_e_email_invalidos()
        {
            var dto = CriarDto();
            dto.Cpf = "111.111.111-11";
            dto.Nome = "Maria";
            dto.Email = "email-invalido";

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Contains(
                MensagemNegocio.CPF_COM_DIGITO_VERIFICADOR_INVALIDO.Parametros(dto.Cpf),
                excecao.Message);
            Assert.Contains(MensagemNegocio.NOME_DEVE_TER_SOBRENOME, excecao.Message);
            Assert.Contains(MensagemNegocio.EMAIL_INVALIDO, excecao.Message);

            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_normalizar_cpf_e_telefone_e_rejeitar_usuario_ja_cadastrado_como_rede_parceria()
        {
            var dto = CriarDto();
            var usuarioExistente = CriarUsuarioExistente(
                tipo: TipoUsuario.RedeParceria,
                excluido: false);

            ConfigurarConsultaUsuario(usuarioExistente);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(MensagemNegocio.USUARIO_JA_POSSUI_CADASTRO_COMO_REDE_PARCERIA, excecao.Message);
            Assert.Equal(CpfSemMascara, dto.Cpf);
            Assert.Equal(TelefoneSemMascara, dto.Telefone);

            mediatorMock.Verify(
                m => m.Send(It.IsAny<ObterAreaPromotoraPorIdQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Executar_Deve_lancar_excecao_quando_area_promotora_nao_for_encontrada()
        {
            var dto = CriarDto();
            ConfigurarConsultaUsuario(null);

            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == dto.AreaPromotoraId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((AreaPromotora)null!);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<UsuarioExisteNoCoreSsoQuery>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData(false, false, false, SenhaPadrao)]
        [InlineData(false, true, true, SenhaPadrao)]
        [InlineData(true, false, false, SenhaPadrao)]
        [InlineData(true, true, true, "")]
        public async Task Executar_Deve_cadastrar_ou_atualizar_no_CoreSSO_com_a_senha_correta(
            bool existeNoConecta,
            bool existeNoCoreSso,
            bool deveAtualizar,
            string senhaEsperada)
        {
            var dto = CriarDto();
            var areaPromotora = CriarAreaPromotora();
            var usuarioExistente = existeNoConecta
                ? CriarUsuarioExistente(TipoUsuario.RedeParceria, excluido: true)
                : null;
            Usuario? usuarioPersistido = null;

            ConfigurarConsultaUsuario(usuarioExistente);
            ConfigurarConsultaAreaPromotora(areaPromotora);
            ConfigurarExistenciaNoCoreSso(existeNoCoreSso);
            ConfigurarCriacaoEAtualizacaoNoCoreSso();
            ConfigurarVinculoComAreaPromotora();
            ConfigurarPersistenciaECache(usuario => usuarioPersistido = usuario);

            var retorno = await sut.Executar(dto);

            Assert.NotNull(retorno);
            Assert.NotNull(usuarioPersistido);
            Assert.Equal(CpfSemMascara, dto.Cpf);
            Assert.Equal(TelefoneSemMascara, dto.Telefone);
            Assert.Equal(TipoUsuario.RedeParceria, usuarioPersistido!.Tipo);
            Assert.Equal(CpfSemMascara, usuarioPersistido.Login);
            Assert.Equal(dto.Nome, usuarioPersistido.Nome);
            Assert.Equal(dto.NomeSocial, usuarioPersistido.NomeSocial);
            Assert.Equal(CpfSemMascara, usuarioPersistido.Cpf);
            Assert.Equal(dto.AreaPromotoraId, usuarioPersistido.AreaPromotoraId);
            Assert.Equal(TelefoneSemMascara, usuarioPersistido.Telefone);
            Assert.Equal(dto.Email, usuarioPersistido.Email);
            Assert.Equal(dto.Situacao, usuarioPersistido.Situacao);
            Assert.False(usuarioPersistido.Excluido);

            if (existeNoConecta)
                Assert.Same(usuarioExistente, usuarioPersistido);
            else
                Assert.NotSame(usuarioExistente, usuarioPersistido);

            if (deveAtualizar)
            {
                mediatorMock.Verify(
                    m => m.Send(
                        It.Is<AtualizarUsuarioServicoAcessoCommand>(c =>
                            c.Login == CpfSemMascara &&
                            c.Nome == dto.Nome &&
                            c.Email == dto.Email &&
                            c.Senha == senhaEsperada &&
                            c.NomeSocial == dto.NomeSocial),
                        It.IsAny<CancellationToken>()),
                    Times.Once);

                mediatorMock.Verify(
                    m => m.Send(
                        It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never);
            }
            else
            {
                mediatorMock.Verify(
                    m => m.Send(
                        It.Is<CadastrarUsuarioServicoAcessoCommand>(c =>
                            c.Login == CpfSemMascara &&
                            c.Nome == dto.Nome &&
                            c.Email == dto.Email &&
                            c.Senha == senhaEsperada &&
                            c.NomeSocial == dto.NomeSocial),
                        It.IsAny<CancellationToken>()),
                    Times.Once);

                mediatorMock.Verify(
                    m => m.Send(
                        It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never);
            }

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<VincularPerfilExternoCoreSSOServicoAcessosCommand>(c =>
                        c.Login == CpfSemMascara && c.PerfilId == GrupoId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(
                    It.Is<SalvarUsuarioCommand>(c => ReferenceEquals(c.Usuario, usuarioPersistido)),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Executar_Deve_reaproveitar_usuario_existente_de_outro_tipo()
        {
            var dto = CriarDto();
            var usuarioExistente = CriarUsuarioExistente(
                tipo: TipoUsuario.Interno,
                excluido: false);
            Usuario? usuarioPersistido = null;

            ConfigurarConsultaUsuario(usuarioExistente);
            ConfigurarConsultaAreaPromotora(CriarAreaPromotora());
            ConfigurarExistenciaNoCoreSso(existe: false);
            ConfigurarCriacaoEAtualizacaoNoCoreSso();
            ConfigurarVinculoComAreaPromotora();
            ConfigurarPersistenciaECache(usuario => usuarioPersistido = usuario);

            await sut.Executar(dto);

            Assert.Same(usuarioExistente, usuarioPersistido);
            Assert.Equal(TipoUsuario.RedeParceria, usuarioExistente.Tipo);
            Assert.False(usuarioExistente.Excluido);
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, false)]
        public async Task Executar_Deve_lancar_excecao_e_nao_persistir_quando_integracao_com_CoreSSO_falhar(
            bool existeNoCoreSso,
            bool usuarioCriadoOuAtualizado,
            bool vinculadoAoGrupo)
        {
            var dto = CriarDto();

            ConfigurarConsultaUsuario(null);
            ConfigurarConsultaAreaPromotora(CriarAreaPromotora());
            ConfigurarExistenciaNoCoreSso(existeNoCoreSso);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioCriadoOuAtualizado);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioCriadoOuAtualizado);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculadoAoGrupo);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Executar(dto));

            Assert.Equal(MensagemNegocio.ERRO_AO_CRIAR_ATUALIZAR_USUARIO_NO_CORESSO, excecao.Message);
            mediatorMock.Verify(
                m => m.Send(
                    It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.Verify(
                m => m.Send(It.IsAny<RemoverCacheCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private void ConfigurarConsultaUsuario(Usuario? usuario)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterUsuarioPorLoginQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario!);
        }

        private void ConfigurarConsultaAreaPromotora(AreaPromotora areaPromotora)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterAreaPromotoraPorIdQuery>(q => q.Id == areaPromotora.Id),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(areaPromotora);
        }

        private void ConfigurarExistenciaNoCoreSso(bool existe)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<UsuarioExisteNoCoreSsoQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existe);
        }

        private void ConfigurarCriacaoEAtualizacaoNoCoreSso(
            bool criado = true,
            bool atualizado = true)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<CadastrarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(criado);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<AtualizarUsuarioServicoAcessoCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(atualizado);
        }

        private void ConfigurarVinculoComAreaPromotora(bool vinculado = true)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(vinculado);
        }

        private void ConfigurarPersistenciaECache(Action<Usuario>? aoSalvar = null)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<SalvarUsuarioCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>(
                    (request, _) =>
                    {
                        if (request is SalvarUsuarioCommand command)
                            aoSalvar?.Invoke(command.Usuario);
                    })
                .ReturnsAsync(true);

            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<RemoverCacheCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        private static UsuarioRedeParceriaDTO CriarDto()
        {
            return new UsuarioRedeParceriaDTO
            {
                AreaPromotoraId = AreaPromotoraId,
                Nome = "Maria da Silva",
                NomeSocial = "Maria Silva",
                Cpf = CpfComMascara,
                Email = "maria.silva@teste.com.br",
                Telefone = TelefoneComMascara,
                Situacao = SituacaoUsuario.Ativo
            };
        }

        private static Usuario CriarUsuarioExistente(
            TipoUsuario tipo,
            bool excluido)
        {
            return new Usuario
            {
                Id = 123,
                Login = CpfSemMascara,
                Nome = "Nome anterior",
                NomeSocial = "Nome social anterior",
                Cpf = CpfSemMascara,
                Email = "anterior@teste.com.br",
                Telefone = "11888888888",
                AreaPromotoraId = 99,
                Tipo = tipo,
                Situacao = SituacaoUsuario.Inativo,
                Excluido = excluido
            };
        }

        private static AreaPromotora CriarAreaPromotora()
        {
            return new AreaPromotora
            {
                Id = AreaPromotoraId,
                Nome = "Área promotora",
                Email = "area@teste.com.br",
                GrupoId = GrupoId
            };
        }
    }
}
