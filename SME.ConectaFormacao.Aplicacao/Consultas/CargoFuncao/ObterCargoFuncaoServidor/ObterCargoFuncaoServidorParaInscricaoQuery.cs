using MediatR;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricoes;

namespace SME.ConectaFormacao.Aplicacao.Consultas.CargoFuncao.ObterCargoFuncaoServidor;

public class ObterCargoFuncaoServidorParaInscricaoQuery(string rf) : IRequest<IEnumerable<DadosInscricaoCargoEol>>
{
    public string Rf { get; set; } = rf;
}