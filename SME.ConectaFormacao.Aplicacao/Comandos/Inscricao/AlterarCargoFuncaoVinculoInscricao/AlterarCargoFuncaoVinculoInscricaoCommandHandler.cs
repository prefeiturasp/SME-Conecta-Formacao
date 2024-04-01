using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarCargoFuncaoVinculoInscricaoCommandHandler : IRequestHandler<AlterarCargoFuncaoVinculoInscricaoCommand, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;

        public AlterarCargoFuncaoVinculoInscricaoCommandHandler(IRepositorioInscricao repositorioInscricao,
            IMediator mediator)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(AlterarCargoFuncaoVinculoInscricaoCommand request,
            CancellationToken cancellationToken)
        {
            var inscricao = await _repositorioInscricao.ObterPorId(request.Id) ??
                            throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA,
                                System.Net.HttpStatusCode.NotFound);

            var cargos =
                await _mediator.Send(
                    new ObterCargoFuncaoPorCodigoEolQuery(
                        new List<long> { long.Parse(request.AlterarCargoFuncaoVinculoIncricao.CargoCodigo) },
                        Enumerable.Empty<long>()),
                    cancellationToken);

            var cargo = cargos.FirstOrDefault(c => c.Tipo == CargoFuncaoTipo.Cargo);
            if (cargo == null)
                throw new NegocioException(MensagemNegocio.CARGO_NAO_ENCONTRATO_PARA_ALTERACAO_VINCULO_INSCRICAO);

            inscricao.CargoId = cargo.Id;
            inscricao.CargoCodigo = request.AlterarCargoFuncaoVinculoIncricao.CargoCodigo;
            inscricao.FuncaoCodigo = null;
            inscricao.FuncaoDreCodigo = null;
            inscricao.FuncaoUeCodigo = null;
            inscricao.FuncaoId = null;
            inscricao.TipoVinculo = request.AlterarCargoFuncaoVinculoIncricao.TipoVinculo;

            await _repositorioInscricao.Atualizar(inscricao);

            return true;
        }
    }
}