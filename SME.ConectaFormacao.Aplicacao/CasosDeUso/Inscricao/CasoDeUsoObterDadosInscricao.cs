using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterDadosInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterDadosInscricao
    {
        public CasoDeUsoObterDadosInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<DadosInscricaoDTO> Executar()
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia());
            DadosInscricaoDTO retorno;
            if (usuarioLogado.Tipo != TipoUsuario.Externo)
            {
                // TODO: Buscar cargos EOL somente para usuários rede parceira
                var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(usuarioLogado.Login));

                if (cargosFuncoesEol.Any())
                    await AtualizaCpfUsuario(usuarioLogado, cargosFuncoesEol);
                retorno = new DadosInscricaoDTO
                {
                    UsuarioNome = usuarioLogado.Nome,
                    UsuarioCpf = cargosFuncoesEol.Any() ? cargosFuncoesEol.FirstOrDefault().Cpf.AplicarMascara(@"000\.000\.000\-00") : usuarioLogado.Login.AplicarMascara(@"000\.000\.000\-00"),
                    UsuarioEmail = usuarioLogado.EmailEducacional ?? string.Empty,
                    UsuarioRf = usuarioLogado.Login,
                    UsuarioCargos = ObterCargosBaseSobrepostoFuncaoAtividade(cargosFuncoesEol)
                };
            }
            else
            {
                retorno = new DadosInscricaoDTO
                {
                    UsuarioNome = usuarioLogado.Nome,
                    UsuarioCpf = usuarioLogado.Login.AplicarMascara(@"000\.000\.000\-00"),
                    UsuarioEmail = usuarioLogado.EmailEducacional ?? string.Empty,
                    UsuarioRf = usuarioLogado.Login,
                };
            }

            return retorno;
        }

        private IEnumerable<DadosInscricaoCargoEol> ObterCargosBaseSobrepostoFuncaoAtividade(IEnumerable<CursistaCargoServicoEol> cargosFuncoesEol)
        {
            var usuarioCargos = new List<DadosInscricaoCargoEol>();
            foreach (var cargoFuncaoEol in cargosFuncoesEol)
            {
                var item = new DadosInscricaoCargoEol
                {
                    Codigo = cargoFuncaoEol.CdCargoBase.ToString(),
                    Descricao = cargoFuncaoEol.CargoBase,
                    DreCodigo = cargoFuncaoEol.CdDreCargoBase,
                    UeCodigo = cargoFuncaoEol.CdUeCargoBase,
                    TipoVinculo = cargoFuncaoEol.TipoVinculoCargoBase ?? 0,
                    DataInicio = cargoFuncaoEol.DataInicioCargoBase
                };

                if (cargoFuncaoEol.CdFuncaoAtividade.HasValue)
                {
                    item.Funcoes.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdFuncaoAtividade.ToString(),
                        Descricao = cargoFuncaoEol.FuncaoAtividade,
                        DreCodigo = cargoFuncaoEol.CdDreFuncaoAtividade,
                        UeCodigo = cargoFuncaoEol.CdUeFuncaoAtividade,
                        TipoVinculo = cargoFuncaoEol.TipoVinculoFuncaoAtividade ?? 0,
                        DataInicio = cargoFuncaoEol.DataInicioFuncaoAtividade
                    });
                }
                usuarioCargos.Add(item);

                if (cargoFuncaoEol.CdCargoSobreposto.HasValue)
                {
                    usuarioCargos.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdCargoSobreposto.ToString(),
                        Descricao = cargoFuncaoEol.CargoSobreposto,
                        DreCodigo = cargoFuncaoEol.CdDreCargoSobreposto,
                        UeCodigo = cargoFuncaoEol.CdUeCargoSobreposto,
                        TipoVinculo = cargoFuncaoEol.TipoVinculoCargoSobreposto ?? 0,
                        DataInicio = cargoFuncaoEol.DataInicioCargoSobreposto
                    });
                }
            }

            return usuarioCargos;
        }

        private async Task AtualizaCpfUsuario(Dominio.Entidades.Usuario usuarioLogado, IEnumerable<CursistaCargoServicoEol> cargosFuncoesEol)
        {
            var cpfEol = cargosFuncoesEol.FirstOrDefault().Cpf;
            if (usuarioLogado.Cpf.NaoEstaPreenchido() && cpfEol.EstaPreenchido())
            {
                usuarioLogado.Cpf = cpfEol;
                await mediator.Send(new SalvarUsuarioCommand(usuarioLogado));
            }
        }
    }
}
