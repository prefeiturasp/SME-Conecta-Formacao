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
    public class SalvarInscricaoCommandHandler : IRequestHandler<SalvarInscricaoCommand, long>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly ITransacao _transacao;

        public SalvarInscricaoCommandHandler(IMapper mapper, IMediator mediator, IRepositorioInscricao repositorioInscricao, IRepositorioProposta repositorioProposta, ITransacao transacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<long> Handle(SalvarInscricaoCommand request, CancellationToken cancellationToken)
        {
            var usuarioLogado = await _mediator.Send(ObterUsuarioLogadoQuery.Instancia, cancellationToken) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            var inscricao = _mapper.Map<Inscricao>(request.InscricaoDTO);
            inscricao.UsuarioId = usuarioLogado.Id;
            inscricao.Situacao = SituacaoInscricao.EmAnalise;

            await MapearCargoFuncao(cancellationToken, inscricao);

            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                                throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            await ValidarExisteInscricaoNaProposta(propostaTurma.PropostaId, inscricao.UsuarioId);

            if (usuarioLogado.Tipo != TipoUsuario.Externo)
                await ValidarCargoFuncao(propostaTurma.PropostaId, inscricao.CargoId, inscricao.FuncaoId, cancellationToken);

            await ValidarDre(inscricao.PropostaTurmaId, inscricao.CargoDreCodigo, inscricao.FuncaoDreCodigo, cancellationToken);

            await ValidarEmail(usuarioLogado.Login, usuarioLogado.Email, request.InscricaoDTO.Email, cancellationToken);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            return await PersistirInscricao(proposta.FormacaoHomologada == FormacaoHomologada.Sim, inscricao);
        }

        private async Task MapearCargoFuncao(CancellationToken cancellationToken, Inscricao inscricao)
        {
            var codigosFuncoesEol = inscricao.FuncaoCodigo.EstaPreenchido() ? new List<long> { long.Parse(inscricao.FuncaoCodigo) } : Enumerable.Empty<long>();
            var codigosCargosEol = inscricao.CargoCodigo.EstaPreenchido() ? new List<long> { long.Parse(inscricao.CargoCodigo) } : Enumerable.Empty<long>();
            if (codigosFuncoesEol.PossuiElementos() || codigosCargosEol.PossuiElementos())
            {
                var cargosFuncoes = await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol), cancellationToken);

                inscricao.CargoId = cargosFuncoes?.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Cargo)?.Id;

                inscricao.FuncaoId = cargosFuncoes?.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Funcao)?.Id;
            }
        }

        private async Task ValidarEmail(string login, string emailUsuario, string novoEmail, CancellationToken cancellationToken)
        {
            if (emailUsuario != novoEmail)
            {
                var emailValidar = novoEmail.ToLower();

                if (!emailValidar.EmailEhValido())
                    throw new NegocioException(string.Format(MensagemNegocio.EMAIL_INVALIDO, emailValidar));

                if (!emailValidar.ToLower().Contains("@sme") && !emailValidar.ToLower().Contains("@edu.sme"))
                    throw new NegocioException(MensagemNegocio.AREA_PROMOTORA_EMAIL_FORA_DOMINIO_REDE_DIRETA);

                await _mediator.Send(new AlterarEmailServicoAcessosCommand(login, emailValidar), cancellationToken);
            }
        }

        private async Task ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var possuiInscricaoNaProposta = await _repositorioInscricao.ExisteInscricaoNaProposta(propostaId, usuarioId);
            if (possuiInscricaoNaProposta)
                throw new NegocioException(MensagemNegocio.USUARIO_JA_INSCRITO_NA_PROPOSTA);
        }

        private async Task ValidarCargoFuncao(long propostaId, long? cargoId, long? funcaoId, CancellationToken cancellationToken)
        {
            var cargosProposta = await _mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId), cancellationToken);
            var funcaoAtividadeProposta = await _mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId), cancellationToken);

            if (cargosProposta.PossuiElementos())
            {
                if (cargoId.HasValue && !cargosProposta.Any(a => a.CargoFuncaoId == cargoId))
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
            }

            if (funcaoAtividadeProposta.PossuiElementos())
            {
                if (funcaoId.HasValue && !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == funcaoId))
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
            }
        }

        private async Task ValidarDre(long propostaTurmaId, string cargoDreCodigo, string funcaoDreCodigo, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                if ((cargoDreCodigo.EstaPreenchido() && !dres.Any(a => a.Dre.Codigo == cargoDreCodigo)) ||
                    (funcaoDreCodigo.EstaPreenchido() && !dres.Any(a => a.Dre.Codigo == funcaoDreCodigo)))
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
