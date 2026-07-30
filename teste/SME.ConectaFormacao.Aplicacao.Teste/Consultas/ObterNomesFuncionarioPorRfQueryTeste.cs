using MediatR;
using SME.ConectaFormacao.Aplicacao.Consultas.Eol.ObterNomesFuncionarioPorRf;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.Teste.Consultas
{
    public class ObterNomesFuncionarioPorRfQueryTestes
    {
        [Fact]
        public void Construtor_Deve_atribuir_rf_informado()
        {
            const string rf = "1234567";

            var query = new ObterNomesFuncionarioPorRfQuery(rf);

            Assert.Equal(rf, query.Rf);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        public void Construtor_Deve_manter_rf_sem_normalizacao(string rf)
        {
            var query = new ObterNomesFuncionarioPorRfQuery(rf);

            Assert.Equal(rf, query.Rf);
        }

        [Fact]
        public void Construtor_Deve_aceitar_rf_nulo()
        {
            var query = new ObterNomesFuncionarioPorRfQuery(null!);

            Assert.Null(query.Rf);
        }

        [Fact]
        public void Rf_Deve_permitir_alteracao()
        {
            var query = new ObterNomesFuncionarioPorRfQuery("1234567");

            query.Rf = "7654321";

            Assert.Equal("7654321", query.Rf);
        }

        [Fact]
        public void Query_Deve_implementar_contrato_mediatr()
        {
            var query = new ObterNomesFuncionarioPorRfQuery("1234567");

            Assert.IsAssignableFrom<IRequest<FuncionarioNomesDto?>>(query);
        }
    }
}
