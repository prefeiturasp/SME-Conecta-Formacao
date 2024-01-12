using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoAutomaticaCommandHandler : IRequestHandler<SalvarInscricaoAutomaticaCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly ITransacao _transacao;

        public SalvarInscricaoAutomaticaCommandHandler(IMapper mapper, IMediator mediator, IRepositorioInscricao repositorioInscricao, IRepositorioProposta repositorioProposta, ITransacao transacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<long> Handle(SalvarInscricaoAutomaticaCommand request, CancellationToken cancellationToken)
        {
            var inscricao = _mapper.Map<Inscricao>(request.InscricaoAutomaticaDTO);
            inscricao.Situacao = SituacaoInscricao.Confirmada;

            if (await ValidarExisteInscricaoNaProposta(request.InscricaoAutomaticaDTO.PropostaId, inscricao.UsuarioId))
                return default;

            await ValidarDre(inscricao.PropostaTurmaId, inscricao.CargoDreCodigo, inscricao.FuncaoDreCodigo, cancellationToken);
           
            return await PersistirInscricao(request.InscricaoAutomaticaDTO.EhFormacaoHomologada, inscricao);
        }

        private async Task<bool> ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            return await _repositorioInscricao.ExisteInscricaoNaProposta(propostaId, usuarioId);
        }

        private async Task ValidarDre(long propostaTurmaId, string cargoDreCodigo, string funcaoDreCodigo, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);

            if (dres.PossuiElementos())
            {
                if ((cargoDreCodigo.EstaPreenchido() && !dres.Any(a => a.DreCodigo.ToString().Equals(cargoDreCodigo)))
                    || (funcaoDreCodigo.EstaPreenchido() && !dres.Any(a => a.DreCodigo.ToString().Equals(funcaoDreCodigo))))
                    throw new NegocioException(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA);
            }
        }

        private async Task<long> PersistirInscricao(bool formacaoHomologada, Inscricao inscricao)
        {
            var transacao = _transacao.Iniciar();
            try
            {
                await _repositorioInscricao.Inserir(inscricao);

                if (!formacaoHomologada)
                {
                    bool confirmada = await _repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                    if (!confirmada)
                        throw new NegocioException(MensagemNegocio.INSCRICAO_NAO_CONFIRMADA_POR_FALTA_DE_VAGA);

                    inscricao.Situacao = SituacaoInscricao.Confirmada;
                    await _repositorioInscricao.Atualizar(inscricao);
                }

                transacao.Commit();

                return inscricao.Id;
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
