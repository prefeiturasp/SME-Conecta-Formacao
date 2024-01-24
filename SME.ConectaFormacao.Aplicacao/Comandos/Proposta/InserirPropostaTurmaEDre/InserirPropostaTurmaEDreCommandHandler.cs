using MediatR;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaTurmaEDreCommandHandler : IRequestHandler<InserirPropostaTurmaEDreCommand, long>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;

        public InserirPropostaTurmaEDreCommandHandler(ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long> Handle(InserirPropostaTurmaEDreCommand request, CancellationToken cancellationToken)
        {
            var transacao = _transacao.Iniciar();

            try
            {
                await _repositorioProposta.InserirTurma(request.Turma);

                var dres = request?.Turma?.Dres.ToList();

                foreach (var propostaTurmaDre in dres)
                    propostaTurmaDre.PropostaTurmaId = request.Turma.Id;

                await _repositorioProposta.InserirPropostaTurmasDres(dres);

                transacao.Commit();

                return request.Turma.Id;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
