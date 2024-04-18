//Creado por Gustavo Brocato #202010030250

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Examen3ParcialGustavoBrocato.Models;

namespace Examen3ParcialGustavoBrocato.Controllers
{
    public class NotaController
    {
        //Cliente de Firebase
        private FirebaseClient client = new FirebaseClient(Config.Config.client);

        //Constructor vacio
        public NotaController() { }

        //Metodo para crear/agregar/update un nuevo producto
        public async Task<bool> CrearNota(notaModel nota)
        {
            try
            {
                // Always create a new note
                if (string.IsNullOrEmpty(nota.Key))
                {
                    await client.Child("Notas").PostAsync(new notaModel
                    {
                        Descripcion = nota.Descripcion,
                        Fecha = nota.Fecha,
                        Photo_Record = nota.Photo_Record,
                        Audio_Record = nota.Audio_Record,
                    });

                    return true;
                }
                else
                {
                    // Update existing note
                    await client.Child("Notas").Child(nota.Key).PutAsync(nota);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        //Metodo para Lectura de elementos
        public async Task<List<notaModel>> GetListNotas()
        {
            var productos = await client.Child("Notas").OnceSingleAsync<Dictionary<string, notaModel>>();

            return productos.Select(x => new notaModel
            {
                Key = x.Key,
                Descripcion = x.Value.Descripcion,
                Fecha = x.Value.Fecha,
                Photo_Record = x.Value.Photo_Record,
                Audio_Record= x.Value.Audio_Record,
            }).ToList();
        }

        //Delete
        public async Task<bool> deleteNota(string key)
        {
            try
            {
                await client.Child("Notas").Child(key).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
