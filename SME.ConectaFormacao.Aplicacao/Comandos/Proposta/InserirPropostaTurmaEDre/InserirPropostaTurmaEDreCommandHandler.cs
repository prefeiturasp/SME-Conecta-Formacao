using MediatR;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class InserirPropostaTurmaEDreCommandHandler : IRequestHandler<InserirPropostaTurmaEDreCommand, long>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly ITransacao _transacao;

        public InserirPropostaTurmaEDreCommandHandler(ITransacao transacao,IRepositorioProposta repositorioProposta)
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

                foreach (var propostaTurmaDre in request.Turma.Dres)
                    propostaTurmaDre.PropostaTurmaId = request.Turma.Id;
                
                await _repositorioProposta.InserirPropostaTurmasDres(request.Turma.Dres);

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
