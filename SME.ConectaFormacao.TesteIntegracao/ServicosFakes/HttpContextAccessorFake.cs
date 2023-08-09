using Microsoft.AspNetCore.Http;

namespace SME.ConectaFormacao.TesteIntegracao.ServicosFakes
{
    public class HttpContextAccessorFake : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }

        public HttpContextAccessorFake()
        {
            HttpContext = new DefaultHttpContext();
        }
    }
}