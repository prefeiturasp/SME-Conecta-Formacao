using AutoMapper;
using SME.ConectaFormacao.Dominio;
using SME.ConectaFormacao.Dominio.Repositorios;
using SME.ConectaFormacao.Aplicacao.DTOS;
using SME.ConectaFormacao.Aplicacao.Servicos.Interface;

namespace SME.ConectaFormacao.Aplicacao.Servicos
{
    public class ServicoAplicacao<E,D> : IServicoAplicacao where E : EntidadeBase where D : BaseDTO
    {
        private readonly IRepositorioBase<E> repositorio;
        private readonly IMapper mapper;
        
        public ServicoAplicacao(IRepositorioBase<E> repositorio, IMapper mapper) 
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<long> Inserir(D entidadeDto)
        {
            var entidade = mapper.Map<E>(entidadeDto);
            return repositorio.Inserir(entidade);
        }

        public async Task<IList<D>> ObterTodos()
        {
            return (await repositorio.ObterTodos()).Where(w=> !w.Excluido).Select(s=> mapper.Map<D>(s)).ToList();
        }

        public async Task<D> Alterar(D entidadeDto)
        {
            var entidade = mapper.Map<E>(entidadeDto);
            return mapper.Map<D>(await repositorio.Atualizar(entidade));
        }

        public async Task<D> ObterPorId(long entidadeId)
        {
            return mapper.Map<D>(await repositorio.ObterPorId(entidadeId));
        }

        public async Task<bool> Excluir(long entidaId)
        {
            var entidade = await ObterPorId(entidaId);
            entidade.Excluido = true;
            await Alterar(entidade);
            return true;
        }
    }
}
