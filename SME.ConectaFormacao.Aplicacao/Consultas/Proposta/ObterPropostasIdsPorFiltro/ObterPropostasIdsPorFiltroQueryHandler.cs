﻿using MediatR;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostasIdsPorFiltroQueryHandler : IRequestHandler<ObterPropostasIdsPorFiltroQuery, IEnumerable<long>>
    {
        private readonly IRepositorioProposta _repositorioProposta;

        public ObterPropostasIdsPorFiltroQueryHandler(IRepositorioProposta repositorioProposta)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public Task<IEnumerable<long>> Handle(ObterPropostasIdsPorFiltroQuery request, CancellationToken cancellationToken)
        {
            return _repositorioProposta.ObterListagemFormacoesPorFiltro(
                request.FiltroListagemFormacaoDTO.PublicosAlvosIds,
                request.FiltroListagemFormacaoDTO.Titulo,
                request.FiltroListagemFormacaoDTO.AreasPromotorasIds,
                request.FiltroListagemFormacaoDTO.DataFinal,
                request.FiltroListagemFormacaoDTO.DataFinal,
                request.FiltroListagemFormacaoDTO.FormatosIds,
                request.FiltroListagemFormacaoDTO.PalavrasChavesIds);
        }
    }
}