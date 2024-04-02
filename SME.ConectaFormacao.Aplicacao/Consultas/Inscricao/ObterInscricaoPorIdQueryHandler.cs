using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterInscricaoPorIdQueryHandler : IRequestHandler<ObterInscricaoPorIdQuery, PaginacaoResultadoComMensagemDTO<DadosListagemInscricaoDTO>>
    {
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;

        public ObterInscricaoPorIdQueryHandler(IRepositorioInscricao repositorioInscricao, IMapper mapper,IRepositorioProposta repositorioProposta)
        {
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<PaginacaoResultadoComMensagemDTO<DadosListagemInscricaoDTO>> Handle(ObterInscricaoPorIdQuery request, CancellationToken cancellationToken)
        {
            var mapeamento = new List<DadosListagemInscricaoDTO>();
            var podeInscrever = false;
            var mensagem = MensagemNegocio.INSCRICAO_NAO_ENCONTRADA;
            
            var totalDeRegistros = await _repositorioInscricao.ObterInscricaoPorIdComFiltrosTotalRegistros(request.PropostaId, request.filtros.RegistroFuncional, request.filtros.Cpf, request.filtros.NomeCursista, request.filtros.TurmaId);
            if (totalDeRegistros > 0)
            {
                var inscricoes = await _repositorioInscricao.ObterInscricaoPorIdComFiltros(request.PropostaId, request.filtros.RegistroFuncional, request.filtros.Cpf, request.filtros.NomeCursista,
                    request.filtros.TurmaId, request.NumeroPagina, request.NumeroRegistros);
                mapeamento.AddRange(_mapper.Map<IEnumerable<DadosListagemInscricaoDTO>>(inscricoes));

                var proposta = await _repositorioProposta.ObterPorId(request.PropostaId);
                
                podeInscrever = proposta.EstaEmPeriodoDeInscricao;
                
                mensagem = podeInscrever
                    ? string.Empty
                    : MensagemNegocio.AS_INSCRICOES_PARA_ESTA_PROPOSTA_NAO_ESTAO_ABERTAS;
            }

            return new PaginacaoResultadoComMensagemDTO<DadosListagemInscricaoDTO>(mapeamento, totalDeRegistros, request.NumeroRegistros, podeInscrever, mensagem);
        }
    }
}