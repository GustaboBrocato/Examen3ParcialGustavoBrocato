//Creado por Gustavo Brocato #202010030250

using CommunityToolkit.Maui.Views;
using Examen3ParcialGustavoBrocato.Controllers;
using Examen3ParcialGustavoBrocato.Models;
using Plugin.AudioRecorder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Examen3ParcialGustavoBrocato.Views;

public partial class NuevaNota : ContentPage
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

    public NuevaNota()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        audioRecorderService = new AudioRecorderService();
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

    private async void btnGrabarAudio_Pressed(object sender, EventArgs e)
    {
        if (!isRecording)
        {
            await StartRecordingAsync();
            isRecording = true;
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

    //Metodo para mostrar la ventana de carga
    private void ShowLoadingDialog()
    {
        // Show your loading dialog here
        activityIndicator.IsRunning = true;
        activityIndicator.IsVisible = true;
        framePrincipal.IsVisible = false;
        btnAgregar.IsEnabled = false;
    }

    //Metodo para esconder la ventana de carga
    private void HideLoadingDialog()
    {
        // Hide your loading dialog here
        activityIndicator.IsRunning = false;
        activityIndicator.IsVisible = false;
        framePrincipal.IsVisible = true;
        btnAgregar.IsEnabled = true;
    }

    //Metodo para Agregar a Base de Datos
    private async void btnAgregar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(entryDescripcion.Text))
        {
            await DisplayAlert("Alerta", "Por favor ingrese una descripción para la nota", "OK");
            return;
        }

        if (string.IsNullOrEmpty(base64Audio))
        {
            await DisplayAlert("Alerta", "Por favor agregue una descripción de voz(audio)", "OK");
            return;
        }

        if (string.IsNullOrEmpty(base64Image))
        {
            await DisplayAlert("Alerta", "Por favor agregue una imagen para la nota", "OK");
            return;
        }

        ShowLoadingDialog();

        try
        {
            //Subir al bucket de firebase
            try
            {
                var data = new
                {
                    enlacefoto = base64Image,
                };

                var dataAudio = new
                {
                    audio64 = base64Audio,
                };


                var resultado = await _apiService.PostDataAsync<UrlModel>("uploadStorage.php", data);
                urlImage = resultado.Url;

                var resultadoAudio = await _apiService.PostDataAsync<UrlModel>("uploadStorageAudio.php", dataAudio);
                urlAudio = resultadoAudio.Url;


                if (!string.IsNullOrEmpty(urlImage) && !string.IsNullOrEmpty(urlAudio))
                {
                    var nota = new Models.notaModel
                    {
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
                            await DisplayAlert("Atención", "Nota Creada", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Alerta", "Error al crear la nota", "OK");
                            HideLoadingDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Alerta", "Error obteniendo Enlace de Imagen", "OK");
                return;
            }
        }
        catch
        {
            await DisplayAlert("Alerta", "Error al crear la nota", "OK");
        }
    }

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

            await sourcephoto.CopyToAsync(streamlocal); //Para Guardarla local
        }
    }

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
}