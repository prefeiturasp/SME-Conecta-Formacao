using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao;

public class AlterarVinculoInscricaoCommandHandler : IRequestHandler<AlterarVinculoInscricaoCommand, bool>
{
    private readonly IRepositorioInscricao _repositorioInscricao;

    public AlterarVinculoInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao)
    {
        _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
    }

    public async Task<bool> Handle(AlterarVinculoInscricaoCommand request, CancellationToken cancellationToken)
    {
        var inscricao = await _repositorioInscricao.ObterPorId(request.Id) ??
                        throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA,
                            System.Net.HttpStatusCode.NotFound);
        
        inscricao.TipoVinculo = request.TipoVinculo;
        await _repositorioInscricao.Atualizar(inscricao);

        return true;
    }
}