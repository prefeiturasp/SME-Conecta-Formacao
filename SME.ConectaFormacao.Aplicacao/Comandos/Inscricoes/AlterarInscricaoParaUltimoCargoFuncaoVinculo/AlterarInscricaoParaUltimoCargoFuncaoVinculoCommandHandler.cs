using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandHandler : IRequestHandler<AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand, bool>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IMediator _mediator;

        public AlterarInscricaoParaUltimoCargoFuncaoVinculoCommandHandler(IRepositorioInscricao repositorioInscricao, IMediator mediator)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand request, CancellationToken cancellationToken)
        {
            var ultimoCargo = request.DadosInscricao.MaxBy(c => c.DataInicio);
            var cargoCodigo = ultimoCargo?.Codigo;
            if (cargoCodigo == null || ultimoCargo == null)
                return false;

            var codigosCargosEol = new List<long> { long.Parse(cargoCodigo) };

            var codigoFuncoesEol = Enumerable.Empty<long>();
            var ultimaFuncao = ultimoCargo.Funcoes.MaxBy(c => c.DataInicio);
            if (ultimaFuncao is { Codigo: not null }) codigoFuncoesEol = new List<long> { long.Parse(ultimaFuncao.Codigo) };

            var tipoVinculo = ultimoCargo.TipoVinculo;
            if (tipoVinculo <= 0)
                return false;

            var inscricao = await _repositorioInscricao.ObterPorId(request.Id) ??
                            throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_ENCONTRADA,
                                System.Net.HttpStatusCode.NotFound);

            if (inscricao.CargoId == null)
            {
                var cargosFuncoes = await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigoFuncoesEol), cancellationToken);
                var cargo = cargosFuncoes.FirstOrDefault(c => c.Tipo == CargoFuncaoTipo.Cargo);
                if (cargo == null)
                    return false;

                inscricao.CargoId = cargo.Id;
                inscricao.CargoCodigo = cargoCodigo;
                inscricao.CargoDreCodigo = ultimoCargo.DreCodigo;
                inscricao.CargoUeCodigo = ultimoCargo.UeCodigo;

                var funcao = cargosFuncoes.FirstOrDefault(c => c.Tipo == CargoFuncaoTipo.Funcao);
                if (funcao != null && ultimaFuncao != null)
                {
                    inscricao.FuncaoId = funcao.Id;
                    inscricao.FuncaoCodigo = ultimaFuncao.Codigo;
                    inscricao.FuncaoUeCodigo = ultimaFuncao.UeCodigo;
                    inscricao.FuncaoDreCodigo = ultimaFuncao.DreCodigo;
                    tipoVinculo = ultimaFuncao.TipoVinculo;
                }
            }

            inscricao.TipoVinculo = tipoVinculo;
            await _repositorioInscricao.Atualizar(inscricao);

            return true;
        }
    }
}