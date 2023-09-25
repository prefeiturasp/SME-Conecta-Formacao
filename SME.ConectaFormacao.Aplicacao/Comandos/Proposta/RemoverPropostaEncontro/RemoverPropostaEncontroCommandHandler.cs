using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class RemoverPropostaEncontroCommandHandler : IRequestHandler<RemoverPropostaEncontroCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public RemoverPropostaEncontroCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(RemoverPropostaEncontroCommand request, CancellationToken cancellationToken)
        {
            var encontro = await _repositorioProposta.ObterEncontroPorId(request.Id);
            await _repositorioProposta.RemoverEncontros(new PropostaEncontro[] { encontro });
            return true;
        }
    }
}
