# Multitronik
Multitronik Challenge
## Plataforma
Se usó .NET 10 unicamente.
## Como ejecutar
El proyecto tiene dos aplicativos, el servidor y el cliente.
### Servidor
Esta en la carpeta: <b>multitronikllcAPIMock</b>

Para correr el servidor usar (en la carpeta del servidor):
```
dotnet run
```
Se podra consumir en [http://localhost:5136](http://localhost:5136)

Va a exponer los enpoint:
- GET /Challenge/restart

Reinicia el proceso eliminando la informacion enviada.
- GET /Challenge/get-next-packet

Envia un pakete, en JSon BASE64, de sus datos binarios, para interpretar los datos, se debera convertir la string JSon de base 64 a binario y despues dividir como se plateo en el problema. Al tomar un packewte mediante este endpoint no se enviara nuevamente hasta realizar un "Restart", si no hay mas paketes que enviar, se generara un pakete NULL, que tendra el valor de ID en -1.
- GET /Challenge/retry-packet?packetId=<i>un_numero</i>

Este endpoint se usa para volver a pedir un packete especificamente, puede ser que no se recicieron correctamente todos los paketes y se soluciona con este enpoint.
- GET /Challenge/ack-packet?packetId=<i>un_numero</i>

Marca el packete con el id como recibido ok.
### Cliente
Aplicacion Web con Blazor SSR en la carpeta: <b>multitronikllc</b>

Ejecutar con:
```
dotnet run
```
Se podra abrir en el navegador en [http://localhost:5131](http://localhost:5131)
### Docker Compose
Se puede usar Docker Compose para ejecutar la aplicacion con el comando:
```
podman compose up --build [-d]
```
-d para dejarlo en ejecucion como demonio

Navegar en: [http://localhost:8091](http://localhost:8091)
Para detener se presiona CTRL+C y:
```
podman compose down
```
### Archivo de configuración

![corriendo](/docs/running.png)