using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;

namespace SME.ConectaFormacao.Aplicacao.Consultas.CriterioCertificacao.ObterCriterioCertificacao
{
    public class ObterCriterioCertificacaoQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        private static ObterCriterioCertificacaoQuery _instancia;
        public static ObterCriterioCertificacaoQuery Instancia => _instancia ??= new();
    }
}