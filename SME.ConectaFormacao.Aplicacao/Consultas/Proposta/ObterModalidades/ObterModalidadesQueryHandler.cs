using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterModalidadesQueryHandler : IRequestHandler<ObterModalidadesQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterModalidadesQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(Modalidade))
                .Cast<Modalidade>()
                .Select(v => new RetornoListagemDTO
                {
                    Id = (short)v,
                    Descricao = v.Nome()
                });

            if (request.TipoFormacao == TipoFormacao.Curso)
                lista = lista.Where(t => (Modalidade)t.Id != Modalidade.Hibrido);

            return Task.FromResult(lista);
        }
    }
}
