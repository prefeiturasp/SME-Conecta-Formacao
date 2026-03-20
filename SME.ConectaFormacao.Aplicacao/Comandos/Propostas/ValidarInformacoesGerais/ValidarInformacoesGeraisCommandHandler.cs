using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarInformacoesGeraisCommandHandler : IRequestHandler<ValidarInformacoesGeraisCommand, IEnumerable<string>>
    {
        private readonly IMediator _mediator;

        public ValidarInformacoesGeraisCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<string>> Handle(ValidarInformacoesGeraisCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDTO;

            if (proposta.TipoFormacao == null)
                erros.Add(MensagemNegocio.TIPO_FORMACAO_NAO_INFORMADO);
            if (proposta.Formato == null)
                erros.Add(MensagemNegocio.FORMATO_NAO_INFORMADO);
            if (proposta.TiposInscricao.NaoPossuiElementos())
                erros.Add(MensagemNegocio.TIPO_INSCRICAO_NAO_INFORMADA);
            if (string.IsNullOrEmpty(proposta.NomeFormacao))
                erros.Add(MensagemNegocio.NOME_FORMACAO_NAO_INFORMADO);
            if (proposta.QuantidadeTurmas == 0 || proposta.QuantidadeTurmas == null)
                erros.Add(MensagemNegocio.QUANTIDADE_DE_TURMAS_NAO_INFORMADA);
            if (proposta.QuantidadeVagasTurma == 0 || proposta.QuantidadeVagasTurma == null)
                erros.Add(MensagemNegocio.QUANTIDADE_DE_VAGAS_POR_TURMAS_NAO_INFORMADA);

            if (proposta.IntegrarNoSGA.GetValueOrDefault())
            {
                var anoAtual = DateTimeExtension.HorarioBrasilia().Year;
                var parametro = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.QtdeCursistasSuportadosPorTurma, anoAtual), cancellationToken);

                if (parametro.Valor.NaoEstaPreenchido())
                    throw new NegocioException(string.Format(MensagemNegocio.PARAMETRO_QTDE_CURSISTAS_SUPORTADOS_POR_TURMA_NAO_ENCONTRADO, anoAtual));

                if (proposta.QuantidadeVagasTurma > short.Parse(parametro.Valor))
                    erros.Add(MensagemNegocio.QUANTIDADE_DE_VAGAS_SGA_MAIOR_QUE_O_PERMINTIDO.Parametros(parametro.Valor));
            }

            return erros;
        }
    }
}