using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverAreaPromotoraCommandHandler : IRequestHandler<RemoverAreaPromotoraCommand, bool>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly ITransacao _transacao;
        private readonly ICacheDistribuido _cacheDistribuido;

        public RemoverAreaPromotoraCommandHandler(IRepositorioAreaPromotora repositorioAreaPromotora, ITransacao transacao, ICacheDistribuido cacheDistribuido)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<bool> Handle(RemoverAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.Id) ??
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            var existeProposta = await _repositorioAreaPromotora.ExistePropostaPorId(areaPromotora.Id);
            if (existeProposta)
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_POSSUI_PROPOSTA);

            var telefones = await _repositorioAreaPromotora.ObterTelefonesPorId(request.Id);

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioAreaPromotora.Remover(transacao, areaPromotora);

                if (telefones != null && telefones.Any())
                    await _repositorioAreaPromotora.RemoverTelefones(transacao, request.Id, telefones);

                transacao.Commit();

                await _cacheDistribuido.RemoverAsync(CacheDistribuidoNomes.AreaPromotora);

                return true;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
