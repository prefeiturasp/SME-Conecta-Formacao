using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryHandler : IRequestHandler<ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery, IEnumerable<DreUeAtribuicaoServicoEol>>
    {
        private readonly IServicoEol _servicoEol;

        public ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public Task<IEnumerable<DreUeAtribuicaoServicoEol>> Handle(ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery request, CancellationToken cancellationToken)
        {
            return _servicoEol.ObterDreUeAtribuicaoPorFuncionarioCargo(request.RegistroFuncional, long.Parse(request.CodigoCargo));
        }
    }
}
