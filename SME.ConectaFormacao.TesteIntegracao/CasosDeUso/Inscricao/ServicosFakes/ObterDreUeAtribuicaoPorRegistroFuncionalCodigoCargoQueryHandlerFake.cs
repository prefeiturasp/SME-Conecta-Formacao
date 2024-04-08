using MediatR;
using SME.ConectaFormacao.Aplicacao;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.TesteIntegracao.CasosDeUso.Inscricao.ServicosFakes
{
    public class ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryHandlerFake : IRequestHandler<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery, IEnumerable<DreUeAtribuicaoServicoEol>>
    {
        public Task<IEnumerable<DreUeAtribuicaoServicoEol>> Handle(ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery request, CancellationToken cancellationToken)
        {
            var retorno = new List<DreUeAtribuicaoServicoEol>()
            {
                new DreUeAtribuicaoServicoEol()
                {
                    DreCodigo = "1",
                    UeCodigo = "1"
                }
            };

            return Task.FromResult(retorno.AsEnumerable());
        }
    }
}
