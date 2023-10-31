using AutoMapper;
using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterTutorPaginadoQueryHandler :IRequestHandler<ObterTutorPaginadoQuery,PaginacaoResultadoDTO<PropostaTutorDTO>>
    {
        private readonly IMapper _mapper;

        public ObterTutorPaginadoQueryHandler(IMapper mapper, IRepositorioProposta repositorioProposta)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repositorioProposta = repositorioProposta ?? throw new ArgumentNullException(nameof(repositorioProposta));
        }

        private readonly IRepositorioProposta _repositorioProposta;
        public async Task<PaginacaoResultadoDTO<PropostaTutorDTO>> Handle(ObterTutorPaginadoQuery request, CancellationToken cancellationToken)
        {
            var totalRegistros = await _repositorioProposta.ObterTotalTutores(request.PropostaId);
            IEnumerable<PropostaTutor> tutores = new List<PropostaTutor>();
            if (totalRegistros > 0)
            {
                tutores = await _repositorioProposta.ObterTutoresPaginado(request.NumeroPagina, request.NumeroRegistros, request.PropostaId);
                var ids = tutores.Select(t => t.Id).ToArray();
                var turmas = await _repositorioProposta.ObterTutorTurmasPorTutorId(ids);
                foreach (var tutor in tutores)
                    tutor.Turmas = turmas.Where(x => x.PropostaTutorId == tutor.Id);
            }
            var items = MapearNomesTurmas(tutores);
            return new PaginacaoResultadoDTO<PropostaTutorDTO>(items, totalRegistros, request.NumeroRegistros);
        }
        private IEnumerable<PropostaTutorDTO> MapearNomesTurmas(IEnumerable<PropostaTutor> listaTutores )
        {
            var items =  _mapper.Map<IEnumerable<PropostaTutorDTO>>(listaTutores);
            var retorno =  new List<PropostaTutorDTO>();
            foreach (var item in items)
            {
                item.NomesTurmas = string.Join(", ",item.Turmas.Select(x => "Turma " + x.Turma));
                retorno.Add(item);
            }

            return retorno;
        }
    }
}