using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInformacoesInscricoesEstaoAbertasPorIdQueryHandler : IRequestHandler<ObterInformacoesInscricoesEstaoAbertasPorIdQuery, PodeInscreverMensagemDTO>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterInformacoesInscricoesEstaoAbertasPorIdQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PodeInscreverMensagemDTO> Handle(ObterInformacoesInscricoesEstaoAbertasPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.PropostaId);

            if (proposta.EhNulo())
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            var estaEmPeriodoDeInscricao = proposta.EstaEmPeriodoDeInscricao;
            var tiposInscricao = await _repositorioProposta.ObterTiposInscricaoPorId(request.PropostaId);

            return new PodeInscreverMensagemDTO
            {
                PodeInscrever = estaEmPeriodoDeInscricao,
                NomeFormacao = proposta.NomeFormacao,
                TiposInscricao = tiposInscricao.Select(t => (int)t.TipoInscricao).ToList(),
                Mensagem = estaEmPeriodoDeInscricao
                    ? string.Empty
                    : MensagemNegocio.AS_INSCRICOES_PARA_ESTA_PROPOSTA_NAO_ESTAO_ABERTAS
            };
        }
    }
}