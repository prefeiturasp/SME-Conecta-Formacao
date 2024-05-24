using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SME.ConectaFormacao.Aplicacao.Dtos.Proposta;
using SME.ConectaFormacao.Dominio.Entidades;
using SME.ConectaFormacao.Dominio.Enumerados;
using SME.ConectaFormacao.Dominio.Extensoes;
using SME.ConectaFormacao.Infra;
using SME.ConectaFormacao.Infra.Dados;
using SME.ConectaFormacao.Infra.Dados.Repositorios.Interfaces;

namespace SME.ConectaFormacao.Aplicacao
{
    public class GerarNotificacaoPareceristaCommandHandler : IRequestHandler<GerarNotificacaoPareceristaCommand, bool>
    {
        private readonly IRepositorioNotificacao _repositorioNotificacao;
        private readonly IRepositorioNotificacaoUsuario _repositorioNotificacaoUsuario;
        private readonly ITransacao _transacao;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GerarNotificacaoPareceristaCommandHandler(ITransacao transacao,IRepositorioNotificacao repositorioNotificacao,
            IRepositorioNotificacaoUsuario repositorioNotificacaoUsuario,IMediator mediator,IMapper mapper)
        {
            _repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
            _repositorioNotificacaoUsuario = repositorioNotificacaoUsuario ?? throw new ArgumentNullException(nameof(repositorioNotificacaoUsuario));
            _transacao = transacao ?? throw new ArgumentNullException(nameof(transacao));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<bool> Handle(GerarNotificacaoPareceristaCommand request, CancellationToken cancellationToken)
        {
            var linkSistema = await _mediator.Send(new ObterParametroSistemaPorTipoEAnoQuery(TipoParametroSistema.UrlConectaFormacao, DateTimeExtension.HorarioBrasilia().Year));
            
            var notificacao = ObterNotificacao(request.Proposta, request.Pareceristas, linkSistema.Valor);
            
            var transacao = _transacao.Iniciar();
            try
            {
                var notificacaoId = await _repositorioNotificacao.Inserir(notificacao);
                
                await _repositorioNotificacaoUsuario.InserirUsuarios(transacao, notificacao.Usuarios, notificacaoId);
                
                transacao.Commit();

                await _mediator.Send(new PublicarNaFilaRabbitCommand(RotasRabbit.EnviarEmail, notificacao));
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
        
        private Notificacao ObterNotificacao(Proposta proposta, IEnumerable<PropostaPareceristaResumidoDTO> pareceristas, string linkSistema)
        {
            return new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Proposta,
                TipoEnvio = NotificacaoTipoEnvio.Email,
                Parametros = JsonConvert.SerializeObject(proposta),
                Usuarios =  _mapper.Map<IEnumerable<NotificacaoUsuario>>(pareceristas),
                    
                Titulo = string.Format("Proposta {0} - {1} foi atribuída a você", 
                proposta.Id, 
                proposta.NomeFormacao),
                
                Mensagem = string.Format("A proposta {0} - {1} foi atribuída a você. Acesse <a href=\"{2}\">Aqui</a> o cadastro da proposta e registre seu parecer.",
                    proposta.Id, 
                    proposta.NomeFormacao, 
                    linkSistema)
            };
        }
    }
}
