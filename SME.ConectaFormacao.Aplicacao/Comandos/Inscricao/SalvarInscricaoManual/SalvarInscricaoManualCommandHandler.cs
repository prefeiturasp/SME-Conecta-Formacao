using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class SalvarInscricaoManualCommandHandler : IRequestHandler<SalvarInscricaoManualCommand, RetornoDTO>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IRepositorioInscricao _repositorioInscricao;
        private readonly ITransacao _transacao;

        public SalvarInscricaoManualCommandHandler(IMapper mapper, IMediator mediator, IRepositorioInscricao repositorioInscricao, ITransacao transacao)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _repositorioInscricao = repositorioInscricao ?? throw new ArgumentNullException(nameof(repositorioInscricao));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
        }

        public async Task<RetornoDTO> Handle(SalvarInscricaoManualCommand request, CancellationToken cancellationToken)
        {
            var usuario = await ObterUsuarioPorLogin(request.InscricaoManualDTO, cancellationToken) ??
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

            if (usuario.Tipo.EhInterno() && request.InscricaoManualDTO.CargoCodigo.NaoEstaPreenchido())
                throw new NegocioException(MensagemNegocio.INFORME_O_CARGO);

            var inscricao = _mapper.Map<Inscricao>(request.InscricaoManualDTO);
            inscricao.UsuarioId = usuario.Id;
            inscricao.Situacao = SituacaoInscricao.AguardandoAnalise;
            inscricao.Origem = OrigemInscricao.Manual;

            await MapearCargoFuncao(inscricao, cancellationToken);

            var propostaTurma = await _mediator.Send(new ObterPropostaTurmaPorIdQuery(inscricao.PropostaTurmaId), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

            var proposta = await _mediator.Send(new ObterPropostaPorIdQuery(propostaTurma.PropostaId), cancellationToken) ??
                    throw new NegocioException(MensagemNegocio.PROPOSTA_NAO_ENCONTRADA);

            if (usuario.Tipo.EhInterno())
            {
                if (proposta.FuncoesEspecificas != null && proposta.FuncoesEspecificas.Any())
                    await ValidarCargoFuncao(propostaTurma.PropostaId, inscricao.CargoId, inscricao.FuncaoId, cancellationToken);

                var possuiErros = await ValidarSeDreUsuarioInternoPossuiErros(usuario.Login, inscricao, cancellationToken);
                if (!request.InscricaoManualDTO.PodeContinuar && possuiErros)
                    throw new NegocioException(request.EhTransferencia ? MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_TRANSFERENCIA : MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL);
            }
            else
            {
                var possuiErros = await ValidarSeDreUsuarioExternoPossuiErros(inscricao.PropostaTurmaId, usuario.CodigoEolUnidade, cancellationToken);
                if (!request.InscricaoManualDTO.PodeContinuar && possuiErros)
                    throw new NegocioException(request.EhTransferencia ? MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_TRANSFERENCIA : MensagemNegocio.USUARIO_SEM_LOTACAO_NA_DRE_DA_TURMA_INSCRICAO_MANUAL);
            }

            if (!request.EhTransferencia)
            {
                await ValidarExisteInscricaoNaProposta(propostaTurma.PropostaId, inscricao.UsuarioId);
                ValidaPeriodoDeInscricao(proposta);
            }

            return await PersistirInscricao(proposta.FormacaoHomologada == FormacaoHomologada.Sim, inscricao);

        }

        private static void ValidaPeriodoDeInscricao(Proposta proposta)
        {
            var dataAtual = DateTimeExtension.HorarioBrasilia().Date;
            if (proposta.DataInscricaoInicio.EhNulo() ||
                !(dataAtual >= proposta.DataInscricaoInicio.GetValueOrDefault().Date && dataAtual <= proposta.DataInscricaoFim.GetValueOrDefault().Date))
                throw new NegocioException(MensagemNegocio.INSCRICAO_FORA_DO_PERIODO_INSCRICAO);
        }

        private async Task<Usuario> ObterUsuarioPorLogin(InscricaoManualDTO inscricaoManualDTO, CancellationToken cancellationToken)
        {
            var login = inscricaoManualDTO.ProfissionalRede ? inscricaoManualDTO.RegistroFuncional : inscricaoManualDTO.Cpf;

            var usuario = await _mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()), cancellationToken);
            if (usuario.NaoEhNulo())
                return usuario;

            if (inscricaoManualDTO.ProfissionalRede)
            {
                var dadosUsuario = await _mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login), cancellationToken);
                if (dadosUsuario.EhNulo())
                    return default;

                usuario = _mapper.Map<Usuario>(dadosUsuario);
                usuario.Cpf = inscricaoManualDTO.Cpf.SomenteNumeros();
                usuario.Tipo = TipoUsuario.Interno;

                await _mediator.Send(new SalvarUsuarioCommand(usuario), cancellationToken);
            }

            return usuario;
        }

        private async Task MapearCargoFuncao(Inscricao inscricao, CancellationToken cancellationToken)
        {
            var codigosFuncoesEol = inscricao.FuncaoCodigo.EstaPreenchido() ? new List<long> { long.Parse(inscricao.FuncaoCodigo) } : Enumerable.Empty<long>();
            var codigosCargosEol = inscricao.CargoCodigo.EstaPreenchido() ? new List<long> { long.Parse(inscricao.CargoCodigo) } : Enumerable.Empty<long>();
            if (codigosFuncoesEol.PossuiElementos() || codigosCargosEol.PossuiElementos())
            {
                var cargosFuncoes = await _mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(codigosCargosEol, codigosFuncoesEol), cancellationToken);

                inscricao.CargoId = cargosFuncoes.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Cargo)?.Id;
                inscricao.FuncaoId = cargosFuncoes.FirstOrDefault(f => f.Tipo == CargoFuncaoTipo.Funcao)?.Id;
            }
        }

        private async Task ValidarCargoFuncao(long propostaId, long? cargoId, long? funcaoId, CancellationToken cancellationToken)
        {
            var temErroCargo = false;
            var temErroFuncao = false;
            var cargosProposta = await _mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId), cancellationToken);
            var funcaoAtividadeProposta = await _mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId), cancellationToken);

            if (cargosProposta.PossuiElementos())
            {
                var cargoFuncaoOutros = await _mediator.Send(ObterCargoFuncaoOutrosQuery.Instancia(), cancellationToken);
                var cargoEhOutros = cargosProposta.Any(t => t.CargoFuncaoId == cargoFuncaoOutros.Id);

                if (cargoId.HasValue && !cargoEhOutros && !cargosProposta.Any(a => a.CargoFuncaoId == cargoId))
                    temErroCargo = true;

            }

            if (funcaoAtividadeProposta.PossuiElementos())
            {
                if (funcaoId.HasValue && !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == funcaoId))
                    temErroFuncao = true;
            }

            if (temErroCargo && temErroFuncao)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);

            if (!funcaoAtividadeProposta.PossuiElementos() && temErroCargo)
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO);
        }

        private async Task<bool> ValidarSeDreUsuarioInternoPossuiErros(string registroFuncional, Inscricao inscricao, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(inscricao.PropostaTurmaId), cancellationToken);
            var existeTodos = dres.Any(t => t.Dre.Todos);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var dreUeAtribuicoes = await _mediator.Send(new ObterDreUeAtribuicaoPorRegistroFuncionalCodigoCargoQuery(registroFuncional, inscricao.CargoCodigo), cancellationToken);
                if (dreUeAtribuicoes.PossuiElementos())
                {
                    var dreUeAtribuicao = dreUeAtribuicoes.FirstOrDefault(f => dres.Any(d => d.DreCodigo == f.DreCodigo));
                    if (dreUeAtribuicao.EhNulo())
                        dreUeAtribuicao = dreUeAtribuicoes.FirstOrDefault();

                    inscricao.CargoDreCodigo = dreUeAtribuicao.DreCodigo;
                    inscricao.CargoUeCodigo = dreUeAtribuicao.UeCodigo;
                }

                if ((inscricao.CargoDreCodigo.EstaPreenchido() && !dres.Any(a => a.Dre.Codigo == inscricao.CargoDreCodigo)) ||
                    (inscricao.FuncaoDreCodigo.EstaPreenchido() && !dres.Any(a => a.Dre.Codigo == inscricao.FuncaoDreCodigo)))
                    return true;

                return false;
            }

            return !existeTodos;
        }

        private async Task<bool> ValidarSeDreUsuarioExternoPossuiErros(long propostaTurmaId, string codigoEolUnidade, CancellationToken cancellationToken)
        {
            var dres = await _mediator.Send(new ObterPropostaTurmaDresPorPropostaTurmaIdQuery(propostaTurmaId), cancellationToken);
            dres = dres.Where(t => !t.Dre.Todos);
            if (dres.PossuiElementos())
            {
                var unidade = await _mediator.Send(new ObterUnidadePorCodigoEOLQuery(codigoEolUnidade), cancellationToken);

                var codigo = unidade.Tipo == Infra.Servicos.Eol.UnidadeEolTipo.Escola ? unidade.CodigoReferencia : unidade.Codigo;
                if (!dres.Any(t => t.Dre.Codigo == codigo))
                    return true;

                return false;
            }
            return true;
        }

        private async Task ValidarExisteInscricaoNaProposta(long propostaId, long usuarioId)
        {
            var possuiInscricaoNaProposta = await _repositorioInscricao.UsuarioEstaInscritoNaProposta(propostaId, usuarioId);
            if (possuiInscricaoNaProposta)
                throw new NegocioException(MensagemNegocio.USUARIO_JA_INSCRITO_NA_PROPOSTA);
        }

        private async Task<RetornoDTO> PersistirInscricao(bool formacaoHomologada, Inscricao inscricao)
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

                return RetornoDTO.RetornarSucesso(MensagemNegocio.INSCRICAO_MANUAL_REALIZADA_COM_SUCESSO, inscricao.Id);
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
