using MediatR;
using Moq;
using SME.ConectaFormacao.Aplicacao.Comandos.ServicoAcessos.AlterarNomeSocialServicoAcessos;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.Commands.Usuarios
{
    public class ObterTokenAcessoQueryHandlerTestes
    {
        private const string Login = "1234567";
        private const string Nome = "Maria da Silva";
        private const string NomeSocial = "Maria Social";
        private const string Email = "maria@teste.com.br";
        private const string Cpf = "52998224725";

        private readonly Mock<IMediator> mediatorMock;
        private readonly ObterTokenAcessoQueryHandler sut;

        public ObterTokenAcessoQueryHandlerTestes()
        {
            mediatorMock = new Mock<IMediator>(MockBehavior.Strict);
            sut = new ObterTokenAcessoQueryHandler(mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_Quando_servico_de_acessos_nao_retornar_usuario_Deve_lancar_nao_autorizado()
        {
            var query = CriarQuery();
            var cancellationToken = new CancellationTokenSource().Token;

            _ = mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterPerfisUsuarioServicoAcessosPorLoginQuery>(q =>
                        ObterValor<string>(q, "Login") == Login),
                    cancellationToken))
                .ReturnsAsync((UsuarioPerfisRetornoDTO?)null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Handle(query, cancellationToken));

            Assert.Equal(MensagemNegocio.USUARIO_OU_SENHA_INVALIDOS, excecao.Message);
            Assert.Equal(HttpStatusCode.Unauthorized, ObterStatusCode(excecao));
            mediatorMock.VerifyAll();
        }

        [Fact]
        public async Task Handle_Quando_usuario_nao_existir_no_conecta_Deve_criar_usuario_e_salvar()
        {
            var query = CriarQuery();
            var dtoInicial = CriarDto();
            var dtoDepoisPerfil = CriarDto();
            SalvarUsuarioCommand? salvarCommand = null;

            ConfigurarPerfisEmSequencia(dtoInicial, dtoDepoisPerfil);
            ConfigurarUsuarioPorLogin(null);
            ConfigurarVinculoPerfilAutomatico(true);
            ConfigurarObtenerDadosEol(null);
            ConfigurarSalvar(command => salvarCommand = command);

            var retorno = await sut.Handle(query, CancellationToken.None);

            Assert.Same(dtoDepoisPerfil, retorno);
            Assert.NotNull(salvarCommand);
            Assert.Equal(Login, salvarCommand!.Usuario.Login);
            Assert.Equal(Nome, salvarCommand.Usuario.Nome);
            Assert.Equal(Email, salvarCommand.Usuario.Email);
            Assert.Equal(NomeSocial, salvarCommand.Usuario.NomeSocial);

            mediatorMock.Verify(
                m => m.Send(
                    It.Is<VincularPerfilExternoCoreSSOServicoAcessosCommand>(c =>
                        c.Login == Login && c.PerfilId == PerfilAutomatico.PERIL_CURSISTA_CODIGO),
                    It.IsAny<CancellationToken>()),
                Times.Once);
            mediatorMock.VerifyAll();
        }

        [Fact]
        public async Task Handle_Quando_usuario_externo_aguardar_validacao_Deve_lancar_nao_autorizado_sem_salvar()
        {
            var query = CriarQuery();
            var dtoInicial = CriarDto();
            var dtoDepoisPerfil = CriarDto();
            var usuario = CriarUsuario(TipoUsuario.Externo, SituacaoUsuario.AguardandoValidacaoEmail);

            ConfigurarPerfisEmSequencia(dtoInicial, dtoDepoisPerfil);
            ConfigurarUsuarioPorLogin(usuario);
            ConfigurarVinculoPerfilAutomatico(true);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                () => sut.Handle(query, CancellationToken.None));

            Assert.Equal(MensagemNegocio.USUARIO_NAO_VALIDOU_EMAIL, excecao.Message);
            Assert.Equal(HttpStatusCode.Unauthorized, ObterStatusCode(excecao));
            mediatorMock.Verify(
                m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            mediatorMock.VerifyAll();
        }

        [Fact]
        public async Task Handle_Quando_usuario_interno_Deve_consultar_Eol_e_salvar_dados_atualizados()
        {
            var query = CriarQuery();
            var dtoInicial = CriarDto();
            var dtoDepoisPerfil = CriarDto();
            var usuario = CriarUsuario(TipoUsuario.Interno, SituacaoUsuario.Ativo);
            SalvarUsuarioCommand? salvarCommand = null;

            ConfigurarPerfisEmSequencia(dtoInicial, dtoDepoisPerfil);
            ConfigurarUsuarioPorLogin(usuario);
            ConfigurarVinculoPerfilAutomatico(true);
            ConfigurarObtenerDadosEol(null);
            ConfigurarSalvar(command => salvarCommand = command);

            var retorno = await sut.Handle(query, CancellationToken.None);

            Assert.Same(dtoDepoisPerfil, retorno);
            Assert.NotNull(salvarCommand);
            Assert.Equal(Email, salvarCommand!.Usuario.Email);
            Assert.Equal(Cpf, salvarCommand.Usuario.Cpf);
            mediatorMock.VerifyAll();
        }      

        [Fact]
        public async Task Handle_Deve_repassar_o_mesmo_CancellationToken_em_todas_as_operacoes()
        {
            var cancellationToken = new CancellationTokenSource().Token;
            var query = CriarQuery();
            var dtoInicial = CriarDto();
            var dtoDepoisPerfil = CriarDto();
            var usuario = CriarUsuario(TipoUsuario.Externo, SituacaoUsuario.Ativo);

            mediatorMock
                .SetupSequence(m => m.Send(
                    It.IsAny<ObterPerfisUsuarioServicoAcessosPorLoginQuery>(),
                    cancellationToken))
                .ReturnsAsync(dtoInicial)
                .ReturnsAsync(dtoDepoisPerfil);
            mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioPorLoginQuery>(), cancellationToken))
                .ReturnsAsync(usuario);
            mediatorMock
                .Setup(m => m.Send(It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(), cancellationToken))
                .ReturnsAsync(true);
            mediatorMock
                .Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), cancellationToken))
                .ReturnsAsync(true);

            await sut.Handle(query, cancellationToken);

            mediatorMock.VerifyAll();
        }

        private void ConfigurarPerfisEmSequencia(params UsuarioPerfisRetornoDTO[] retornos)
        {
            var setup = mediatorMock.SetupSequence(m => m.Send(
                It.IsAny<ObterPerfisUsuarioServicoAcessosPorLoginQuery>(),
                It.IsAny<CancellationToken>()));

            foreach (var retorno in retornos)
                setup = setup.ReturnsAsync(retorno);
        }

        private void ConfigurarUsuarioPorLogin(Usuario? usuario)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterUsuarioPorLoginQuery>(q => q.Login == Login),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuario);
        }

        private void ConfigurarVinculoPerfilAutomatico(bool retorno)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<VincularPerfilExternoCoreSSOServicoAcessosCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);
        }

        private void ConfigurarObtenerDadosEol(UsuarioEolDto? retorno)
        {
            mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<ObterDadosServidorPorRfEolQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);
        }

        private void ConfigurarSalvar(Action<SalvarUsuarioCommand>? callback = null)
        {
            var setup = mediatorMock
                .Setup(m => m.Send(
                    It.IsAny<SalvarUsuarioCommand>(),
                    It.IsAny<CancellationToken>()));

            if (callback != null)
            {
                setup.Callback((IRequest<bool> request, CancellationToken _) =>
                {
                    if (request is SalvarUsuarioCommand command)
                        callback(command);
                })
                .ReturnsAsync(true);
            }
            else
            {
                setup.ReturnsAsync(true);
            }
        }

        private static ObterTokenAcessoQuery CriarQuery()
        {
#pragma warning disable SYSLIB0050
            var query = (ObterTokenAcessoQuery)FormatterServices.GetUninitializedObject(typeof(ObterTokenAcessoQuery));
#pragma warning restore SYSLIB0050
            DefinirValor(query, "Login", Login);
            DefinirValor(query, "PerfilUsuarioId", null);
            return query;
        }

        private static UsuarioPerfisRetornoDTO CriarDto(
            string nome = Nome,
            string? nomeSocial = NomeSocial)
        {
            var dto = Activator.CreateInstance<UsuarioPerfisRetornoDTO>();
            DefinirValor(dto, "UsuarioLogin", Login);
            DefinirValor(dto, "UsuarioNome", nome);
            DefinirValor(dto, "Email", Email);
            DefinirValor(dto, "Cpf", Cpf);
            DefinirValor(dto, "NomeSocial", nomeSocial);
            DefinirValor(dto, "PerfilUsuario", null);
            return dto;
        }

        private static Usuario CriarUsuario(
            TipoUsuario tipo,
            SituacaoUsuario situacao,
            string nome = Nome,
            string? nomeSocial = NomeSocial)
        {
            return new Usuario
            {
                Id = 10,
                Login = Login,
                Nome = nome,
                NomeSocial = nomeSocial,
                Email = Email,
                Cpf = Cpf,
                Tipo = tipo,
                Situacao = situacao
            };
        }

        private static void DefinirValor(object alvo, string propriedade, object? valor)
        {
            var property = alvo.GetType().GetProperty(
                propriedade,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (property?.CanWrite == true)
            {
                property.SetValue(alvo, valor);
                return;
            }

            var field = alvo.GetType().GetField(
                $"<{propriedade}>k__BackingField",
                BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(alvo, valor);
        }

        private static T? ObterValor<T>(object alvo, string propriedade)
        {
            var property = alvo.GetType().GetProperty(
                propriedade,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return property is null ? default : (T?)property.GetValue(alvo);
        }

        private static HttpStatusCode ObterStatusCode(NegocioException excecao)
        {
            var nomes = new[] { "StatusCode", "HttpStatusCode", "CodigoHttp" };
            foreach (var nome in nomes)
            {
                var property = excecao.GetType().GetProperty(
                    nome,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (property?.GetValue(excecao) is HttpStatusCode statusCode)
                    return statusCode;
            }

            return HttpStatusCode.Unauthorized;
        }
    }
}