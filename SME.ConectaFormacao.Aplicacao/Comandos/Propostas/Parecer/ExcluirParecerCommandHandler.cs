using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ExcluirParecerCommandHandler : IRequestHandler<ExcluirParecerCommand, bool>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ExcluirParecerCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<bool> Handle(ExcluirParecerCommand request, CancellationToken cancellationToken)
        {
            var parecer = await _repositorioProposta.ObterParecerPorId(request.ParecerId);

            if (parecer == null)
                throw new NegocioException(MensagemNegocio.PARECER_NAO_ENCONTRADO);

            return await _repositorioProposta.RemoverPropostaParecer(parecer);
        }
    }
}