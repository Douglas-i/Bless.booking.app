﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Bless.Proxy
{
    public class BookingHub 
    {
        private HubConnection _hubConnection;
        private bool _handlerRegistrado = false;

        public event Func<string, Task> OnNotificacionRecibida;

        public async Task ConectarAsync()
        {
            if (_hubConnection == null)
            {
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:7289/hub/notificaciones")
                    .WithAutomaticReconnect()
                    .ConfigureLogging(logging =>
                    {
                        logging.SetMinimumLevel(LogLevel.Debug);
                    })
                    .Build();
            }

            if (!_handlerRegistrado)
            {
                _hubConnection.On<string>("RecibirNotificacion", async (mensaje) =>
                {
                    Console.WriteLine($"Mensaje recibido: {mensaje}");

                    if (OnNotificacionRecibida != null)
                        await OnNotificacionRecibida.Invoke(mensaje);
                });

                _handlerRegistrado = true;
            }

            if (_hubConnection.State != HubConnectionState.Connected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    Console.WriteLine("Conectado al Hub SignalR");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al conectar al Hub: {ex.Message}");
                }
            }
        }



    }
}
