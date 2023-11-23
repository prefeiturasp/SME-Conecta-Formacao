using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
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
            var proposta = await _repositorioProposta.ObterPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);
            
            proposta.Situacao = request.SituacaoProposta;
            proposta.GrupoGestaoId = request.GrupoGestaoId;
            
            await _repositorioProposta.Atualizar(proposta);

            return true;
        }
    }
}