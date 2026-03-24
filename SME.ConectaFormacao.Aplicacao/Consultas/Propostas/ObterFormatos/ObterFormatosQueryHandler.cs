using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormatosQueryHandler : IRequestHandler<ObterFormatosQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterFormatosQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(Formato))
                .Cast<Formato>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            if (request.TipoFormacao.HasValue && request.TipoFormacao == TipoFormacao.Curso)
                lista = lista.Where(t => (Formato)t.Id != Formato.Hibrido);

            return Task.FromResult(lista);
        }
    }
}
