using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarImportacaoRegistro;
using SME.ConectaFormacao.Aplicacao.Comandos.ImportacaoInscricao.AlterarSituacaoImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;
using SME.ConectaFormacao.Infra.Servicos.Utilitarios;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoImportacaoInscricaoCursistaValidarItem(IMediator mediator, IMapper mapper) :
        CasoDeUsoAbstrato(mediator), ICasoDeUsoImportacaoInscricaoCursistaValidarItem
    {
        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDto>() ??
                throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            try
            {
                var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaImportacaoDto>()!;

                var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorNomeQuery(importacaoInscricaoCursista.Turma, importacaoArquivoRegistro.PropostaId)) ??
                    throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);

                var usuario = await ObterUsuarioPorLogin(importacaoInscricaoCursista) ??
                    throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);

                if (usuario.Tipo.EhInterno() && string.IsNullOrWhiteSpace(importacaoInscricaoCursista.Vinculo))
                    throw new NegocioException(MensagemNegocio.ATUALIZACAO_VINCULO_INSCRICAO_NAO_LOCALIZADA);

                var tipoVinculo = usuario.Tipo.EhInterno() ? int.Parse(importacaoInscricaoCursista.Vinculo ?? "0") : default;

                var inscricao = new Inscricao()
                {
                    PropostaTurmaId = propostaTurma.Id,
                    UsuarioId = usuario.Id,
                    Situacao = SituacaoInscricao.AguardandoAnalise,
                    Origem = OrigemInscricao.Manual,
                    TipoVinculo = tipoVinculo,
                };

                var alterarImportacaoRegistroDto = new AlterarImportacaoRegistroDto(importacaoArquivoRegistro.Id, importacaoInscricaoCursista.ObjetoParaJson(), SituacaoImportacaoArquivoRegistro.Validado, null);

                if (usuario.Tipo.EhInterno())
                {
                    // variável inscricao é alterada aqui
                    var resultado = await MapearValidarCargoFuncao(inscricao, usuario.Login, propostaTurma.PropostaId, tipoVinculo);
                    importacaoInscricaoCursista.Inscricao = inscricao;
                    alterarImportacaoRegistroDto = alterarImportacaoRegistroDto with { Situacao = resultado.Sucesso ?
                        SituacaoImportacaoArquivoRegistro.Validado :
                        SituacaoImportacaoArquivoRegistro.Erro,
                        Erro = resultado.MensagemErro,
                        Conteudo = importacaoInscricaoCursista.ObjetoParaJson()
                    };
                }

                await mediator.Send(new AlterarImportacaoRegistroCommand(alterarImportacaoRegistroDto));
            }
            catch (NegocioException e)
            {
                await mediator.Send(new AlterarSituacaoImportacaoArquivoRegistroCommand(importacaoArquivoRegistro.Id, SituacaoImportacaoArquivoRegistro.Erro, e.Message));
            }

            await AlterarSituacaoArquivo(importacaoArquivoRegistro.ImportacaoArquivoId);

            return true;
        }

        private async Task<ResultadoMapeamento> MapearValidarCargoFuncao(Inscricao inscricao, string login, long propostaId, int tipoVinculo)
        {
            var temErroCargo = false;
            var temErroFuncao = false;
            var cargoFuncaoUsuarioEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(login));

            cargoFuncaoUsuarioEol = cargoFuncaoUsuarioEol.Where(t =>
               t.TipoVinculoCargoSobreposto == tipoVinculo ||
               t.TipoVinculoCargoBase == tipoVinculo ||
               t.TipoVinculoFuncaoAtividade == tipoVinculo);

            var cargosProposta = await mediator.Send(new ObterPropostaPublicosAlvosPorIdQuery(propostaId));
            if (cargosProposta.PossuiElementos())
            {
                foreach (var cargoEol in cargoFuncaoUsuarioEol)
                {
                    long codigoCargo = 0;
                    string codigoDre, codigoUe;

                    if (cargoEol.CdCargoSobreposto.HasValue && cargoEol.TipoVinculoCargoSobreposto == tipoVinculo)
                    {
                        codigoCargo = cargoEol.CdCargoSobreposto.Value;
                        codigoDre = cargoEol.CdDreCargoSobreposto;
                        codigoUe = cargoEol.CdUeCargoSobreposto;
                    }
                    else if (cargoEol.CdCargoBase.HasValue && cargoEol.TipoVinculoCargoBase == tipoVinculo)
                    {
                        codigoCargo = cargoEol.CdCargoBase.Value;
                        codigoDre = cargoEol.CdDreCargoBase;
                        codigoUe = cargoEol.CdUeCargoBase;
                    }
                    else
                        continue;

                    var cargoFuncao = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery([codigoCargo], []));

                    var cargoId = cargoFuncao.FirstOrDefault(t => t.Tipo == CargoFuncaoTipo.Cargo)?.Id;
                    if (cargosProposta.Any(a => a.CargoFuncaoId == cargoId))
                    {
                        inscricao.CargoCodigo = codigoCargo.ToString();
                        inscricao.CargoDreCodigo = codigoDre;
                        inscricao.CargoUeCodigo = codigoUe;
                        inscricao.CargoId = cargoId;
                        break;
                    }
                }

                if (cargosProposta.PossuiElementos())
                {
                    var cargoFuncaoOutros = await mediator.Send(new ObterCargoFuncaoOutrosQuery());
                    var cargoEhOutros = cargosProposta.Any(t => t.CargoFuncaoId == cargoFuncaoOutros.Id);
                    if (!cargoEhOutros && (!inscricao.CargoId.HasValue || !cargosProposta.Any(a => a.CargoFuncaoId == inscricao.CargoId)))
                        temErroCargo = true;
                }
            }

            var funcaoAtividadeProposta = await mediator.Send(new ObterPropostaFuncoesEspecificasPorIdQuery(propostaId));
            if (funcaoAtividadeProposta.PossuiElementos())
            {
                foreach (var funcaoEol in cargoFuncaoUsuarioEol.Where(t => t.CdFuncaoAtividade.HasValue && t.TipoVinculoFuncaoAtividade == tipoVinculo))
                {
                    var codigoCargo = funcaoEol.CdCargoSobreposto ?? funcaoEol.CdCargoBase ?? 0;
                    var codigoDre = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdDreCargoSobreposto : funcaoEol.CdDreCargoBase;
                    var codigoUe = funcaoEol.CdCargoSobreposto.HasValue ? funcaoEol.CdUeCargoSobreposto : funcaoEol.CdUeCargoBase;

                    var cargoFuncao = await mediator.Send(new ObterCargoFuncaoPorCodigoEolQuery(
                        [codigoCargo], [funcaoEol.CdFuncaoAtividade ?? 0]));

                    var cargoId = cargoFuncao.FirstOrDefault(t => t.Tipo == CargoFuncaoTipo.Cargo)?.Id;
                    var funcaoId = cargoFuncao.FirstOrDefault(t => t.Tipo == CargoFuncaoTipo.Funcao)?.Id;

                    if (funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == funcaoId))
                    {
                        inscricao.CargoCodigo = codigoCargo.ToString();
                        inscricao.CargoDreCodigo = codigoDre;
                        inscricao.CargoUeCodigo = codigoUe;
                        inscricao.CargoId = cargoId;

                        inscricao.FuncaoCodigo = funcaoEol.CdFuncaoAtividade.ToString();
                        inscricao.FuncaoDreCodigo = funcaoEol.CdDreFuncaoAtividade.ToString();
                        inscricao.FuncaoUeCodigo = funcaoEol.CdUeFuncaoAtividade.ToString();
                        inscricao.FuncaoId = funcaoId;

                        break;
                    }
                }

                if (funcaoAtividadeProposta.PossuiElementos())
                {
                    if (!inscricao.FuncaoId.HasValue || !funcaoAtividadeProposta.Any(a => a.CargoFuncaoId == inscricao.FuncaoId))
                        temErroFuncao = true;
                }
            }

            if (temErroCargo && (temErroFuncao || !funcaoAtividadeProposta.PossuiElementos()))
                return ResultadoMapeamento.Ok(MensagemNegocio.CURSISTA_NAO_POSSUI_CARGO_PUBLI_ALVO_FORMACAO_INSCRICAO_MANUAL);

            return ResultadoMapeamento.Ok();
        }

        private async Task<Usuario> ObterUsuarioPorLogin(InscricaoCursistaImportacaoDto inscricaoCursistaDTO)
        {
            var ehProfissionalRede = inscricaoCursistaDTO.ColaboradorRede.EhColaboradorRede();
            var login = ehProfissionalRede ? inscricaoCursistaDTO.RegistroFuncional : inscricaoCursistaDTO.Cpf;

            if (string.IsNullOrWhiteSpace(login))
                throw new NegocioException(MensagemNegocio.USUARIO_NAO_FOI_ENCONTRADO_COM_O_REGISTRO_FUNCIONAL_OU_CPF_INFORMADOS);

            if (ehProfissionalRede)
            {
                if (login.Length < 7)
                    throw new NegocioException(MensagemNegocio.RF_MENOR_QUE_7_DIGITOS);
            }
            else
            {
                if (!UtilValidacoes.CpfEhValido(login))
                    throw new NegocioException(MensagemNegocio.CPF_INVALIDO);
            }

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()));
            if (usuario.NaoEhNulo())
                return usuario;

            if (ehProfissionalRede)
            {
                var dadosUsuario = await mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login));
                if (dadosUsuario is null || string.IsNullOrWhiteSpace(dadosUsuario.Login))
                    return default!;

                usuario = mapper.Map<Usuario>(dadosUsuario);
                usuario.Cpf = inscricaoCursistaDTO.Cpf.SomenteNumeros();
                usuario.Tipo = TipoUsuario.Interno;

                await mediator.Send(new SalvarUsuarioCommand(usuario));
            }

            return usuario;
        }

        private async Task AlterarSituacaoArquivo(long importacaoArquivoId)
        {
            var possuiRegistroCarregamentoInicial = await mediator.Send(new PossuiRegistroPorArquivoSituacaoQuery(importacaoArquivoId, SituacaoImportacaoArquivoRegistro.CarregamentoInicial));
            var possuiRegistrosNaFila = await mediator.Send(new ObterTotalRegistroFilaQuery(RotasRabbit.RealizarImportacaoInscricaoCursistaValidarItem)) > 0;

            if (!possuiRegistroCarregamentoInicial || !possuiRegistrosNaFila)
                await mediator.Send(new AlterarSituacaoImportacaoArquivoCommand(importacaoArquivoId, SituacaoImportacaoArquivo.Validado));
        }
        public class ResultadoMapeamento
        {
            public bool Sucesso { get; private set; }
            public string? MensagemErro { get; private set; }

            public static ResultadoMapeamento Ok(string? mensagem = "") => new() { Sucesso = true, MensagemErro = mensagem };
            public static ResultadoMapeamento Falha(string mensagem) => new() { Sucesso = false, MensagemErro = mensagem };
        }
    }
}
