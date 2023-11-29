using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoGrupoGestaoDaPropostaCommandHandler : IRequestHandler<AlterarSituacaoGrupoGestaoDaPropostaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public AlterarSituacaoGrupoGestaoDaPropostaCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(AlterarSituacaoGrupoGestaoDaPropostaCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.AtualizarSituacaoGrupoGestao(request.Id, request.SituacaoProposta, request.GrupoGestaoId);

            return true;
        }
    }
}