using Shared;
using System.Collections.Concurrent;

namespace multitronikllc.Servicios
{
    public class BackgroudTask(SrvApiService api, IConfiguration configRoot)
    {
        private BlockingCollection<PacketHeader> reintentos = new BlockingCollection<PacketHeader>();
        public EventHandler<BackgroudTask, Tuple<int, string>>? OnPaketReceived;
        public EventHandler<BackgroudTask, int>? OnPaketError;
        private bool hayMas = false;
        private int maximoTareas = configRoot.GetValue<int?>("maximoTareas") ?? 10;
        

        public async Task Start(int id)
        {
            api.userId = id;
            hayMas = true;
            var reintentosLimite = 0;
            var tareasParaleloBloker = new SemaphoreSlim(maximoTareas);
            var tareasEnParalelo = new List<Task>();

            var taskReintentos = Task.Run(async () =>
            {
                try
                {
                    while (reintentosLimite < 9999)
                    {
                        await Task.Delay(hayMas ? 1000 : 1);
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
                await tareasParaleloBloker.WaitAsync();
                tareasEnParalelo.Add(Task.Run(async () =>
                {                    
                    try
                    {
                        var ret = await ProesarPaquete();
                        if (!ret) hayMas = false;
                        await Task.Yield();
                    }
                    finally
                    {
                        tareasParaleloBloker.Release();
                    }
                }));                           
            } while (hayMas);

            await Task.WhenAll(tareasEnParalelo);
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
                        await api.Ack(p.Item1.Id);
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
                // en ocaciones tora un 404
                return true;
            }
            
        }
    }
}
