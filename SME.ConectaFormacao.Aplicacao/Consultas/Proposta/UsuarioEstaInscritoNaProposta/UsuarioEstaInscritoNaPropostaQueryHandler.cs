using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class UsuarioEstaInscritoNaPropostaQueryHandler : IRequestHandler<UsuarioEstaInscritoNaPropostaQuery, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;

        public UsuarioEstaInscritoNaPropostaQueryHandler(IRepositorioInscricao repositorioInscricao)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
        }

        public async Task<bool> Handle(UsuarioEstaInscritoNaPropostaQuery request, CancellationToken cancellationToken)
        {
            var possuiInscricaoNaProposta = await _repositorioInscricao.UsuarioEstaInscritoNaProposta(request.UsuarioId, request.PropostaId);

            if (possuiInscricaoNaProposta)
                throw new NegocioException(MensagemNegocio.USUARIO_JA_INSCRITO_NA_PROPOSTA);

            return false;
        }
    }
}
