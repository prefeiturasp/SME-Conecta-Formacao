using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class AlterarPropostaRascunhoCommandHandler : IRequestHandler<AlterarPropostaRascunhoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly ITransacao _transacao;
        private readonly IRepositorioProposta _repositorioProposta;

        public AlterarPropostaRascunhoCommandHandler(IMapper mapper, ITransacao transacao, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        public async Task<long> Handle(AlterarPropostaRascunhoCommand request, CancellationToken cancellationToken)
        {
            var proposta = await _repositorioProposta.ObterPorId(request.Id);
            if (proposta == null)
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA, System.Net.HttpStatusCode.NotFound);

            var propostaDepois = _mapper.Map<Proposta>(request.PropostaDTO);

            propostaDepois.Id = proposta.Id;
            propostaDepois.AreaPromotoraId = proposta.AreaPromotoraId;
            propostaDepois.CriadoEm = proposta.CriadoEm;
            propostaDepois.CriadoPor = proposta.CriadoPor;
            propostaDepois.CriadoLogin = proposta.CriadoLogin;
            propostaDepois.Situacao = SituacaoRegistro.Rascunho;

            var publicoAlvoAntes = await _repositorioProposta.ObterPublicoAlvoPorId(request.Id);
            var publicoAlvoDepois = _mapper.Map<IEnumerable<PropostaPublicoAlvo>>(request.PropostaDTO.PublicosAlvo);
            var publicoAlvoInserir = publicoAlvoDepois.Where(w => !publicoAlvoAntes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));
            var publicoAlvoExcluir = publicoAlvoAntes.Where(w => !publicoAlvoDepois.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));

            var funcoesEspecificasAntes = await _repositorioProposta.ObterFuncoesEspecificasPorId(request.Id);
            var funcoesEspecificasDepois = _mapper.Map<IEnumerable<PropostaFuncaoEspecifica>>(request.PropostaDTO.FuncoesEspecificas).Where(t => t.CargoFuncaoId != (long)OpcaoListagem.Outros);
            var funcoesEspecificasInserir = funcoesEspecificasDepois.Where(w => !funcoesEspecificasAntes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));
            var funcoesEspecificasExcluir = funcoesEspecificasAntes.Where(w => !funcoesEspecificasDepois.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));

            var criteriosValidacaoInscricaoAntes = await _repositorioProposta.ObterCriteriosValidacaoInscricaoPorId(request.Id);
            var criteriosValidacaoInscricaoDepois = _mapper.Map<IEnumerable<PropostaCriterioValidacaoInscricao>>(request.PropostaDTO.CriteriosValidacaoInscricao).Where(t => t.CriterioValidacaoInscricaoId != (long)OpcaoListagem.Outros);
            var criteriosValidacaoInscricaoInserir = criteriosValidacaoInscricaoDepois.Where(w => !criteriosValidacaoInscricaoAntes.Any(a => a.CriterioValidacaoInscricaoId == w.CriterioValidacaoInscricaoId));
            var criteriosValidacaoInscricaoExcluir = criteriosValidacaoInscricaoAntes.Where(w => !criteriosValidacaoInscricaoDepois.Any(a => a.CriterioValidacaoInscricaoId == w.CriterioValidacaoInscricaoId));

            var vagasRemanecentesAntes = await _repositorioProposta.ObterVagasRemacenentesPorId(request.Id);
            var vagasRemanecentesDepois = _mapper.Map<IEnumerable<PropostaVagaRemanecente>>(request.PropostaDTO.VagasRemanecentes);
            var vagasRemanecentesInserir = vagasRemanecentesDepois.Where(w => !vagasRemanecentesAntes.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));
            var vagasRemanecentesExcluir = vagasRemanecentesAntes.Where(w => !vagasRemanecentesDepois.Any(a => a.CargoFuncaoId == w.CargoFuncaoId));

            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioProposta.Atualizar(transacao, propostaDepois);

                if (publicoAlvoInserir.Any())
                    await _repositorioProposta.InserirPublicosAlvo(transacao, request.Id, publicoAlvoInserir);

                if (publicoAlvoExcluir.Any())
                    await _repositorioProposta.RemoverPublicosAlvo(transacao, publicoAlvoExcluir);

                if (funcoesEspecificasInserir.Any())
                    await _repositorioProposta.InserirFuncoesEspecificas(transacao, request.Id, funcoesEspecificasInserir);

                if (funcoesEspecificasExcluir.Any())
                    await _repositorioProposta.RemoverFuncoesEspecificas(transacao, funcoesEspecificasExcluir);

                if (criteriosValidacaoInscricaoInserir.Any())
                    await _repositorioProposta.InserirCriteriosValidacaoInscricao(transacao, request.Id, criteriosValidacaoInscricaoInserir);

                if (criteriosValidacaoInscricaoExcluir.Any())
                    await _repositorioProposta.RemoverCriteriosValidacaoInscricao(transacao, criteriosValidacaoInscricaoExcluir);

                if (vagasRemanecentesInserir.Any())
                    await _repositorioProposta.InserirVagasRemanecentes(transacao, request.Id, vagasRemanecentesInserir);

                if (vagasRemanecentesExcluir.Any())
                    await _repositorioProposta.RemoverVagasRemanecentes(transacao, vagasRemanecentesExcluir);

                transacao.Commit();

                return request.Id;
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }
        }
    }
}
