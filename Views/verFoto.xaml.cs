//Creado por Gustavo Brocato #202010030250

using Examen3ParcialGustavoBrocato.Models;

namespace Examen3ParcialGustavoBrocato.Views;

public partial class verFoto : ContentPage
{
	public verFoto(notaModel nota)
	{
		InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        imgFoto.Source = nota.Photo_Record;
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}