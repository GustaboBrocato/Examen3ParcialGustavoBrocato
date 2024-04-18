//Creado por Gustavo Brocato #202010030250

namespace Examen3ParcialGustavoBrocato
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void btnAgregarNota_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.NuevaNota());
        }

        private async void btnVerNotas_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Views.VerNotas());
        }
    }

}
