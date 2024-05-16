using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterSituacaoUsuarioRedeParceriaQueryHandler : IRequestHandler<ObterSituacaoUsuarioRedeParceriaQuery, IEnumerable<RetornoListagemDTO>>
    {
        public Task<IEnumerable<RetornoListagemDTO>> Handle(ObterSituacaoUsuarioRedeParceriaQuery request, CancellationToken cancellationToken)
        {
            var lista = Enum.GetValues(typeof(SituacaoUsuario))
               .Cast<SituacaoUsuario>()
               .Select(v => new RetornoListagemDTO
               {
                   Id = (short)v,
                   Descricao = v.Nome()
               });

            lista = lista.Where(t => (SituacaoUsuario)t.Id != SituacaoUsuario.AguardandoValidacaoEmail);

            return Task.FromResult(lista);
        }
    }
}
