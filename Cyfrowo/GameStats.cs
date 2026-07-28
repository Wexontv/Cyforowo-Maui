using Microsoft.Maui.Storage; 

namespace Cyfrowo
{
    public static class GameStats
    {
        private const string WinsKey = "stats_wins";
        private const string LossesKey = "stats_losses";

        public static int Wins
        {
            get => Preferences.Get(WinsKey, 0);
            set => Preferences.Set(WinsKey, value);
        }

        public static int Losses
        {
            get => Preferences.Get(LossesKey, 0);
            set => Preferences.Set(LossesKey, value);
        }

        public static void ResetujStatystyki()
        {
            Preferences.Remove(WinsKey);
            Preferences.Remove(LossesKey);
        }
    }
}