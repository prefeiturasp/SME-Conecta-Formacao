using MediatR;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.RemoverCoordenadoria
{
    public class RemoverCoordenadoriaCommandHandler(IRepositorioCoordenadoria repositorio) :
        IRequestHandler<RemoverCoordenadoriaCommand, Resultado>
    {
        public async Task<Resultado> Handle(RemoverCoordenadoriaCommand request, CancellationToken cancellationToken)
        {
            var entidade = await repositorio.ObterNaoExcluidosPorIdAsync(request.Id);
            if (entidade == null) return Erro.NaoEncontrado("Coordenadoria não encontrada.");
            entidade.Excluido = true;
            await repositorio.Atualizar(entidade);
            return Resultado.DeSucesso();
        }
    }
}