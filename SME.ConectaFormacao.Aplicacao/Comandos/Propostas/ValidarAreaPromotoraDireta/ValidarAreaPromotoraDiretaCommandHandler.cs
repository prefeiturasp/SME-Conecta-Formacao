using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarAreaPromotoraCommandHandler : IRequestHandler<ValidarAreaPromotoraCommand>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ValidarAreaPromotoraCommandHandler(IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task Handle(ValidarAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.AreaPromotoraId);

            if (areaPromotora.EhDireta() && request.IntegrarNoGSA.EhNulo())
                throw new NegocioException(MensagemNegocio.INTEGRAR_NO_SGA_EH_OBRIGATORIO_QUANDO_AREA_PROMOTORA_DIRETA);
        }
    }
}
