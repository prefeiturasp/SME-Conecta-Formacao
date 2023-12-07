using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarComponenteCurricularCommandHandler : IRequestHandler<AlterarComponenteCurricularCommand, bool>
{
    private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;

    public AlterarComponenteCurricularCommandHandler(IRepositorioComponenteCurricular repositorioComponenteCurricular)
    {
        _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
    }

    public async Task<bool> Handle(AlterarComponenteCurricularCommand request, CancellationToken cancellationToken)
    {
       await _repositorioComponenteCurricular.Atualizar(request.ComponenteCurricular);
       return true;
    }
}