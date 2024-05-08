using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaParecerCommandHandler : IRequestHandler<SalvarPropostaParecerCommand, RetornoDTO>
    {
        private readonly IRepositorioPropostaPareceristaConsideracao _repositorioPropostaParecerConsideracao;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public SalvarPropostaParecerCommandHandler(IMapper mapper, IRepositorioPropostaPareceristaConsideracao repositorioPropostaParecerConsideracao,IMediator _mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaParecerConsideracao = repositorioPropostaParecerConsideracao ?? throw new ArgumentNullException(nameof(repositorioPropostaParecerConsideracao));
        }

        public async Task<RetornoDTO> Handle(SalvarPropostaParecerCommand request, CancellationToken cancellationToken)
        {
            if (request.PropostaParecerCadastroDto.Id > 0)
            {
                var alterarPropostaParecer = await _repositorioPropostaParecerConsideracao.ObterPorId(request.PropostaParecerCadastroDto.Id.Value);
                
                alterarPropostaParecer.Descricao = request.PropostaParecerCadastroDto.Descricao;
            
                await _repositorioPropostaParecerConsideracao.Atualizar(alterarPropostaParecer);

                return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "alterado"), alterarPropostaParecer.Id);
            }
            
            var propostaParecer = _mapper.Map<PropostaPareceristaConsideracao>(request.PropostaParecerCadastroDto);
            
            //TODO
            // propostaParecer.Situacao = SituacaoParecerista.PendenteEnvioParecerPeloParecerista;
            // propostaParecer.UsuarioPareceristaId = request.UsuarioLogadoId;
            
            var id = await _repositorioPropostaParecerConsideracao.Inserir(propostaParecer);
            
            return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "inserido"), id);
        }
    }
}