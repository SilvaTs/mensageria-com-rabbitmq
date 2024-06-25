namespace ItemService.EventProcessor
{
    public interface IProcessaEvento
    {
        void Processo(string mensagem);
    }
}
