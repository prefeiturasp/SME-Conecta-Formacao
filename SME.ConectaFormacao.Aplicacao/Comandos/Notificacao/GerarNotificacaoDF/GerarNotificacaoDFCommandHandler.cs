using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Constantes;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoDFCommandHandler : IRequestHandler<GerarNotificacaoDFCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoDFCommandHandler(ITransacao transacao,IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,IMediator mediator,IMapper mapper)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(GerarNotificacaoDFCommand request, CancellationToken cancellationToken)
        {
            var linkSistema = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacao, DateTimeExtension.HorarioBrasilia().Year));
            
            var notificacao = await ObterNotificacao(request.Proposta, request.Pareceristas.FirstOrDefault(), linkSistema.Valor);
            
            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);
                
                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);
                
                transacao.Commit();

                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarNotificacao, notificacao));
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
            finally
            {
                transacao.Dispose();
            }

            return true;
        }
        
        private async Task<Notificacao> ObterNotificacao(Proposta proposta, PropostaPareceristaResumidoDTO parecerista, string linkSistema)
        {
            var usuariosDFs = await _mediator.Send(new ObterUsuariosPorPerfilQuery(Perfis.ADMIN_DF));
            
            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.SignalR,
                Parametros = JsonConvert.SerializeObject(proposta),
                Usuarios =  _mapper.Map<IEnumerable<NotificacaoUsuario>>(usuariosDFs),
                    
                Titulo = string.Format("Proposta {0} - {1} foi analisada pelo Parecerista", 
                    proposta.Id, 
                    proposta.NomeFormacao),
                
                Mensagem = string.Format("O Parecerista {0} ({1}) Inseriu comentários na proposta {2} - {3}. Acesse <a href=\"{4}\">Aqui</a> o cadastro da proposta.",
                    parecerista.Login, 
                    parecerista.Nome, 
                    proposta.Id, 
                    proposta.NomeFormacao, 
                    linkSistema),
                
            };
        }
    }
}
