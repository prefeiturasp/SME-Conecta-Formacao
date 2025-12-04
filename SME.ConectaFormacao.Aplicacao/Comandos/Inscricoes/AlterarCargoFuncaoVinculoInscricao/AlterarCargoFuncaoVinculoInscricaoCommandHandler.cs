using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Eol;

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

            var cargosFuncoesEol = await ObterCargosFuncoesEol();
            if (!cargosFuncoesEol.Any())
                return false;

            var dadosInscricao = ObterCargosBaseSobrepostoFuncaoAtividade(cargosFuncoesEol);
            var tipoVinculo = request.AlterarCargoFuncaoVinculoIncricao.TipoVinculo;

            var cargoInscricao = dadosInscricao.FirstOrDefault(c =>
                c.Codigo == request.AlterarCargoFuncaoVinculoIncricao.CargoCodigo &&
                c.TipoVinculo == tipoVinculo);

            var cargoCodigo = cargoInscricao?.Codigo;
            if (cargoCodigo == null || cargoInscricao == null)
                return false;

            var funcaoInscricao = cargoInscricao.Funcoes.FirstOrDefault(c => c.TipoVinculo == tipoVinculo);

            var cargosFuncoes = await ObterCargosFuncoes(cargoCodigo, funcaoInscricao?.Codigo);
            var cargo = cargosFuncoes.FirstOrDefault(c => c.Tipo == CargoFuncaoTipo.Cargo);
            if (cargo == null)
                throw new NegocioException(MensagemNegocio.CARGO_NAO_ENCONTRATO_PARA_ALTERACAO_VINCULO_INSCRICAO);

            inscricao.CargoId = cargo.Id;
            inscricao.CargoCodigo = cargoCodigo;
            inscricao.CargoDreCodigo = cargoInscricao.DreCodigo;
            inscricao.CargoUeCodigo = cargoInscricao.UeCodigo;
            inscricao.TipoVinculo = cargoInscricao.TipoVinculo;

            if (inscricao.FuncaoId != null && funcaoInscricao != null)
            {
                var funcao = cargosFuncoes.FirstOrDefault(c => c.Tipo == CargoFuncaoTipo.Funcao);
                if (funcao != null)
                {
                    inscricao.FuncaoCodigo = funcaoInscricao.Codigo;
                    inscricao.FuncaoDreCodigo = funcaoInscricao.DreCodigo;
                    inscricao.FuncaoUeCodigo = funcaoInscricao.UeCodigo;
                    inscricao.FuncaoId = funcao.Id;
                    inscricao.TipoVinculo = funcaoInscricao.TipoVinculo;
                }
            }

            await ValidarCargoFuncao(inscricao, cancellationToken);

            await _repositorioInscricao.Atualizar(inscricao);
            return true;
        }

        private async Task ValidarCargoFuncao(Inscricao inscricao, CancellationToken cancellationToken)
        {
            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken);
            var cargosProposta = await _mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaTurma.PropostaId), cancellationToken);
            var funcaoAtividadeProposta = await _mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaTurma.PropostaId), cancellationToken);

            if (cargosProposta.PossuiElementos())
            {
                if (inscricao.CargoId.HasValue && !cargosProposta.Any(a => a.CargoFuncaoId == inscricao.CargoId))
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
            }

            if (funcaoAtividadeProposta.PossuiElementos())
            {
                if (inscricao.FuncaoId.HasValue && !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == inscricao.FuncaoId))
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
            }
        }

        private async Task<IEnumerable<CargoFuncao>> ObterCargosFuncoes(string cargoCodigo, string? funcaoCodigo)
        {
            var codigosCargosEol = new List<long> { long.Parse(cargoCodigo) };

            var codigosFuncoesEol = Enumerable.Empty<long>();
            if (!string.IsNullOrEmpty(funcaoCodigo))
                codigosFuncoesEol = new List<long> { long.Parse(funcaoCodigo) };

            return await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol));
        }

        private async Task<IEnumerable<CursistaCargoServicoEol>> ObterCargosFuncoesEol()
        {
            var usuarioLogado = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            return await _mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(usuarioLogado.Login));
        }

        private static IEnumerable<DadosInscricaoCargoEol> ObterCargosBaseSobrepostoFuncaoAtividade(IEnumerable<CursistaCargoServicoEol> cargosFuncoesEol)
        {
            var usuarioCargos = new List<DadosInscricaoCargoEol>();
            foreach (var cargoFuncaoEol in cargosFuncoesEol)
            {
                var item = new DadosInscricaoCargoEol
                {
                    Codigo = cargoFuncaoEol.CdCargoBase.ToString(),
                    Descricao = cargoFuncaoEol.CargoBase,
                    DreCodigo = cargoFuncaoEol.CdDreCargoBase,
                    UeCodigo = cargoFuncaoEol.CdUeCargoBase,
                    TipoVinculo = cargoFuncaoEol.TipoVinculoCargoBase ?? 0,
                    DataInicio = cargoFuncaoEol.DataInicioCargoBase
                };

                if (cargoFuncaoEol.CdFuncaoAtividade.HasValue)
                {
                    item.Funcoes.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdFuncaoAtividade.ToString(),
                        Descricao = cargoFuncaoEol.FuncaoAtividade,
                        DreCodigo = cargoFuncaoEol.CdDreFuncaoAtividade,
                        UeCodigo = cargoFuncaoEol.CdUeFuncaoAtividade,
                        TipoVinculo = cargoFuncaoEol.TipoVinculoFuncaoAtividade ?? 0,
                        DataInicio = cargoFuncaoEol.DataInicioFuncaoAtividade
                    });
                }
                usuarioCargos.Add(item);

                if (cargoFuncaoEol.CdCargoSobreposto.HasValue)
                {
                    usuarioCargos.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdCargoSobreposto.ToString(),
                        Descricao = cargoFuncaoEol.CargoSobreposto,
                        DreCodigo = cargoFuncaoEol.CdDreCargoSobreposto,
                        UeCodigo = cargoFuncaoEol.CdUeCargoSobreposto,
                        TipoVinculo = cargoFuncaoEol.TipoVinculoCargoSobreposto ?? 0,
                        DataInicio = cargoFuncaoEol.DataInicioCargoSobreposto
                    });
                }
            }

            return usuarioCargos;
        }
    }
}