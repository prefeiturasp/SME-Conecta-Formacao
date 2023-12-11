using Bogus;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Arquivo.Mocks
{
    public class BaixarArquivoMock
    {
        public static string GerarUrlImagem()
        {
            return new Faker().Image.PicsumUrl();
        }
    }
}
