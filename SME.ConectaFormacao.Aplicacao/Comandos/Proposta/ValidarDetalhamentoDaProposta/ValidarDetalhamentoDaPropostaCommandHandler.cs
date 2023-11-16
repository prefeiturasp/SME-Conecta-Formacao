using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ValidarDetalhamentoDaPropostaCommandHandler : IRequestHandler<ValidarDetalhamentoDaPropostaCommand, IEnumerable<string>>
    {
        private const int QUANTIDADE_MINIMA_DE_PALAVRAS_CHAVES = 3;
        private const int QUANTIDADE_MAXIMA_DE_PALAVRAS_CHAVES = 5;
        public async Task<IEnumerable<string>> Handle(ValidarDetalhamentoDaPropostaCommand request, CancellationToken cancellationToken)
        {
            var erros = new List<string>();
            var proposta = request.PropostaDto;
            if (proposta.Modalidade is Modalidade.Presencial && string.IsNullOrEmpty(proposta.CargaHorariaPresencial))
                erros.Add(MensagemNegocio.CARGA_HORARIA_NAO_INFORMADA);
            if (string.IsNullOrEmpty(proposta.Justificativa))
                erros.Add(MensagemNegocio.JUSTIFICATIVA_NAO_INFORMADA);
            if (string.IsNullOrEmpty(proposta.Objetivos))
                erros.Add(MensagemNegocio.OBJETIVO_NAO_INFORMADO);
            if (string.IsNullOrEmpty(proposta.ConteudoProgramatico))
                erros.Add(MensagemNegocio.CONTEUDO_PROGRAMATICO_NAO_INFORMADO);
            if (string.IsNullOrEmpty(proposta.ProcedimentoMetadologico))
                erros.Add(MensagemNegocio.PROCEDIMENTOS_METODOLOGICOS_NAO_INFORMADO);
            if (string.IsNullOrEmpty(proposta.Referencia))
                erros.Add(MensagemNegocio.REFERENCIA_NAO_INFORMADA);
            if (proposta.PalavrasChaves.Count() < QUANTIDADE_MINIMA_DE_PALAVRAS_CHAVES || proposta.PalavrasChaves.Count() > QUANTIDADE_MAXIMA_DE_PALAVRAS_CHAVES)
                erros.Add(MensagemNegocio.PALAVRA_CHAVE_NAO_INFORMADA);

            return erros;
        }
    }
}