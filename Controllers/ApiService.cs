﻿/*
 * Descripción:
 * Este código define una clase llamada ApiService en el espacio de nombres ProyectoFinal_Grupo3_Floristeria_Margaritas.Controllers.
 * La clase proporciona métodos para interactuar con un servicio web API, como obtener y enviar datos utilizando solicitudes HTTP GET y POST.
 * Utiliza HttpClient para realizar las solicitudes HTTP y Newtonsoft.Json para la serialización y deserialización de objetos JSON.
 * Creado por Gustavo Brocato #202010030250
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Examen3ParcialGustavoBrocato.Controllers
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Constructor
        public ApiService()
        {
            _httpClient = new HttpClient();
            //_httpClient = CreateHttpClient();
        }

        // Método para realizar una solicitud HTTP POST y enviar datos al servidor
        public async Task<TResponse> PostDataAsync<TResponse>(string endpoint, object data)
        {
            var apiUrl = "https://phpclusters-164276-0.cloudclusters.net/";

            try
            {
                var jsonData = JsonConvert.SerializeObject(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{apiUrl}{endpoint}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResponse>(responseContent);
                }

                // Handle errors, you can throw an exception or return a default value
                throw new HttpRequestException($"Error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostDataAsync: {ex.Message}");
                return default;
            }
        }

        // Método estático para crear un HttpClient con un handler personalizado que ignora los errores de certificado SSL
        public static HttpClient CreateHttpClient()
        {
            // Create a handler that ignores SSL certificate errors
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            // Create HttpClient with the custom handler
            var httpClient = new HttpClient(handler);

            return httpClient;
        }
    }
}
