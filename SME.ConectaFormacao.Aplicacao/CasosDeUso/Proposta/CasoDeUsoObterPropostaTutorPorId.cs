using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaTutorPorId : CasoDeUsoAbstrato, ICasoDeUsoObterPropostaTutorPorId
    {
        private readonly IMapper _mapper;
        public CasoDeUsoObterPropostaTutorPorId(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PropostaTutorDTO> Executar(long tutorId)
        {
            var tutor = await mediator.Send(new ObterTutorPorIdQuery(tutorId));
            if (tutor == null)
                throw new NegocioException("Registro não encontrado", HttpStatusCode.NoContent);
            var turmas = await mediator.Send(new ObterTutorTurmaPorTutorIdQuery(tutorId));

            var tutorDto = _mapper.Map<PropostaTutorDTO>(tutor);
            if (turmas.Count() > 0)
                tutorDto.Turmas = MapearTurmas(turmas);

            return tutorDto;
        }
        private IEnumerable<PropostaTutorTurmaDTO> MapearTurmas(IEnumerable<PropostaTutorTurma> turmas)
        {
            var turmasDto = new List<PropostaTutorTurmaDTO>();
            foreach (var turma in turmas)
                turmasDto.Add(new PropostaTutorTurmaDTO() { Turma = turma.Turma });
            return turmasDto;
        }
    }
}