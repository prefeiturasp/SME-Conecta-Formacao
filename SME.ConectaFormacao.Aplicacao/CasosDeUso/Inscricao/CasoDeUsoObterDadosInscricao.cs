using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterDadosInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterDadosInscricao
    {
        public CasoDeUsoObterDadosInscricao(IMediator mediator) : base(mediator)
        {
        }

        public async Task<DadosInscricaoDTO> Executar()
        {
            var usuarioLogado = await mediator.Send(ObterUsuarioLogadoQuery.Instancia);

            // TODO: Buscar cargos EOL somente para usuários rede parceira
            var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(usuarioLogado.Login));

            await AtualizaCpfUsuario(usuarioLogado, cargosFuncoesEol);

            var retorno = new DadosInscricaoDTO()
            {
                UsuarioNome = usuarioLogado.Nome,
                UsuarioCpf = cargosFuncoesEol.FirstOrDefault().Cpf.AplicarMascara(@"\(00\) 00000\-0000"),
                UsuarioEmail = usuarioLogado.Email,
                UsuarioRf = usuarioLogado.Login,
                UsuarioCargos = ObterCargosBaseSobrepostoFuncaoAtividade(cargosFuncoesEol)
            };

            return retorno;
        }

        private List<DadosInscricaoCargoEol> ObterCargosBaseSobrepostoFuncaoAtividade(IEnumerable<CargoFuncionarioConectaDTO> cargosFuncoesEol)
        {
            var usuarioCargos = new List<DadosInscricaoCargoEol>();
            foreach (var cargoFuncaoEol in cargosFuncoesEol)
            {
                var item = new DadosInscricaoCargoEol
                {
                    Codigo = cargoFuncaoEol.CdCargoBase.GetValueOrDefault(),
                    Descricao = cargoFuncaoEol.CargoBase,
                    DreCodigo = cargoFuncaoEol.CdDreCargoBase.GetValueOrDefault(),
                    UeCodigo = cargoFuncaoEol.CdUeCargoBase.GetValueOrDefault()
                };

                if (cargoFuncaoEol.CdFuncaoAtividade.HasValue)
                {
                    item.Funcoes.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdFuncaoAtividade.GetValueOrDefault(),
                        Descricao = cargoFuncaoEol.FuncaoAtividade,
                        DreCodigo = cargoFuncaoEol.CdDreFuncaoAtividade.GetValueOrDefault(),
                        UeCodigo = cargoFuncaoEol.CdUeFuncaoAtividade.GetValueOrDefault()
                    });
                }
                usuarioCargos.Add(item);

                if (cargoFuncaoEol.CdCargoSobreposto.HasValue)
                {
                    usuarioCargos.Add(new DadosInscricaoCargoEol
                    {
                        Codigo = cargoFuncaoEol.CdCargoSobreposto.GetValueOrDefault(),
                        Descricao = cargoFuncaoEol.CargoSobreposto,
                        DreCodigo = cargoFuncaoEol.CdDreCargoSobreposto.GetValueOrDefault(),
                        UeCodigo = cargoFuncaoEol.CdUeCargoSobreposto.GetValueOrDefault()
                    });
                }
            }

            return usuarioCargos;
        }

        private async Task AtualizaCpfUsuario(Dominio.Entidades.Usuario usuarioLogado, IEnumerable<CargoFuncionarioConectaDTO> cargosFuncoesEol)
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
