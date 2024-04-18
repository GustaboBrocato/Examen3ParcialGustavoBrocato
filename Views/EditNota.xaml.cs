//Creado por Gustavo Brocato #202010030250

using Examen3ParcialGustavoBrocato.Models;
using Examen3ParcialGustavoBrocato.Controllers;
using Plugin.AudioRecorder;
using CommunityToolkit.Maui.Views;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace Examen3ParcialGustavoBrocato.Views;

public partial class EditNota : ContentPage
{
    //Firebase Controller
    private NotaController controller = new NotaController();
    private ApiService _apiService = new ApiService();

    //Variables
    private string? imagenFilePath = null;
    private FileResult? photo = null;
    private FileResult? audio = null;
    private AudioRecorderService audioRecorderService;
    private string? audioFilePath = null;
    private bool isRecording;
    private byte[] audioBytes;
    private byte[] imgBytes;
    private string? base64Audio = null;
    private string? base64Image = null;
    private string? urlImage = null;
    private string? urlAudio = null;
    private bool audioUpdated = false;
    private bool imageUpdated = false;

    private notaModel _model;

    public EditNota(notaModel nota)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        audioRecorderService = new AudioRecorderService();
        _model = nota;
        entryDescripcion.Text = nota.Descripcion;
        imgFoto.Source = nota.Photo_Record;
        mediaElementAudio.Source = nota.Audio_Record;
    }

    private async Task StartRecordingAsync()
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                return;
            }

            // Empieza a grabar
            var audioRecordTask = await audioRecorderService.StartRecording();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting recording: {ex.Message}");
        }
    }

    private async Task StopRecordingAsync()
    {
        try
        {
            // Stop recording
            await audioRecorderService.StopRecording();


            audioFilePath = audioRecorderService.GetAudioFilePath();

            // Convierte a Base64
            base64Audio = await ConvertAudioToBase64Async(audioFilePath);

            if (base64Audio != null)
            {
                // Muestra el audio
                mediaElementAudio.Source = MediaSource.FromFile(audioFilePath);

            }
            else
            {
                Console.WriteLine("Error converting audio to base64.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping recording: {ex.Message}");
        }
    }

    //Metodo para mostrar la ventana de carga
    private void ShowLoadingDialog()
    {
        // Show your loading dialog here
        activityIndicator.IsRunning = true;
        activityIndicator.IsVisible = true;
        framePrincipal.IsVisible = false;
        btnAgregar.IsEnabled = false;
        btnEliminar.IsEnabled = false;
    }

    //Metodo para esconder la ventana de carga
    private void HideLoadingDialog()
    {
        // Hide your loading dialog here
        activityIndicator.IsRunning = false;
        activityIndicator.IsVisible = false;
        framePrincipal.IsVisible = true;
        btnAgregar.IsEnabled = true;
        btnEliminar.IsEnabled=true;
    }

    //Metodo para convertir imagen en base64
    public string? GetImg64()
    {
        if (photo != null)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Stream stream = photo.OpenReadAsync().Result;
                stream.CopyTo(ms);
                byte[] data = ms.ToArray();

                string Base64 = Convert.ToBase64String(data);

                return Base64;
            }
        }
        return null;
    }

    //Metodo para convertir audio en base64
    private async Task<string> ConvertAudioToBase64Async(string filePath)
    {
        try
        {
            byte[] audioBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(audioBytes);
            return base64String;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error converting audio to base64: {ex.Message}");
            return null;
        }
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private void btnCancelarAudio_Clicked(object sender, EventArgs e)
    {
        audioUpdated = false;
        mediaElementAudio.Source = _model.Audio_Record;
    }

    private async void btnGrabarAudio_Pressed(object sender, EventArgs e)
    {
        if (!isRecording)
        {
            await StartRecordingAsync();
            isRecording = true;
            audioUpdated = true;
        }
    }

    private async void btnGrabarAudio_Released(object sender, EventArgs e)
    {
        if (isRecording)
        {
            await StopRecordingAsync();
            isRecording = false;
        }
    }

    private void btnCancelarFoto_Clicked(object sender, EventArgs e)
    {
        imageUpdated = false;
        imgFoto.Source = _model.Photo_Record;
    }

    //Metodo para seleccionar una foto
    private async void btnSeleccionar_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Abre la galería de medios para que el usuario seleccione una foto.
            photo = await MediaPicker.PickPhotoAsync();

            // Verifica si se ha seleccionado una foto desde la galería.
            if (photo != null)
            {
                // Abre un flujo de memoria para la foto seleccionada.
                var memoriaStream = await photo.OpenReadAsync();

                Console.WriteLine($"Error picking photo: {photo}"); // Mensaje en caso de que ocurra un error al seleccionar la foto.

                base64Image = GetImg64();

                // Establece la imagen seleccionada como origen de la imagen en la interfaz de usuario.
                imgFoto.Source = ImageSource.FromFile(photo.FullPath);
                imagenFilePath = photo.FullPath;
                imageUpdated = true;
            }
            else
            {
                Console.WriteLine("Photo selection cancelled or failed."); // Mensaje en caso de que se cancele o falle la selección de la foto.
            }
        }
        catch (FeatureNotSupportedException)
        {
            Console.WriteLine("Photo picking is not supported on this device."); // Mensaje en caso de que la selección de fotos no esté soportada en el dispositivo.
        }
        catch (PermissionException)
        {
            Console.WriteLine("Gallery permission denied."); // Mensaje en caso de que se deniegue el permiso para acceder a la galería.
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error picking photo: {ex.Message}"); // Mensaje en caso de que ocurra un error al seleccionar la foto.
        }
    }

    //Metodo para tomar una foto
    private async void btnTomar_Clicked(object sender, EventArgs e)
    {
        photo = await MediaPicker.CapturePhotoAsync();

        if (photo != null)
        {
            string photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using Stream sourcephoto = await photo.OpenReadAsync();
            using FileStream streamlocal = File.OpenWrite(photoPath);


            using (MemoryStream ms = new MemoryStream())
            {
                await sourcephoto.CopyToAsync(ms);
                imgBytes = ms.ToArray();
            }

            base64Image = GetImg64();

            imgFoto.Source = ImageSource.FromStream(() => photo.OpenReadAsync().Result); //Para verla dentro de archivo

            imageUpdated = true;

            await sourcephoto.CopyToAsync(streamlocal); //Para Guardarla local
        }
    }

    //Metodo para Agregar a Base de Datos
    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        bool userConfirmed = await DisplayAlert("Confirmación", "¿Está seguro de que desea actualizar esta nota?", "Si", "No");

        if (!userConfirmed)
        {
            return;
        }

        if (string.IsNullOrEmpty(entryDescripcion.Text))
        {
            await DisplayAlert("Alerta", "Por favor ingrese una descripción para la nota", "OK");
            return;
        }


        ShowLoadingDialog();

        if(audioUpdated)
        {
            try
            {
                var dataAudio = new
                {
                    audio64 = base64Audio,
                };

                var resultadoAudio = await _apiService.PostDataAsync<UrlModel>("uploadStorageAudio.php", dataAudio);
                urlAudio = resultadoAudio.Url;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", "Error obteniendo Enlace de Audio", "OK");
                return;
            }
        }
        else
        {
            urlAudio = _model.Audio_Record;
            audioUpdated = false;
        }

        if(imageUpdated)
        {
            try
            {
                var data = new
                {
                    enlacefoto = base64Image,
                };

                var resultado = await _apiService.PostDataAsync<UrlModel>("uploadStorage.php", data);
                urlImage = resultado.Url;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", "Error obteniendo Enlace de Imagen", "OK");
                return;
            }
        }
        else
        {
            urlImage = _model.Photo_Record;
            imageUpdated = false;
        }

        try
        {           
                var nota = new Models.notaModel
                {
                    Key = _model.Key,
                    Descripcion = entryDescripcion.Text,
                    Fecha = DateTime.Now,
                    Photo_Record = urlImage,
                    Audio_Record = urlAudio,
                };

                if (controller != null)
                {
                    bool addedSuccessfully = await controller.CrearNota(nota);

                    if (addedSuccessfully)
                    {
                        HideLoadingDialog();
                        await DisplayAlert("Atención", "Nota Actualizada", "OK");
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        await DisplayAlert("Alerta", "Error al actualizar la nota", "OK");
                        HideLoadingDialog();
                    }
                }
            
        }
        catch
        {
            await DisplayAlert("Alerta", "Error al crear la nota", "OK");
        }
    }

    private async void btnEliminar_Clicked(object sender, EventArgs e)
    {
        bool userConfirmed = await DisplayAlert("Confirmación", "¿Está seguro de que desea eliminar esta nota?", "Si", "No");

        if (userConfirmed)
        {
            try
            {

                if (controller != null)
                {
                    bool success = await controller.deleteNota(_model.Key);

                    if (success)
                    {
                        await DisplayAlert("Atención", "Nota Eliminada", "OK");
                        await Navigation.PopModalAsync();
                        await Navigation.PushAsync(new Views.VerNotas());
                    }
                }

            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo eliminar el producto", "OK");
            }
        }
    }
}