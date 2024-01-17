using MediatR;
using SME.ConectaFormacao.Infra.Servicos.Eol.Dto;

namespace SME.ConectaFormacao.Aplicacao
{
    public class ObterFuncionarioPorFiltroPropostaServicoEolQuery : IRequest<IEnumerable<FuncionarioRfNomeDreCodigoDTO>>
    {
        public ObterFuncionarioPorFiltroPropostaServicoEolQuery(IEnumerable<long> codigosCargos, 
            IEnumerable<long> codigosFuncoes, IEnumerable<long> codigoModalidade, IEnumerable<string> anosTurma, 
            IEnumerable<string> codigosDres, IEnumerable<long> codigosComponentesCurriculares, bool ehTipoJornadaJEIF)
        {
            CodigosCargos = codigosCargos;
            CodigosFuncoes = codigosFuncoes;
            CodigoModalidade = codigoModalidade;
            AnosTurma = anosTurma;
            CodigosDres = codigosDres;
            CodigosComponentesCurriculares = codigosComponentesCurriculares;
            EhTipoJornadaJEIF = ehTipoJornadaJEIF;
        }

        public IEnumerable<long> CodigosCargos { get; }
        public IEnumerable<long> CodigosFuncoes { get; }
        public IEnumerable<long> CodigoModalidade { get; }
        public IEnumerable<string> AnosTurma { get; }
        public IEnumerable<string> CodigosDres { get; }
        public IEnumerable<long> CodigosComponentesCurriculares { get; }
        public bool EhTipoJornadaJEIF { get; }
    }
}
