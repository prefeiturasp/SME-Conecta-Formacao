using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaTipoInscricaoCommandHandler : IRequestHandler<SalvarPropostaTipoInscricaoCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public SalvarPropostaTipoInscricaoCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(SalvarPropostaTipoInscricaoCommand request, CancellationToken cancellationToken)
        {
            var tiposInscricaoAntes = await _repositorioProposta.ObterTiposInscricaoPorId(request.PropostaId);

            var tiposInscricaoInserir = request.TiposInscricao.Where(w => !tiposInscricaoAntes.Any(a => a.TipoInscricao == w.TipoInscricao));
            var tiposInscrocaoExcluir = tiposInscricaoAntes.Where(w => !request.TiposInscricao.Any(a => a.TipoInscricao == w.TipoInscricao));

            if (tiposInscricaoInserir.Any())
                await _repositorioProposta.InserirTiposInscricao(request.PropostaId, tiposInscricaoInserir);

            if (tiposInscrocaoExcluir.Any())
                await _repositorioProposta.RemoverTiposInscricao(tiposInscrocaoExcluir);

            return true;
        }
    }
}
