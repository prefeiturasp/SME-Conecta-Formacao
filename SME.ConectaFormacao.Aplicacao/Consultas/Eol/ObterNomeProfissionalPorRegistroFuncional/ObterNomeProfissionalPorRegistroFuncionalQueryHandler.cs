using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterNomeProfissionalPorRegistroFuncionalQueryHandler : IRequestHandler<ObterNomeProfissionalPorRegistroFuncionalQuery, string>
    {
        private readonly IServicoEol _servicoEol;

        public ObterNomeProfissionalPorRegistroFuncionalQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<string> Handle(ObterNomeProfissionalPorRegistroFuncionalQuery request, CancellationToken cancellationToken)
        {
            return await _servicoEol.ObterNomeProfissionalPorRegistroFuncional(request.RegistroFuncional);
        }
    }
}