using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao.Comandos.Propostas.SalvarPropostaDre
{
    public class SalvarPropostaDreCommandHandler(IRepositorioProposta repositorioProposta) :
        IRequestHandler<SalvarPropostaDreCommand, bool>
    {
        public async Task<bool> Handle(SalvarPropostaDreCommand request, CancellationToken cancellationToken)
        {
            var dresAntes = await repositorioProposta.ObterDrePorId(request.PropostaId);

            var dresInserir = request.Dres.Where(w => !dresAntes.Any(a => a.DreId == w.DreId));
            var dresExcluir = dresAntes.Where(w => !request.Dres.Any(a => a.DreId == w.DreId));

            if (dresInserir.Any())
                await repositorioProposta.InserirDres(request.PropostaId, dresInserir);

            if (dresExcluir.Any())
                await repositorioProposta.RemoverDres(dresExcluir);

            return true;
        }
    }
}
