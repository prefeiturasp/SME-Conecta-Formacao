using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterAreaPromotoraPorIdQueryHandler : IRequestHandler<ObterAreaPromotoraPorIdQuery, AreaPromotoraCompletoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterAreaPromotoraPorIdQueryHandler(IMapper mapper, IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<AreaPromotoraCompletoDTO> Handle(ObterAreaPromotoraPorIdQuery request, CancellationToken cancellationToken)
        {
            var areaPromotora = await _repositorioAreaPromotora.ObterPorId(request.Id);
            if (areaPromotora == null || areaPromotora.Excluido)
                throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            areaPromotora.Telefones = await _repositorioAreaPromotora.ObterTelefonesPorId(request.Id);

            var retorno = _mapper.Map<AreaPromotoraCompletoDTO>(areaPromotora);
            retorno.Auditoria = _mapper.Map<AuditoriaDTO>(areaPromotora);

            return retorno;
        }
    }
}
