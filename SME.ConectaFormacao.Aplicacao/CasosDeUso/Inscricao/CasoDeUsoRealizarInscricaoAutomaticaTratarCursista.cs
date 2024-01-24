using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratarCursista : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomaticaTratarCursista
    {
        private readonly IMapper _mapper;

        public CasoDeUsoRealizarInscricaoAutomaticaTratarCursista(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricaoCursistaDto = param.ObterObjetoMensagem<InserirInscricaoDTO>();

            foreach (var propostaTurmaCursista in inscricaoCursistaDto.InscricaoAutomaticaPropostaTurmaCursistasDTO)
            {
                foreach (var cursista in propostaTurmaCursista.Cursistas)
                {
                    var inscricaoAutomaticaDTO = new InscricaoAutomaticaDTO()
                    {
                        UsuarioRf = cursista.Rf,
                        UsuarioNome = cursista.Nome,
                        PropostaId = inscricaoCursistaDto.PropostaId,
                        PropostaTurmaId = propostaTurmaCursista.Id,
                        CargoCodigo = cursista.CargoCodigo,
                        FuncaoCodigo = cursista.FuncaoCodigo,
                    };
                    await mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.RealizarInscricaoAutomaticaIncreverCursista, inscricaoAutomaticaDTO));
                }
            }

            return true;
        }
    }
}
