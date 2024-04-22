﻿using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.AreaPromotora;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterPropostaCompletaPorIdQueryHandler : IRequestHandler<ObterPropostaCompletaPorIdQuery, PropostaCompletoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IRepositorioProposta _repositorioProposta;
        private readonly IRepositorioArquivo _repositorioArquivo;
        private readonly IRepositorioPropostaMovimentacao _repositorioPropostaMovimentacao;
        private readonly IRepositorioAreaPromotora _repositorioAreaPromotora;

        public ObterPropostaCompletaPorIdQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta,
            IRepositorioArquivo repositorioArquivo, IRepositorioPropostaMovimentacao repositorioPropostaMovimentacao,
            IRepositorioAreaPromotora repositorioAreaPromotora)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
            _repositorioArquivo = repositorioArquivo ?? throw new ArgumentNullException(nameof(repositorioArquivo));
            _repositorioPropostaMovimentacao = repositorioPropostaMovimentacao ?? throw new ArgumentNullException(nameof(repositorioPropostaMovimentacao));
            _repositorioAreaPromotora = repositorioAreaPromotora ?? throw new ArgumentNullException(nameof(repositorioAreaPromotora));
        }

        public async Task<PropostaCompletoDTO> Handle(ObterPropostaCompletaPorIdQuery request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id);
            if (proposta == null || proposta.Excluido)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            proposta.Dres = await _repositorioProposta.ObterDrePorId(request.Id);
            proposta.PublicosAlvo = await _repositorioProposta.ObterPublicoAlvoPorId(request.Id);
            proposta.FuncoesEspecificas = await _repositorioProposta.ObterFuncoesEspecificasPorId(request.Id);
            proposta.CriteriosValidacaoInscricao = await _repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.Id);
            proposta.VagasRemanecentes = await _repositorioProposta.ObterVagasRemacenentesPorId(request.Id);
            proposta.PalavrasChaves = await _repositorioProposta.ObterPalavrasChavesPorId(request.Id);
            proposta.Modalidades = await _repositorioProposta.ObterModalidadesPorId(request.Id);
            proposta.AnosTurmas = await _repositorioProposta.ObterAnosTurmasPorId(request.Id);
            proposta.ComponentesCurriculares = await _repositorioProposta.ObterComponentesCurricularesPorId(request.Id);
            proposta.CriterioCertificacao = await _repositorioProposta.ObterCriterioCertificacaoPorPropostaId(request.Id);
            proposta.Turmas = await _repositorioProposta.ObterTurmasPorId(request.Id);
            proposta.TiposInscricao = await _repositorioProposta.ObterTiposInscricaoPorId(request.Id);
            proposta.Movimentacao = await _repositorioPropostaMovimentacao.ObterUltimoParecerPropostaId(request.Id);
            proposta.AreaPromotora = await _repositorioAreaPromotora.ObterPorId(proposta.AreaPromotoraId);
            proposta.UltimaJustificativaDevolucao = await _repositorioPropostaMovimentacao.ObterUltimaJustificativaDevolucao(request.Id);

            foreach (var turma in proposta.Turmas)
                turma.Dres = await _repositorioProposta.ObterPropostaTurmasDresPorPropostaTurmaId(turma.Id);

            var propostaCompletaDTO = _mapper.Map<PropostaCompletoDTO>(proposta);
            propostaCompletaDTO.Auditoria = _mapper.Map<AuditoriaDTO>(proposta);
            propostaCompletaDTO.AreaPromotora = _mapper.Map<PropostaAreaPromotoraDTO>(proposta.AreaPromotora);

            if (proposta.ArquivoImagemDivulgacaoId.HasValue)
            {
                var arquivo = await _repositorioArquivo.ObterPorId(proposta.ArquivoImagemDivulgacaoId.Value);
                propostaCompletaDTO.ArquivoImagemDivulgacao = _mapper.Map<PropostaImagemDivulgacaoDTO>(arquivo);
            }

            return propostaCompletaDTO;
        }
    }
}
