using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarSituacaoDaPropostaCommandHandler : IRequestHandler<AlterarSituacaoDaPropostaCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public AlterarSituacaoDaPropostaCommandHandler(IMediator mediator, IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(AlterarSituacaoDaPropostaCommand request, CancellationToken cancellationToken)
        {
            await _repositorioProposta.AtualizarSituacao(request.Id, request.SituacaoProposta);

            return true;
        }
    }
}