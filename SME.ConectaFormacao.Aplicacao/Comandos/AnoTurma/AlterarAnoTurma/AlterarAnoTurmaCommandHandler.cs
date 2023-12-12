using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarAnoTurmaCommandHandler : IRequestHandler<AlterarAnoTurmaCommand, bool>
{
    private readonly IRepositorioAnoTurma _repositorioAnoTurma;

    public AlterarAnoTurmaCommandHandler(IRepositorioAnoTurma repositorioAnoTurma)
    {
        _repositorioAnoTurma = repositorioAnoTurma ?? throw new ArgumentNullException(nameof(repositorioAnoTurma));
    }

    public async Task<bool> Handle(AlterarAnoTurmaCommand request, CancellationToken cancellationToken)
    {
        await _repositorioAnoTurma.Atualizar(request.AnoTurma);
        return true;
    }
}