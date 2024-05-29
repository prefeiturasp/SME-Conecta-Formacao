using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoTipoQueryHandler : IRequestHandler<ObterNotificacaoTipoQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterNotificacaoTipoQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(NotificacaoTipo))
                .Cast<NotificacaoTipo>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            return Task.FromResult(lista);
        }
    }
}
