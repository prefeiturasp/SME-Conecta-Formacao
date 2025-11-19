using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra.Servicos.Rabbit.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomaticaInscreverCursista
    {
        private readonly IMapper _mapper;

        public CasoDeUsoRealizarInscricaoAutomaticaInscreverCursista(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricao = param.ObterObjetoMensagem<InscricaoAutomaticaDTO>();

            var usuario = await ObterUsuarioPorLogin(inscricao);
            inscricao.UsuarioId = usuario.Id;

            await mediator.Send(new SalvarInscricaoAutomaticaCommand(inscricao));

            return true;
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuarioPorLogin(InscricaoAutomaticaDTO inscricaoAutomaticaDto)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(inscricaoAutomaticaDto.UsuarioRf));

            if (usuario.NaoEhNulo())
                return usuario;

            usuario = _mapper.Map<Dominio.Entidades.Usuario>(inscricaoAutomaticaDto);

            await mediator.Send(new SalvarUsuarioCommand(usuario));
            return usuario;
        }
    }
}
