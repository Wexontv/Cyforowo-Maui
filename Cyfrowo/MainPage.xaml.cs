namespace Cyfrowo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OdswiezStatystyki();
        }

        private void OdswiezStatystyki()
        {
            int wygrane = GameStats.Wins;
            int przegrane = GameStats.Losses;
            EtykietaStatystyk.Text = $"WYGRANE: {wygrane}   |   PRZEGRANE: {przegrane}";
        }

        private async void KlikniecieGraj(object sender, EventArgs e)
        {
            int selectedIndex = DifficultyPicker.SelectedIndex;

            int dlugoscKodu = 3;
            bool czyPowtorzenia = true;
            int liczbaProb = 6;

            switch (selectedIndex)
            {
                case 0: // Łatwy
                    dlugoscKodu = 3; 
                   czyPowtorzenia = false; 
                    liczbaProb = 6; break;
                case 1: // Średni
                    dlugoscKodu = 4; 
                    czyPowtorzenia = false;
                    liczbaProb = 5; break;
                case 2: // Trudny
                    dlugoscKodu = 5; 
                    czyPowtorzenia = true; 
                    liczbaProb = 4; break;

            }

            await Navigation.PushAsync(new GamePage(dlugoscKodu, czyPowtorzenia, liczbaProb));
        }

        private void KlikniecieWyjscie(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }

        private async void KlikniecieReset(object sender, EventArgs e)
        {
            if (GameStats.Wins == 0 && GameStats.Losses == 0)
            {
                await DisplayAlert("Informacja", "Statystyki są już wyzerowane.", "OK");
                return;
            }

            bool potwierdzenie = await DisplayAlert("Reset", "Czy na pewno chcesz wyzerować wyniki?", "Tak", "Nie");

            if (potwierdzenie)
            {
                GameStats.ResetujStatystyki(); 
                OdswiezStatystyki();           
            }
        }
    }
}