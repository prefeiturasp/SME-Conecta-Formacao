using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class InserirComponenteCurricularCommandHandler : IRequestHandler<InserirComponenteCurricularCommand, long>
{
    private readonly IRepositorioComponenteCurricular _repositorioComponenteCurricular;

    public InserirComponenteCurricularCommandHandler(IRepositorioComponenteCurricular repositorioComponenteCurricular)
    {
        _repositorioComponenteCurricular = repositorioComponenteCurricular ?? throw new ArgumentNullException(nameof(repositorioComponenteCurricular));
    }

    public async Task<long> Handle(InserirComponenteCurricularCommand request, CancellationToken cancellationToken)
    {
        return await _repositorioComponenteCurricular.Inserir(request.ComponenteCurricular);
    }
}