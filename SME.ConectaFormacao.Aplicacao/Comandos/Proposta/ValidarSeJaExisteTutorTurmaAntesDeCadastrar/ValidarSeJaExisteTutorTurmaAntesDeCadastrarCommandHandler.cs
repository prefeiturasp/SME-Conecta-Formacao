using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandHandler : IRequestHandler<ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommandHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task Handle(ValidarSeJaExisteTutorTurmaAntesDeCadastrarCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var turmasExistentes = await _repositorioProposta.ObterTurmasJaExistenteParaTutor(request.RegistroFuncional, request.Cpf, request.NomeTutor, request.TurmaIds);
            foreach (var turma in request.TurmaIds)
            {
                var turmaExiste = turmasExistentes.FirstOrDefault(a => a.Id.Equals(turma));
                if (turmaExiste.NaoEhNulo())
                    erros.Add(string.Format(MensagemNegocio.JA_EXISTE_ESSA_TURMA_PARA_ESSE_TURTOR, request.NomeTutor, turmaExiste.Nome));
            }

            if (erros.Any())
                throw new NegocioException(erros);
        }
    }
}