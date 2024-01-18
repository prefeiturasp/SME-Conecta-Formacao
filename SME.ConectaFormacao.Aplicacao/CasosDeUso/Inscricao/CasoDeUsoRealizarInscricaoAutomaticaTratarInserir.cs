using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.Inscricao;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Inscricao
{
    public class CasoDeUsoRealizarInscricaoAutomaticaTratarInserir : CasoDeUsoAbstrato, ICasoDeUsoRealizarInscricaoAutomaticaTratarInserir
    {
        private readonly IMapper _mapper;
        
        public CasoDeUsoRealizarInscricaoAutomaticaTratarInserir(IMediator mediator,IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var inscricaoCursistaDto = param.ObterObjetoMensagem<InserirInscricaoDTO>();

            foreach (var propostaTurmaCursista in inscricaoCursistaDto.PropostasTurmasCursistas)
            {
                foreach (var cursista in propostaTurmaCursista.Cursistas)
                {
                    var usuario = await ObterUsuarioPorLogin(cursista);
                    
                    var inscricaoAutomaticaDTO = new InscricaoAutomaticaDTO()
                    {
                        UsuarioId = usuario.Id,
                        PropostaId = inscricaoCursistaDto.PropostaId,
                        PropostaTurmaId = propostaTurmaCursista.Id,
                        
                        CargoCodigo = cursista.CargoCodigo,
                        CargoDreCodigo = cursista.CargoDreCodigo,
                        CargoUeCodigo = cursista.CargoUeCodigo,
                        
                        FuncaoCodigo =  cursista.FuncaoCodigo,
                        FuncaoDreCodigo = cursista.FuncaoDreCodigo,
                        FuncaoUeCodigo = cursista.FuncaoUeCodigo
                    };
                    await mediator.Send(new SalvarInscricaoAutomaticaCommand(inscricaoAutomaticaDTO));
                }
            }
            
            return true;
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuarioPorLogin(FuncionarioRfNomeDreCodigoCargoFuncaoDTO funcionarioRfNomeDreCodigoCargoFuncaoDTO)
        {
            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(funcionarioRfNomeDreCodigoCargoFuncaoDTO.Rf));

            if (usuario.NaoEhNulo()) 
                return usuario;

            usuario = _mapper.Map<Dominio.Entidades.Usuario>(funcionarioRfNomeDreCodigoCargoFuncaoDTO);
            
            await mediator.Send(new SalvarUsuarioCommand(usuario));
            return usuario;
        }
    }
}
