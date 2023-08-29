using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverAreaPromotoraCommandHandler : IRequestHandler<RemoverAreaPromotoraCommand, bool>
    {
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly ITransacao _transacao;

        public RemoverAreaPromotoraCommandHandler(IRepositorioAreaPromotora repositorioAreaPromotora, ITransacao transacao)
        {
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<bool> Handle(RemoverAreaPromotoraCommand request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.Id);
            if (areaPromotora == null)
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, HttpStatusCode.NotFound);

            var telefones = await _repositorioAreaPromotora.ObterTelefonesPorId(request.Id);

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioAreaPromotora.Remover(transacao, areaPromotora);

                if (telefones != null && telefones.Any())
                    await _repositorioAreaPromotora.RemoverTelefones(transacao, request.Id, telefones);

                transacao.Commit();

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
