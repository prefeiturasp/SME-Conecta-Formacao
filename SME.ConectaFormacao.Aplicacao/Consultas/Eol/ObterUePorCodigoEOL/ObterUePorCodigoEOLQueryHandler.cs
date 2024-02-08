using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUePorCodigoEOLQueryHandler : IRequestHandler<ObterUePorCodigoEOLQuery, UeServicoEol>
    {
        public ObterUePorCodigoEOLQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        private readonly IServicoEol _servicoEol;
        public async Task<UeServicoEol> Handle(ObterUePorCodigoEOLQuery request, CancellationToken cancellationToken)
        {
            var ue = await _servicoEol.ObterUePorCodigo(request.UeCodigo);
            if (ue.NomeEscola.EhNulo())
                throw new NegocioException(MensagemNegocio.UE_NAO_LOCALIZADA_POR_CODIGO);

            return ue;
        }
    }
}