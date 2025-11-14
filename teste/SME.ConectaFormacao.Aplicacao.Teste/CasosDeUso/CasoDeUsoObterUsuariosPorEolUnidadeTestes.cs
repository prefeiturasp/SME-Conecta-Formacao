using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using MediatR;
using Moq;
using Moq.AutoMock;
using SME.ConectaFormacao.Aplicacao.CasosDeUso.Usuario;
using SME.ConectaFormacao.Aplicacao.Consultas.Usuario.ObterUsuariosPorUnidadeEol;
using SME.ConectaFormacao.Aplicacao.Dtos.Usuario;
using SME.ConectaFormacao.Dominio.Extensoes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Teste.CasosDeUso
{
    public class CasoDeUsoObterUsuariosPorEolUnidadeTestes
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CasoDeUsoObterUsuariosPorEolUnidade _casoDeUso;
        private readonly Faker _faker;

        public CasoDeUsoObterUsuariosPorEolUnidadeTestes()
        {
            var mocker = new AutoMocker();
            _mediator = mocker.GetMock<IMediator>();
            _casoDeUso = mocker.CreateInstance<CasoDeUsoObterUsuariosPorEolUnidade>();
            _faker = new();
        }

        [Fact]
        public async Task DadoQualquerRequisicao_QuandoChamarExecutar_DeveRetornarDadosDoLoginDoUsuario()
        {
            // Arrange
            var codigoEolUnidade = _faker.Random.Int(1).ToString();
            var login = _faker.Person.Cpf();
            var nome = _faker.Person.FirstName;

            var dadosUsuario = new List<DadosLoginUsuarioDto>
            {
                new ()
                {
                    Login = login.SomenteNumeros(),
                    Nome = _faker.Person.FullName
                }
            };

            _mediator
                .Setup(m => m.Send(It.IsAny<ObterUsuariosPorEolUnidadeQuery>()))
                .ReturnsAsync(dadosUsuario);

            // Act
            var resposta = await _casoDeUso.ExecutarAsync(codigoEolUnidade, login, nome);

            // Assert
            resposta.Should().BeEquivalentTo(dadosUsuario);
        }
    }
}
