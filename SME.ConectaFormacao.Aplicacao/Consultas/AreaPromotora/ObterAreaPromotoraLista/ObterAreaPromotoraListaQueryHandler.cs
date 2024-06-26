﻿using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraListaQueryHandler : IRequestHandler<ObterAreaPromotoraListaQuery, IEnumerable<RetornoListagemDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterAreaPromotoraListaQueryHandler(IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora, ICacheDistribuido cacheDistribuido)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<IEnumerable<RetornoListagemDTO>> Handle(ObterAreaPromotoraListaQuery request, CancellationToken cancellationToken)
        {
            var areasPromotoras = await _cacheDistribuido.ObterAsync(CacheDistribuidoNomes.AreaPromotora, () => _repositorioAreaPromotora.ObterLista());

            if (request.AreaPromotoraIdUsuarioLogado.GetValueOrDefault() > 0)
                areasPromotoras = areasPromotoras.Where(t => t.Id == request.AreaPromotoraIdUsuarioLogado.Value);

            if (request.Tipo.NaoEhNulo())
                areasPromotoras = areasPromotoras.Where(t => t.Tipo.EhRedeParceria());

            return _mapper.Map<IEnumerable<RetornoListagemDTO>>(areasPromotoras);
        }
    }
}
