using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared;
using System.Collections.Concurrent;
using System.ComponentModel.Design;

namespace multitronikllc.Servicios
{
    public class BackgroudTask(SrvApiService api)
    {
        private BlockingCollection<PacketHeader> reintentos = new BlockingCollection<PacketHeader>();
        public EventHandler<BackgroudTask, Tuple<int, string>>? OnPaketReceived;
        
        public async Task Start(int id)
        {
            api.userId = id;
            var hayMas = true;
            var reintentosLimite = 0;

            var taskReintentos = Task.Run(async () =>
            {
                while (reintentosLimite < 999)
                {
                    await Task.Delay(1000);
                    if (reintentos.TryTake(out PacketHeader item))
                    {
                        reintentosLimite++;
                        hayMas = await ProesarPaquete(item.Id);                        
                    }                    
                    if (!hayMas && !reintentos.Any()) break;                    
                }
            });
            
            do
            {
                try
                {
                    hayMas = await ProesarPaquete();                    
                }
                catch (CheckSumException ex)
                {
                    reintentos.Add(ex.Packet);
                    continue;
                }
            } while (hayMas);

            taskReintentos.Wait();
        }

        private async Task<bool> ProesarPaquete(int? id = null)
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
    }
}
