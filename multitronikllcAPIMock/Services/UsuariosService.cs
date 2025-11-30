using Shared;
using System.Collections.Generic;
using System.Reflection;

namespace multitronikllcAPIMock.Services
{
    public class UsuariosService
    {
        private readonly Dictionary<int,List<PacketModel>> _usuarios = [];
        private readonly Random _random = new();

        public void EliminarUsuario(int id) {
            _usuarios.Remove(id);
        }

        private void AgregarUsuario(int id) {
            if(!_usuarios.ContainsKey(id)) {
                _usuarios[id] = GeneratePackages();
            }
        }

        public PacketModel GetPackage(int id, bool last = false)
        {
            if(!_usuarios.ContainsKey(id)) {
                AgregarUsuario(id);
            }
            var paquetes = _usuarios[id];
            if(paquetes.Count == 0) {
                return NullPacage();
            }
            if (last)
            {
                var p = paquetes.Last();
                p.Status = EstadoEnum.Enviado;
                return p;
            }
            var noEnviados = paquetes.Where(p => p.Status == EstadoEnum.Pendiente).ToList();
            if (!noEnviados.Any())
            {
                return NullPacage();
            }
            var index = _random.Next(noEnviados.Count);            
            var paquete = noEnviados[index];
            paquete.Status = EstadoEnum.Enviado;            
            return paquete;
        }

        private PacketModel NullPacage()
        {            
            var head = ProcesarPacket.GeneratePackages(-1);
            return new PacketModel(head.Item1, head.Item2);
        }

        private List<PacketModel> GeneratePackages()
        {
            // Busco en el texto las palabras y voy generando los paquetes
            var paquetes = new List<PacketModel>();
            var texto = ObtenerTexto();
            var tramos = SegmentarTextoAleatorio(texto);
            int id = 0;
            foreach(var tramo in tramos) {
                var paket = ProcesarPacket.GeneratePackages(id, tramo);
                var pModel = new PacketModel(paket.Item1,paket.Item2);
                paquetes.Add(pModel);
                id++;
            }
            return paquetes;
        }

        private string ObtenerTexto()
        {
            string nombreRecurso = "multitronikllcAPIMock.data.JUICIO_INJUSTO.txt";
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(nombreRecurso) 
                ?? throw new Exception($"No se encontró el recurso incrustado: {nombreRecurso}");
            using StreamReader reader = new(stream);
            string contenido = reader.ReadToEnd();
            return contenido;
        }

        private List<string> SegmentarTextoAleatorio(string text)
        {
            List<string> result = [];
            int remainingLength = text.Length;
            int currentIndex = 0;

            while (remainingLength > 0)
            {
                // Determina la longitud máxima del segmento (hasta 9 caracteres o lo que quede)
                int maxSegmentLength = Math.Min(9, remainingLength);

                // Genera una longitud aleatoria para el segmento
                int segmentLength = _random.Next(1, maxSegmentLength + 1);                                
                string segment = text.Substring(currentIndex, segmentLength);
                result.Add(segment);                                
                currentIndex += segmentLength;
                remainingLength -= segmentLength;
            }
            return result;
        }

        public PacketModel RetryPackage(int userId, int packetId)
        {
            if (!_usuarios.ContainsKey(userId))
            {
                throw new ProcessException("Usuario no encontrado");
            }
            var paquetes = _usuarios[userId];            
            var paquete = paquetes.FirstOrDefault(p=> p.Id == packetId);
            if (paquete == null)
            {
                return NullPacage();
            }            
            paquete.Status = EstadoEnum.Enviado;
            return paquete;
        }

        public void AckPackage(int userId, int packetId)
        {
            if (!_usuarios.ContainsKey(userId))
            {
                throw new ProcessException("Usuario no encontrado");
            }
            var paquetes = _usuarios[userId];
            var paquete = paquetes.FirstOrDefault(p => p.Id == packetId);
            if (paquete == null)
            {
                throw new ProcessException("Paquete no encontrado");
            }
            paquete.Status = EstadoEnum.Confirmado;
        }
    }
}
