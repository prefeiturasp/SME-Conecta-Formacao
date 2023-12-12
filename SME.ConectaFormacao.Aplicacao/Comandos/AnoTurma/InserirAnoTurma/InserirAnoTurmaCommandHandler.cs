using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class InserirAnoTurmaCommandHandler : IRequestHandler<InserirAnoTurmaCommand, long>
{
    private readonly IRepositorioAnoTurma _repositorioAnoTurma;

    public InserirAnoTurmaCommandHandler(IRepositorioAnoTurma repositorioAnoTurma)
    {
        _repositorioAnoTurma = repositorioAnoTurma ?? throw new ArgumentNullException(nameof(repositorioAnoTurma));
    }

    public async Task<long> Handle(InserirAnoTurmaCommand request, CancellationToken cancellationToken)
    {
        return await _repositorioAnoTurma.Inserir(request.AnoTurma);
    }
}