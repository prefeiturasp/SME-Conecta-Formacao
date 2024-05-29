using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoObterNomeCpfCpfCursistaInscricao : CasoDeUsoAbstrato, ICasoDeUsoObterNomeCpfCursistaInscricao
    {
        private readonly IMapper _mapper;
        public CasoDeUsoObterNomeCpfCpfCursistaInscricao(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<RetornoUsuarioCpfNomeDTO> Executar(string? registroFuncional, string? cpf)
        {
            RetornoUsuarioCpfNomeDTO? retornoUsuarioCpfNomeDTO = null;

            var login = registroFuncional.EstaPreenchido() ? registroFuncional : cpf;

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()));
            if (usuario is { Nome: not null, Cpf: not null })
                retornoUsuarioCpfNomeDTO = _mapper.Map<RetornoUsuarioCpfNomeDTO>(usuario);

            if (registroFuncional.EstaPreenchido())
            {
                var cursistaResumidoServicoEol = await mediator.Send(new ObterNomeCpfProfissionalPorRegistroFuncionalQuery(registroFuncional));

                if (cursistaResumidoServicoEol is { Nome: not null, Cpf: not null })
                {
                    retornoUsuarioCpfNomeDTO = _mapper.Map<RetornoUsuarioCpfNomeDTO>(cursistaResumidoServicoEol);
                    retornoUsuarioCpfNomeDTO.UsuarioCargos = await ObterCargosBaseSobrepostoFuncaoAtividade(registroFuncional);
                }
            }

            if (retornoUsuarioCpfNomeDTO.NaoEhNulo())
                return retornoUsuarioCpfNomeDTO;

            throw new NegocioException(MensagemNegocio.CURSISTA_NAO_ENCONTRADO);
        }

        private async Task<IEnumerable<DadosInscricaoCargoEol>> ObterCargosBaseSobrepostoFuncaoAtividade(string login)
        {
            var cargosFuncoesEol = await mediator.Send(new ObterCargosFuncoesDresFuncionarioServicoEolQuery(login));
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
    }
}
