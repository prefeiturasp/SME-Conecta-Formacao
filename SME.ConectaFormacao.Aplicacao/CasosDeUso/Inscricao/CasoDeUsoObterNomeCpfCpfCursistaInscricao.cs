using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
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

        public async Task<RetornoUsuarioDTO> Executar(string? registroFuncional, string? cpf)
        {
            var login = registroFuncional.EstaPreenchido() ? registroFuncional : cpf;

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()));
            if (usuario is { Nome: not null, Cpf: not null })
                return _mapper.Map<RetornoUsuarioDTO>(usuario);

            if (registroFuncional.EstaPreenchido())
            {
                var cursistaResumidoServicoEol = await mediator.Send(new ObterNomeCpfProfissionalPorRegistroFuncionalQuery(registroFuncional));

                if (cursistaResumidoServicoEol is { Nome: not null, Cpf: not null })
                    return _mapper.Map<RetornoUsuarioDTO>(cursistaResumidoServicoEol);
            }

            throw new NegocioException(MensagemNegocio.CURSISTA_NAO_ENCONTRADO);
        }
    }
}
