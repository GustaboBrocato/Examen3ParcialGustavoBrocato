//Creado por Gustavo Brocato #202010030250

using System.Collections.ObjectModel;
using Examen3ParcialGustavoBrocato.Models;
using Examen3ParcialGustavoBrocato.ViewModel;
using Examen3ParcialGustavoBrocato.Controllers;

namespace Examen3ParcialGustavoBrocato.Views;

public partial class VerNotas : ContentPage
{
    //Observable collection
    public ObservableCollection<VerNotasViewModel>? Notas { get; set; }

    //Firebase Controller
    private NotaController controller = new NotaController();


    public VerNotas()
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            List<notaModel> listNotas = await controller.GetListNotas();

            // Ordena la lista por la fecha de forma descendiente, para que arriba salgan las mas nuevas
            listNotas = listNotas.OrderByDescending(nota => nota.Fecha).ToList();

            Notas = new ObservableCollection<VerNotasViewModel>();

            foreach (var nota in listNotas)
            {
                bool visible = false;

                if(!string.IsNullOrEmpty(nota.Photo_Record))
                {
                    visible = true;
                }

                VerNotasViewModel notasView = new VerNotasViewModel
                {
                    Key = nota.Key,
                    Descripcion = nota.Descripcion,
                    Fecha = nota.Fecha,
                    Foto = nota.Photo_Record,
                    Audio = nota.Audio_Record,
                    AudioCommand = new Command(() => HandleAudioCommand(nota)),
                    ImageCommand = new Command(() => HandleImageCommand(nota)),
                    EliminarCommand = new Command(() => HandleEliminarCommand(nota)),
                    TappedCommand = new Command(() => HandleTappedCommand(nota)),
                    Visibilidad = visible,
                };

                Notas.Add(notasView);
            }

            collectionViewNotas.ItemsSource = Notas;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Atención", "Se produjo un error al obtener las notas", "OK");
        }
    }

    private void HandleAudioCommand(notaModel nota)
    {
        mediaElementAudio.Source = nota.Audio_Record;
        mediaElementAudio.Play();
    }

    private async void HandleImageCommand(notaModel nota)
    {
        await Navigation.PushModalAsync(new Views.verFoto(nota));
    }

    private async void HandleEliminarCommand(notaModel nota)
    {
        var tappedItem = Notas.FirstOrDefault(item => item.Key == nota.Key);
        bool userConfirmed = await DisplayAlert("Confirmación", "¿Está seguro de que desea eliminar esta nota?", "Si", "No");

        if (userConfirmed)
        {
            try
            {

                if (controller != null)
                {
                    bool success = await controller.deleteNota(nota.Key);

                    if (success)
                    {
                        Notas.Remove(tappedItem);

                        await DisplayAlert("Atención", "Nota Eliminada", "OK");
                    }
                }

            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo eliminar el producto", "OK");
            }
        }
    }

    private async void HandleTappedCommand(notaModel nota)
    {
        await Navigation.PushModalAsync(new Views.EditNota(nota));
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopAsync();
    }
}