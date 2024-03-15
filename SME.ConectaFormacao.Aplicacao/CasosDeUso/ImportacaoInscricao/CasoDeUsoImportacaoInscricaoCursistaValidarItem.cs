using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.Dtos;
using SME.ConectaFormacao.Aplicacao.Dtos.ImportacaoArquivo;
using SME.ConectaFormacao.Aplicacao.Dtos.Inscricao;
using SME.ConectaFormacao.Aplicacao.Interfaces.ImportacaoArquivo;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Excecoes;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;

namespace SME.ConectaFormacao.Aplicacao.CasosDeUso.ImportacaoInscricao
{
    public class CasoDeUsoImportacaoInscricaoCursistaValidarItem : CasoDeUsoAbstrato, ICasoDeUsoImportacaoInscricaoCursistaValidarItem
    {
        private readonly IMapper _mapper;
        
        public CasoDeUsoImportacaoInscricaoCursistaValidarItem(IMediator mediator,IMapper mapper) : base(mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Executar(MensagemRabbit param)
        {
            var importacaoArquivoRegistro = param.ObterObjetoMensagem<ImportacaoArquivoRegistroDTO>() 
                                            ?? throw new NegocioException(MensagemNegocio.IMPORTACAO_ARQUIVO_REGISTRO_NAO_LOCALIZADA);

            var importacaoInscricaoCursista = importacaoArquivoRegistro.Conteudo.JsonParaObjeto<InscricaoCursistaDTO>();

            var propostaTurma = await mediator.Send(new ObterPropostaTurmaPorNomeQuery(importacaoInscricaoCursista.Turma, importacaoArquivoRegistro.PropostaId)) 
                                ?? throw new NegocioException(MensagemNegocio.TURMA_NAO_ENCONTRADA);
            
            var usuario = await ObterUsuarioPorLogin(importacaoInscricaoCursista) ??
                          throw new NegocioException(MensagemNegocio.USUARIO_NAO_ENCONTRADO);
            
            return true;
        }

        private async Task<Dominio.Entidades.Usuario> ObterUsuarioPorLogin(InscricaoCursistaDTO inscricaoCursistaDTO )
        {
            var ehProfissionalRede = inscricaoCursistaDTO.ColaboradorRede.EhColaboradorRede();
            
            var login = ehProfissionalRede ? inscricaoCursistaDTO.RegistroFuncional : inscricaoCursistaDTO.Cpf;

            var usuario = await mediator.Send(new ObterUsuarioPorLoginQuery(login.SomenteNumeros()));
            if (usuario.NaoEhNulo())
                return usuario;

            if (ehProfissionalRede)
            {
                var dadosUsuario = await mediator.Send(new ObterMeusDadosServicoAcessosPorLoginQuery(login));
                if (dadosUsuario.EhNulo())
                    return default;

                usuario = _mapper.Map<Dominio.Entidades.Usuario>(dadosUsuario);
                usuario.Cpf = inscricaoCursistaDTO.Cpf.SomenteNumeros();
                usuario.Tipo = TipoUsuario.Interno;

                await mediator.Send(new SalvarUsuarioCommand(usuario));
            }

            return usuario;
        }
    }
}
