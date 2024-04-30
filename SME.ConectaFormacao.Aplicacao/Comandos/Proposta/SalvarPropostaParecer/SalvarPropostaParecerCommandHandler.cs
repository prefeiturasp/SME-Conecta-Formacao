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
        private readonly IRepositorioPropostaParecer _repositorioPropostaParecer;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public SalvarPropostaParecerCommandHandler(IMapper mapper, IRepositorioPropostaParecer repositorioPropostaParecer,IMediator _mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaParecer = repositorioPropostaParecer ?? throw new ArgumentNullException(nameof(repositorioPropostaParecer));
        }

        public async Task<RetornoDTO> Handle(SalvarPropostaParecerCommand request, CancellationToken cancellationToken)
        {
            if (request.PropostaParecerCadastroDto.Id > 0)
            {
                var alterarPropostaParecer = await _repositorioPropostaParecer.ObterPorId(request.PropostaParecerCadastroDto.Id.Value);
                
                alterarPropostaParecer.Descricao = request.PropostaParecerCadastroDto.Descricao;
            
                await _repositorioPropostaParecer.Atualizar(alterarPropostaParecer);

                return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "alterado"), alterarPropostaParecer.Id);
            }
            
            var propostaParecer = _mapper.Map<PropostaParecer>(request.PropostaParecerCadastroDto);
            
            propostaParecer.Situacao = SituacaoParecer.PendenteEnvioParecerPeloParecerista;
            propostaParecer.UsuarioPareceristaId = request.UsuarioLogadoId;
            
            var id = await _repositorioPropostaParecer.Inserir(propostaParecer);
            
            return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "inserido"), id);
        }
    }
}