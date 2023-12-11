using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandHandler : IRequestHandler<ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task Handle(ValidarSeJaExisteRegenteTurmaAntesDeCadastrarCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var turmasExistentes = await _repositorioProposta.ObterTurmasJaExistenteParaRegente(request.PropostaId, request.NomeRegente, request.RegistroFuncional, request.TurmaIds);
            foreach (var turma in request.TurmaIds)
                if (turmasExistentes.Contains(turma))
                    erros.Add(string.Format(MensagemNegocio.JA_EXISTE_ESSA_TURMA_PARA_ESSE_REGENTE, request.NomeRegente, turma));

            if (erros.Any())
                throw new NegocioException(erros);
        }
    }
}