using System.Net;
using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Proposta.ServicosFakes
{
    public class ObterNomeProfissionalPorRegistroFuncionalQueryHandlerFake : IRequestHandler<ObterNomeProfissionalPorRegistroFuncionalQuery, string>
    {
        public async Task<string> Handle(ObterNomeProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RegistroFuncional))
                throw new NegocioException(mensagem: MensagemNegocio.PROFISSIONAL_NAO_LOCALIZADO, statusCode: HttpStatusCode.NoContent);
            return "Nome do Profissional";
        }
    }
}