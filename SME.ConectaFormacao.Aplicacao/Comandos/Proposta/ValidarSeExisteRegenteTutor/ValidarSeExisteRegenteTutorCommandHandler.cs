using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeExisteRegenteTutorCommandHandler : IRequestHandler<ValidarSeExisteRegenteTutorCommand, IEnumerable<string>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarSeExisteRegenteTutorCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<IEnumerable<string>> Handle(ValidarSeExisteRegenteTutorCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var totalRegentes = await _repositorioProposta.ObterTotalRegentes(request.PropostaId);
            if (totalRegentes == 0)
                erros.Add(MensagemNegocio.NAO_EXISTE_NENHUM_REGENTE);
            var totalTutores = await _repositorioProposta.ObterTotalTutores(request.PropostaId);
            if (totalTutores == 0)
                erros.Add(MensagemNegocio.NAO_EXISTE_NENHUM_TUTOR);
            return erros;
        }
    }
}