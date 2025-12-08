using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ConectaFormacao.Aplicacao.Consultas.CargoFuncao.ObterCargoFuncaoServidor
{
    public class ObterCargoFuncaoServidorParaInscricaoQueryHandler(IRepositorioCargoFuncao repositorioCargoFuncao)
        : IRequestHandler<ObterCargoFuncaoServidorParaInscricaoQuery, IEnumerable<DadosInscricaoCargoEol>>
    {
        public Task<IEnumerable<DadosInscricaoCargoEol>> Handle(ObterCargoFuncaoServidorParaInscricaoQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
