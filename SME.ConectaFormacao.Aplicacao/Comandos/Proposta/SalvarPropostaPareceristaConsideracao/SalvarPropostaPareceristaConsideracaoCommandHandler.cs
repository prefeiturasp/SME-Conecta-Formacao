using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaConsideracaoCommandHandler : IRequestHandler<SalvarPropostaPareceristaConsideracaoCommand, RetornoDTO>
    {
        private readonly IRepositorioPropostaParecerConsideracao _repositorioPropostaParecerConsideracao;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public SalvarPropostaPareceristaConsideracaoCommandHandler(IMapper mapper, IRepositorioPropostaParecerConsideracao repositorioPropostaParecerConsideracao,IMediator _mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaParecerConsideracao = repositorioPropostaParecerConsideracao ?? throw new ArgumentNullException(nameof(repositorioPropostaParecerConsideracao));
        }

        public async Task<RetornoDTO> Handle(SalvarPropostaPareceristaConsideracaoCommand request, CancellationToken cancellationToken)
        {
            if (request.PropostaPareceristaConsideracaoCadastroDto.Id > 0)
            {
                var alterarPropostaParecer = await _repositorioPropostaParecerConsideracao.ObterPorId(request.PropostaPareceristaConsideracaoCadastroDto.Id.Value);
                
                alterarPropostaParecer.Descricao = request.PropostaPareceristaConsideracaoCadastroDto.Descricao;
            
                await _repositorioPropostaParecerConsideracao.Atualizar(alterarPropostaParecer);

                return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "alterado"), alterarPropostaParecer.Id);
            }
            
            var propostaPareceristaConsideracao = _mapper.Map<PropostaPareceristaConsideracao>(request.PropostaPareceristaConsideracaoCadastroDto);
            propostaPareceristaConsideracao.PropostaPareceristaId = request.PropostaPareceristaConsideracaoCadastroDto.PropostaPareceristaId;
            
            var id = await _repositorioPropostaParecerConsideracao.Inserir(propostaPareceristaConsideracao);
            
            return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "inserido"), id);
        }
    }
}