﻿using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoEnviarProposta : CasoDeUsoAbstrato, ICasoDeUsoEnviarProposta
    {
        public CasoDeUsoEnviarProposta(IMediator mediator) : base(mediator)
        {
        }

        public async Task<bool> Executar(long propostaId)
        {
            var proposta = await mediator.Send(new ObterPropostaPorIdQuery(propostaId));
            if (proposta.EhNulo() || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var situacoes = new SituacaoProposta[] { SituacaoProposta.Cadastrada, SituacaoProposta.Devolvida, SituacaoProposta.AguardandoAnaliseDf };

            if (!situacoes.Contains(proposta.Situacao))
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ESTA_COMO_CADASTRADA_NEM_DEVOLVIDA);

            var existeFuncaoEspecificaOutros = await mediator.Send(new ExisteCargoFuncaoOutrosNaPropostaQuery(proposta.Id));
            var propostasTipoInscricao = await mediator.Send(new ObterPropostaTipoInscricaoPorIdQuery(proposta.Id));
            if (propostasTipoInscricao.PossuiElementos())
            {
                if (propostasTipoInscricao.Any(a => a.TipoInscricao == TipoInscricao.Automatica || a.TipoInscricao == TipoInscricao.AutomaticaJEIF) && existeFuncaoEspecificaOutros)
                    throw new NegocioException(MensagemNegocio.PROPOSTA_JEIF_COM_OUTROS);
            }

            var validarDatas = await mediator.Send(new ValidarSeDataInscricaoEhMaiorQueDataRealizacaoCommand(proposta.DataInscricaoFim, proposta.DataRealizacaoFim));
            if (!string.IsNullOrEmpty(validarDatas))
                throw new NegocioException(validarDatas);

            var situacao = await ObterSituacaoProposta(proposta);

            await mediator.Send(new EnviarPropostaCommand(propostaId, situacao));
            await mediator.Send(new SalvarPropostaMovimentacaoCommand(propostaId, situacao));

            proposta.TiposInscricao = await mediator.Send(new ObterPropostaTipoInscricaoPorIdQuery(propostaId));

            if (situacao == SituacaoProposta.Publicada && proposta.FormacaoHomologada != FormacaoHomologada.Sim)
            {
                if (proposta.TiposInscricao.Any(a => a.TipoInscricao == TipoInscricao.Automatica || a.TipoInscricao == TipoInscricao.AutomaticaJEIF))
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomatica, propostaId));
                else
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.GerarPropostaTurmaVaga, propostaId));
            }

            return true;
        }

        private async Task<SituacaoProposta> ObterSituacaoProposta(Dominio.Entidades.Proposta proposta)
        {
            if (proposta.FormacaoHomologada.EstaHomologada())
                return await ObterSituacaoHomologada(proposta);
            
            return SituacaoProposta.Publicada;
        }

        private async Task<SituacaoProposta> ObterSituacaoHomologada(Dominio.Entidades.Proposta proposta)
        {
            if (proposta.Situacao.EstaAguardandoAnaliseDf()
                && await mediator.Send(new ExistePareceristasAdicionadosNaPropostaQuery(proposta.Id)))
                return SituacaoProposta.AguardandoAnalisePeloParecerista;

            return SituacaoProposta.AguardandoAnaliseDf;
        }
    }
}