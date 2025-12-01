using Shared;
using System.Collections.Concurrent;

namespace multitronikllc.Servicios
{
    public class BackgroudTask(SrvApiService api)
    {
        private BlockingCollection<PacketHeader> reintentos = new BlockingCollection<PacketHeader>();
        public EventHandler<BackgroudTask, Tuple<int, string>>? OnPaketReceived;
        public EventHandler<BackgroudTask, int>? OnPaketError;
        private bool hayMas = false;

        public async Task Start(int id)
        {
            api.userId = id;
            hayMas = true;
            var reintentosLimite = 0;

            var taskReintentos = Task.Run(async () =>
            {
                try
                {
                    while (reintentosLimite < 999)
                    {
                        await Task.Delay(hayMas ? 1000 : 200);
                        if (reintentos.TryTake(out PacketHeader item))
                        {
                            reintentosLimite++;                            
                            await ProesarPaquete(item.Id);
                            await Task.Yield();
                            OnPaketError?.Invoke(this, reintentos.Count());
                        }
                        if (!hayMas && !reintentos.Any()) break;
                    }
                    Console.WriteLine("Finalizando tarea de reintentos");
                    return Task.CompletedTask;
                }
                catch (Exception eex)
                {
                    throw;
                }
                
            });
            
            do
            {   
                var ret = await ProesarPaquete();
                if (!ret) hayMas = false;
                await Task.Yield();
            } while (hayMas);

            await taskReintentos;
        }

        public void Stop()
        {
            hayMas = false;
        }

        private async Task<bool> ProesarPaquete(int? id = null)
        {
            try
            {
                try
                {
                    var p = await api.LeerPackete(id);
                    if (p.Item1.Id != -1) // valido que no hay mas paquetes
                    {
                        var t = new Tuple<int, string>(p.Item1.Id, p.Item2);
                        OnPaketReceived?.Invoke(this, t);
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (CheckSumException ex)
                {
                    reintentos.Add(ex.Packet);                    
                }
                return true;
            }
            catch (Exception eex)
            {

                throw;
            }
            
        }
    }
}
