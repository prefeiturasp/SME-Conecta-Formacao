using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNotificacaoCategoriaQueryHandler : IRequestHandler<ObterNotificacaoCategoriaQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterNotificacaoCategoriaQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(NotificacaoCategoria))
                .Cast<NotificacaoCategoria>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            return Task.FromResult(lista);
        }
    }
}
