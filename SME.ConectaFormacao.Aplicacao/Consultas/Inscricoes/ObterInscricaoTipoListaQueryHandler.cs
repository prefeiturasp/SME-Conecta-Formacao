using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoTipoListaQueryHandler : IRequestHandler<ObterInscricaoTipoListaQuery, IEnumerable<RetornoListagemDTO>>
    {
        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterInscricaoTipoListaQuery request, CancellationToken cancellationToken)
        {
            return Enum.GetValues(typeof(TipoInscricao))
                .Cast<TipoInscricao>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });
        }
    }
}