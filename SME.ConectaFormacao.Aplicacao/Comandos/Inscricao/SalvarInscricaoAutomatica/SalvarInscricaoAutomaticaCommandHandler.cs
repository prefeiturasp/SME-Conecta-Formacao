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

        public SalvarInscricaoAutomaticaCommandHandler(IMapper mapper, IMediator mediator, IRepositorioInscricao repositorioInscricao, ITransacao transacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<long> Handle(SalvarInscricaoAutomaticaCommand request, CancellationToken cancellationToken)
        {
            var propostaId = request.InscricaoAutomaticaDTO.PropostaId;

            var inscricao = _mapper.Map<Inscricao>(request.InscricaoAutomaticaDTO);
            inscricao.Situacao = SituacaoInscricao.AguardandoAnalise;
            inscricao.Origem = OrigemInscricao.Automatica;

            if (await ValidarExisteInscricaoNaProposta(propostaId, inscricao.UsuarioId))
                return default;

            await MapearCargoFuncao(cancellationToken, inscricao);

            await ValidarCargoFuncao(propostaId, inscricao.CargoId, inscricao.FuncaoId, cancellationToken);

            await ValidarDre(inscricao.PropostaTurmaId, inscricao.CargoDreCodigo, inscricao.FuncaoDreCodigo, cancellationToken);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaId), cancellationToken);

            var transacao = _transacao.Iniciar();
            try
            {
                if(proposta.FormacaoHomologada.NaoEstaHomologada())
                    inscricao.Situacao = SituacaoInscricao.Confirmada;

                await _repositorioInscricao.Inserir(inscricao);

                if (proposta.FormacaoHomologada.NaoEstaHomologada())
                {
                    var confirmada = await _repositorioInscricao.ConfirmarInscricaoVaga(inscricao);
                    if (!confirmada)
                        throw new NegocioException("não foi possível realizar a inscrição por falta de vaga na turma");
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

        private async Task MapearCargoFuncao(CancellationToken cancellationToken, Inscricao inscricao)
        {
            var codigosFuncoesEol = inscricao.FuncaoCodigo.EstaPreenchido() ? new List<long> { long.Parse(inscricao.FuncaoCodigo) } : Enumerable.Empty<long>();
            var codigosCargosEol = inscricao.CargoCodigo.EstaPreenchido() ? new List<long> { long.Parse(inscricao.CargoCodigo) } : Enumerable.Empty<long>();

            var cargosFuncoes = await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol), cancellationToken);

            inscricao.CargoId = cargosFuncoes?.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Cargo)?.Id;

            inscricao.FuncaoId = cargosFuncoes?.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Funcao)?.Id;
        }

        private async Task ValidarCargoFuncao(long propostaId, long? cargoId, long? funcaoId, CancellationToken cancellationToken)
        {
            var cargosProposta = await _mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId), cancellationToken);
            var funcaoAtividadeProposta = await _mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId), cancellationToken);

            if (cargosProposta.PossuiElementos())
            {
                if (cargoId.HasValue && !cargosProposta.Any(a => a.CargoFuncaoId == cargoId))
                    throw new NegocioException(string.Format(MensagemNegocio.USUARIO_NAO_INSCRITO_AUTOMATICAMENTE_NAO_POSSUI_PUBLICO_ALVO_NA_FORMACAO, $"Proposta: {propostaId} - Cargo: {cargoId}"));
            }

            if (funcaoAtividadeProposta.PossuiElementos())
            {
                if (funcaoId.HasValue && !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == funcaoId))
                    throw new NegocioException(string.Format(MensagemNegocio.USUARIO_NAO_INSCRITO_AUTOMATICAMENTE_NAO_POSSUI_FUNCAO_ESPECIFICA_NA_FORMACAO, $"Proposta: {propostaId} - Função: {funcaoId}"));
            }
        }

        private async Task<bool> ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            return await _repositorioInscricao.UsuarioEstaInscritoNaProposta(propostaId, usuarioId);
        }

        private async Task ValidarDre(long propostaTurmaId, string cargoDreCodigo, string? funcaoDreCodigo, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                if (!dres.Any(a => a.DreCodigo == cargoDreCodigo || a.DreCodigo == funcaoDreCodigo))
                    throw new NegocioException(string.Format(MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_AUTOMATICO, $"PropostaTurma: {0} - Cargo: {cargoDreCodigo} - Função: {funcaoDreCodigo}"));
            }
        }
    }
}
