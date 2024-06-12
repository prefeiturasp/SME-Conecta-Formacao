using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoSituacaoQueryHandler : IRequestHandler<ObterNotificacaoSituacaoQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterNotificacaoSituacaoQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(NotificacaoUsuarioSituacao))
                .Cast<NotificacaoUsuarioSituacao>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            return Task.FromResult(lista);
        }
    }
}
