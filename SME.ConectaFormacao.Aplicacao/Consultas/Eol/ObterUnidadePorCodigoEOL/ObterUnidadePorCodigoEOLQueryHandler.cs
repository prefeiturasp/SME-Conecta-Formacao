using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Eol.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterUnidadePorCodigoEOLQueryHandler : IRequestHandler<ObterUnidadePorCodigoEOLQuery, UnidadeEol>
    {
        public ObterUnidadePorCodigoEOLQueryHandler(IServicoEol servicoEol)
        {
            _servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        private readonly IServicoEol _servicoEol;
        public async Task<UnidadeEol> Handle(ObterUnidadePorCodigoEOLQuery request, CancellationToken cancellationToken)
        {
            var ue = await _servicoEol.ObterUnidadePorCodigoEol(request.UnidadeCodigo);
            if (ue.NomeUnidade.EhNulo())
                throw new NegocioException(MensagemNegocio.UNIDADE_NAO_LOCALIZADA_POR_CODIGO);

            return ue;
        }
    }
}