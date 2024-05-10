using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarPropostaPareceristaConsideracaoCommandHandler : IRequestHandler<SalvarPropostaPareceristaConsideracaoCommand, RetornoDTO>
    {
        private readonly IRepositorioPropostaPareceristaConsideracao _repositorioPropostaPareceristaConsideracao;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IMapper _mapper;

        public SalvarPropostaPareceristaConsideracaoCommandHandler(IMapper mapper, IRepositorioPropostaPareceristaConsideracao repositorioPropostaPareceristaConsideracao,IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioPropostaPareceristaConsideracao = repositorioPropostaPareceristaConsideracao ?? throw new ArgumentNullException(nameof(repositorioPropostaPareceristaConsideracao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<RetornoDTO> Handle(SalvarPropostaPareceristaConsideracaoCommand request, CancellationToken cancellationToken)
        {
            if (request.PropostaPareceristaConsideracaoCadastroDto.Id > 0)
            {
                var alterarPropostaParecer = await _repositorioPropostaPareceristaConsideracao.ObterPorId(request.PropostaPareceristaConsideracaoCadastroDto.Id.Value);
                
                alterarPropostaParecer.Descricao = request.PropostaPareceristaConsideracaoCadastroDto.Descricao;
            
                await _repositorioPropostaPareceristaConsideracao.Atualizar(alterarPropostaParecer);

                return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "alterado"), alterarPropostaParecer.Id);
            }
            
            var propostaPareceristaConsideracao = _mapper.Map<PropostaPareceristaConsideracao>(request.PropostaPareceristaConsideracaoCadastroDto);
            
            var parecerista = await _repositorioProposta.ObterPareceristaPorPropostaIdRegistroFuncional(request.PropostaPareceristaConsideracaoCadastroDto.PropostaId, request.Login) ??
                               throw new NegocioException(MensagemNegocio.USUARIO_LOGADO_NAO_E_PARECERISTA_DA_PROPOSTA);

            propostaPareceristaConsideracao.PropostaPareceristaId = parecerista.Id;
            
            var id = await _repositorioPropostaPareceristaConsideracao.Inserir(propostaPareceristaConsideracao);
            
            return RetornoDTO.RetornarSucesso(string.Format(MensagemNegocio.PARECER_X_COM_SUCESSO, "inserido"), id);
        }
    }
}