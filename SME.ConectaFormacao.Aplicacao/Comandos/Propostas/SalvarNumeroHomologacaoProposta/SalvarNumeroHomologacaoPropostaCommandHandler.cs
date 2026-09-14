using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarNumeroHomologacaoProposta
{
    public class SalvarNumeroHomologacaoPropostaCommandHandler(IRepositorioProposta repositorioProposta)
        : IRequestHandler<SalvarNumeroHomologacaoPropostaCommand, bool>
    {
        public async Task<bool> Handle(SalvarNumeroHomologacaoPropostaCommand request, CancellationToken cancellationToken)
        {
            var linhasAfetadas = await repositorioProposta.AtualizarNumeroHomologacao(request.PropostaId, request.NumeroHomologacao);
            return linhasAfetadas > 0;
        }
    }
}
