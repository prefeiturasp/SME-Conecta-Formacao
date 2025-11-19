using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar : CasoDeUsoAbstrato, ICasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar
    {
        public CasoDeUsoAtualizarCargoFuncaoVinculoInscricaoCursistaTratar(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var atualizarCargoFuncaoVinculoInscricao =
                param.ObterObjetoMensagem<AtualizarCargoFuncaoVinculoInscricaoCursistaTratarDto>() ??
                throw new NegocioException(MensagemNegocio.ATUALIZACAO_VINCULO_INSCRICAO_NAO_LOCALIZADA);

            var cargosFuncoes = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(atualizarCargoFuncaoVinculoInscricao.Login));
            if (!cargosFuncoes.Any())
                return false;

            var dadosInscricao = ObterCargosBaseSobrepostoFuncaoAtividade(cargosFuncoes);
            if (atualizarCargoFuncaoVinculoInscricao.CargoCodigo != null)
                dadosInscricao = dadosInscricao.Where(c => c.Codigo == atualizarCargoFuncaoVinculoInscricao.CargoCodigo);

            await mediator.Send(new AlterarInscricaoParaUltimoCargoFuncaoVinculoCommand(atualizarCargoFuncaoVinculoInscricao.Id, dadosInscricao));
            return true;
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