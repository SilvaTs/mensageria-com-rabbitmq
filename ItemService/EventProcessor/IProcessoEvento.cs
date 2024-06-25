namespace ItemService.EventProcessor
{
    public interface IProcessoEvento
    {
        void Processo(string mensagem);
    }
}
