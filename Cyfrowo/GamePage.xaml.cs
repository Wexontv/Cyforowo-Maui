using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Devices;

namespace Cyfrowo
{
    public partial class GamePage : ContentPage
    {
        private readonly int _dlugoscKodu;      
        private readonly bool _czyPowtorzenia;  
        private readonly int _maksProb;        

        private int[] _tajnyKod;           
        private int _aktualnaProba = 0;    
        private string _wpisanyTekst = ""; 

        private Label[,] _polaPlanszy;

        public GamePage(int dlugosc, bool powtorzenia, int proby)
        {
            InitializeComponent();

            _dlugoscKodu = dlugosc;
            _czyPowtorzenia = powtorzenia;
            _maksProb = proby;

            NowaGra();
        }

        private void NowaGra()
        {
            _aktualnaProba = 0;
            _wpisanyTekst = "";

            GenerujTajnyKod();
            UtworzPlansze();
        }

        private void UtworzPlansze()
        {
            BoardContainer.Clear();

            Grid boardGrid = new Grid
            {
                ColumnSpacing = 5,
                RowSpacing = 5,
                HorizontalOptions = LayoutOptions.Center
            };

            for (int i = 0; i < _dlugoscKodu; i++)
                boardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 60 });

            for (int i = 0; i < _maksProb; i++)
                boardGrid.RowDefinitions.Add(new RowDefinition { Height = 60 });

            _polaPlanszy = new Label[_maksProb, _dlugoscKodu];

            for (int row = 0; row < _maksProb; row++)
            {
                for (int col = 0; col < _dlugoscKodu; col++)
                {
                    Border border = new Border
                    {
                        Stroke = Colors.Gray,
                        StrokeThickness = 2,
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        BackgroundColor = Colors.White,
                        AnchorX = 0.5,
                        AnchorY = 0.5
                    };

                    Label label = new Label
                    {
                        Text = "",
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    };

                    _polaPlanszy[row, col] = label;

                    border.Content = label;
                    boardGrid.Add(border, col, row);
                }
            }

            BoardContainer.Children.Add(boardGrid);
        }

        private void GenerujTajnyKod()
        {
            Random rnd = new Random();
            _tajnyKod = new int[_dlugoscKodu];

            if (_czyPowtorzenia)
            {
                for (int i = 0; i < _dlugoscKodu; i++)
                {
                    _tajnyKod[i] = rnd.Next(0, 10);
                }
            }
            else
            {
                List<int> dostepneCyfry = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                for (int i = 0; i < _dlugoscKodu; i++)
                {
                    int index = rnd.Next(dostepneCyfry.Count);
                    _tajnyKod[i] = dostepneCyfry[index];
                    dostepneCyfry.RemoveAt(index);
                }
            }
        }

        private void KlikniecieCyfry(object sender, EventArgs e)
        {
            if (sender is Button btn && _wpisanyTekst.Length < _dlugoscKodu)
            {
                _wpisanyTekst += btn.Text;
                AktualizujWidokWiersza();
            }
        }

        private void KlikniecieUsun(object sender, EventArgs e)
        {
            if (_wpisanyTekst.Length > 0)
            {
                _wpisanyTekst = _wpisanyTekst.Substring(0, _wpisanyTekst.Length - 1);
                AktualizujWidokWiersza();
            }
        }

        private void AktualizujWidokWiersza()
        {
            for (int i = 0; i < _dlugoscKodu; i++)
            {
                if (i < _wpisanyTekst.Length)
                    _polaPlanszy[_aktualnaProba, i].Text = _wpisanyTekst[i].ToString();
                else
                    _polaPlanszy[_aktualnaProba, i].Text = "";
            }
        }

        private async void KlikniecieSprawdz(object sender, EventArgs e)
        {
            if (_wpisanyTekst.Length != _dlugoscKodu)
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);

                await BoardContainer.TranslateTo(-10, 0, 50);
                await BoardContainer.TranslateTo(10, 0, 50);
                await BoardContainer.TranslateTo(-5, 0, 50);
                await BoardContainer.TranslateTo(5, 0, 50);
                await BoardContainer.TranslateTo(0, 0, 50);

                await DisplayAlert("B³¹d", $"Wpisz {_dlugoscKodu} cyfry!", "OK");
                return;
            }

            int[] zgadywane = new int[_dlugoscKodu];
            for (int i = 0; i < _dlugoscKodu; i++)
            {
                zgadywane[i] = int.Parse(_wpisanyTekst[i].ToString());
            }


            List<int> kopiaSekretu = new List<int>(_tajnyKod);
            Color[] koloryWyniku = new Color[_dlugoscKodu];
            for (int i = 0; i < _dlugoscKodu; i++) koloryWyniku[i] = Colors.LightGray;

            for (int i = 0; i < _dlugoscKodu; i++)
            {
                if (zgadywane[i] == _tajnyKod[i])
                {
                    koloryWyniku[i] = Colors.LightGreen;
                    kopiaSekretu.Remove(zgadywane[i]);
                    zgadywane[i] = -1; 
                }
            }

            for (int i = 0; i < _dlugoscKodu; i++)
            {
                if (zgadywane[i] != -1 && kopiaSekretu.Contains(zgadywane[i]))
                {
                    koloryWyniku[i] = Colors.Khaki;
                    kopiaSekretu.Remove(zgadywane[i]);
                }
            }

            for (int i = 0; i < _dlugoscKodu; i++)
            {
                Label etykieta = _polaPlanszy[_aktualnaProba, i];
                Border ramka = etykieta.Parent as Border;

                if (ramka != null)
                {
                    await ramka.RotateXTo(90, 200);
                    ramka.BackgroundColor = koloryWyniku[i];
                    await ramka.RotateXTo(0, 200);
                }
            }

            bool wygrana = true;
            foreach (var kol in koloryWyniku)
            {
                if (kol != Colors.LightGreen) wygrana = false;
            }

            if (wygrana)
            {
                GameStats.Wins++;
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                await PokazKomunikatKoniecGry("WYGRANA!", $"Zgad³eœ w {_aktualnaProba + 1} próbie.");
                return;
            }

            _aktualnaProba++;
            _wpisanyTekst = "";

            if (_aktualnaProba >= _maksProb)
            {
                GameStats.Losses++;
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                string poprawnyKod = string.Join("", _tajnyKod);
                await PokazKomunikatKoniecGry("PRZEGRANA", $"Poprawny kod to: {poprawnyKod}");
            }
        }

        private async Task PokazKomunikatKoniecGry(string tytul, string wiadomosc)
        {
            wiadomosc += $"\n\nStatystyki:\nWygrane: {GameStats.Wins}\nPrzegrane: {GameStats.Losses}";

            string akcja = await DisplayActionSheet(tytul + "\n" + wiadomosc, "WyjdŸ", null, "Nowa Gra");

            if (akcja == "Nowa Gra")
            {
                NowaGra();
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        private async void KliknieciePowrotMenu(object sender, EventArgs e)
        {
            bool decyzja = await DisplayAlert("Wyjœcie", "Czy na pewno chcesz przerwaæ grê?", "Tak", "Nie");
            if (decyzja)
            {
                await Navigation.PopAsync();
            }
        }
    }
}