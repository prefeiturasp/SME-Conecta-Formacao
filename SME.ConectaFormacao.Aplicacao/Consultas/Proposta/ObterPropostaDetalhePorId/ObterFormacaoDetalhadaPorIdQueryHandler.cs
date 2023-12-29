using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using SME.ConectaFormacao.Infra.Servicos.Cache;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFormacaoDetalhadaPorIdQueryHandler : IRequestHandler<ObterFormacaoDetalhadaPorIdQuery, RetornoFormacaoDetalhadaDTO>
    {
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ICacheDistribuido _cacheDistribuido;

        public ObterFormacaoDetalhadaPorIdQueryHandler(IRepositorioProposta repositorioProposta, IMapper mapper, IMediator mediator, ICacheDistribuido cacheDistribuido)
        {
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cacheDistribuido = cacheDistribuido ?? throw new ArgumentNullException(nameof(cacheDistribuido));
        }

        public async Task<RetornoFormacaoDetalhadaDTO> Handle(ObterFormacaoDetalhadaPorIdQuery request, CancellationToken cancellationToken)
        {
            var chaveRedis = CacheDistribuidoNomes.FormacaoDetalhada.Parametros(request.Id);
            var retornoFormacaoDetalhadaDto = await _cacheDistribuido.ObterObjetoAsync<RetornoFormacaoDetalhadaDTO>(chaveRedis);

            if (retornoFormacaoDetalhadaDto.EhNulo())
            {
                var formacaoDetalhada = await _repositorioProposta.ObterFormacaoDetalhadaPorId(request.Id) ?? throw new NegocioException(MensagemNegocio.FORMACAO_NAO_ENCONTRADA, HttpStatusCode.NotFound);

                retornoFormacaoDetalhadaDto = _mapper.Map<RetornoFormacaoDetalhadaDTO>(formacaoDetalhada);

                if (formacaoDetalhada.ArquivoImagemDivulgacao.NaoEhNulo())
                    retornoFormacaoDetalhadaDto.ImagemUrl = await _mediator.Send(new ObterEnderecoArquivoServicoArmazenamentoQuery(formacaoDetalhada.ArquivoImagemDivulgacao.NomeArquivoFisico, false), cancellationToken);

                await _cacheDistribuido.SalvarAsync(chaveRedis, retornoFormacaoDetalhadaDto);
            }

            if (retornoFormacaoDetalhadaDto.FormacaoHomologada != Dominio.Enumerados.FormacaoHomologada.Sim)
            {
                var turmasComVaga = await _mediator.Send(new ObterPropostaTurmasComVagasPorIdQuery(request.Id), cancellationToken);
                foreach (var turma in retornoFormacaoDetalhadaDto.Turmas)
                {
                    turma.InscricaoEncerrada = !turmasComVaga.Any(t => t.Id == turma.Id);
                }
            }

            return retornoFormacaoDetalhadaDto;
        }
    }
}