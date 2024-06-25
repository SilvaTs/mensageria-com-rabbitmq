using AutoMapper;
using ItemService.Data;
using ItemService.Dtos;
using ItemService.Models;
using System.Text.Json;

namespace ItemService.EventProcessor;

public class ProcessoEvento : IProcessoEvento
{
    private readonly IMapper _mapper;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProcessoEvento(IMapper mapper, IServiceScopeFactory serviceScopeFactory)
    {
        _mapper = mapper;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void Processo(string mensagem)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var itemRepository = scope.ServiceProvider.GetRequiredService<ItemRepository>();

        var restauranteReadDto = JsonSerializer.Deserialize<RestauranteReadDto>(mensagem);

        var restaurante = _mapper.Map<Restaurante>(restauranteReadDto);

        if (!itemRepository.ExisteRestauranteExterno(restaurante.Id))
        {
            itemRepository.CreateRestaurante(restaurante);
            itemRepository.SaveChanges();
        }
    }
}
