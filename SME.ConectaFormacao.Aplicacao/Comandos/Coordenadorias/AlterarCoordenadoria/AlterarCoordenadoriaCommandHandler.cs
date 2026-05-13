using MediatR;
using SME.ConectaFormacao.Dominio.Comum;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Coordenadorias.AlterarCoordenadoria
{
    public class AlterarCoordenadoriaCommandHandler(IRepositorioCoordenadoria repositorio) : IRequestHandler<AlterarCoordenadoriaCommand, Resultado>
    {
        public async Task<Resultado> Handle(AlterarCoordenadoriaCommand request, CancellationToken cancellationToken)
        {
            var coordenadoria = await repositorio.ObterNaoExcluidosPorIdAsync(request.Id);
            if (coordenadoria == null)
                return Erro.NaoEncontrado("Coordenadoria não encontrada.");
            coordenadoria.Nome = request.Nome;
            coordenadoria.Sigla = request.Sigla;
            await repositorio.Atualizar(coordenadoria);
            return Resultado.DeSucesso();
        }
    }
}
