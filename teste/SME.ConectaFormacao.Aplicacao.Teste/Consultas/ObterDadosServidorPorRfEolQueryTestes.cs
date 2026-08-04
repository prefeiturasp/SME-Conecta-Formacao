using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterDadosServidorPorRfEol;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterDadosServidorPorRfEolQueryTestes
    {
        [Theory]
        [InlineData("1234567")]
        [InlineData("")]
        [InlineData(" ")]
        public void Construtor_Deve_atribuir_rf_servidor(string rfServidor)
        {
            var query = new ObterDadosServidorPorRfEolQuery(rfServidor);

            Assert.Equal(rfServidor, query.RfServidor);
        }

        [Fact]
        public void Construtor_Quando_rf_for_nulo_Deve_manter_valor_nulo()
        {
            var query = new ObterDadosServidorPorRfEolQuery(null!);

            Assert.Null(query.RfServidor);
        }

        [Fact]
        public void Query_Deve_implementar_request_com_retorno_usuario_eol_opcional()
        {
            var query = new ObterDadosServidorPorRfEolQuery("1234567");

            Assert.IsAssignableFrom<IRequest<UsuarioEolDto?>>(query);
        }
    }
}
