using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Aplicacao.Interfaces.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Excecoes;
using System.Net;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.Proposta
{
    public class CasoDeUsoObterPropostaTutorPorId(IMediator mediator, IMapper mapper) : CasoDeUsoAbstrato(mediator), ICasoDeUsoObterPropostaTutorPorId
    {
        public async Task<PropostaTutorDTO> Executar(long tutorId)
        {
            var tutor = await mediator.Send(new ObterTutorPorIdQuery(tutorId)) ?? 
                        throw new NegocioException("Registro não encontrado", HttpStatusCode.NoContent);
            var turmas = await mediator.Send(new ObterTutorTurmaPorTutorIdQuery(tutorId));

            var tutorDto = mapper.Map<PropostaTutorDTO>(tutor);
            if (turmas.Any())
                tutorDto.Turmas = MapearTurmas(turmas);

            return tutorDto;
        }
        private static List<PropostaTutorTurmaDTO> MapearTurmas(IEnumerable<PropostaTutorTurma> turmas)
        {
            var turmasDto = new List<PropostaTutorTurmaDTO>();
            foreach (var turma in turmas)
                turmasDto.Add(new PropostaTutorTurmaDTO() { TurmaId = turma.TurmaId });
            return turmasDto;
        }
    }
}