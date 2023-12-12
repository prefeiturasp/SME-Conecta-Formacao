using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormatosQuery : IRequest<IEnumerable<RetornoListagemDTO>>
    {
        public ObterFormatosQuery(TipoFormacao? tipoFormacao)
        {
            TipoFormacao = tipoFormacao;
        }

        public TipoFormacao? TipoFormacao { get; }
    }
}
