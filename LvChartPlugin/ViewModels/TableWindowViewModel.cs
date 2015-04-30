using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace LvChartPlugin.ViewModels
{
    public class TableWindowViewModel : ViewModel
    {

        #region ShipTable変更通知プロパティ
        private ShipTableRow[] _ShipTable;

        public ShipTableRow[] ShipTable
        {
            get
            { return this._ShipTable; }
            set
            { 
                if (this._ShipTable == value)
                    return;
                this._ShipTable = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region ShipTableSubHeader変更通知プロパティ
        private ShipTableHeader _shipTableHeader;

        public ShipTableHeader ShipTableHeader
        {
            get
            { return this._shipTableHeader; }
            set
            { 
                if (this._shipTableHeader == value)
                    return;
                this._shipTableHeader = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsLocked変更通知プロパティ
        private bool _IsLocked;

        public bool IsLocked
        {
            get
            { return this._IsLocked; }
            set
            {
                if (this._IsLocked == value)
                    return;
                this._IsLocked = value;
                this.RaisePropertyChanged();
                this.UpdateView();
                Properties.Settings.Default.IsLocked = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion


        #region LevelMaximum変更通知プロパティ
        private int _LevelMaximum;

        public int LevelMaximum
        {
            get
            { return this._LevelMaximum; }
            set
            { 
                if (this._LevelMaximum == value)
                    return;
                this._LevelMaximum = value;
                this.RaisePropertyChanged();
                this.UpdateView();
                Properties.Settings.Default.LevelMaximum = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion


        #region LevelMinimum変更通知プロパティ
        private int _LevelMinimum;

        public int LevelMinimum
        {
            get
            { return this._LevelMinimum; }
            set
            { 
                if (this._LevelMinimum == value)
                    return;
                this._LevelMinimum = value;
                this.RaisePropertyChanged();
                this.UpdateView();
                Properties.Settings.Default.LevelMinimum = value;
                Properties.Settings.Default.Save();
            }
        }
        #endregion


        public TableWindowViewModel()
        {
            Properties.Settings.Default.Reload();
            var settings = Properties.Settings.Default;
            this._LevelMaximum = settings.LevelMaximum;
            this._LevelMinimum = settings.LevelMinimum;
            this._IsLocked = settings.IsLocked;
        }

        public void Initialize()
        {
            this.UpdateView();
            this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Organization)
            {
                {
                    () => KanColleClient.Current.Homeport.Organization.Ships,
                    (_, __) => { DispatcherHelper.UIDispatcher.Invoke(this.UpdateView); }
                }
            });
        }

        private void UpdateView()
        {
            if (!KanColleClient.Current.IsStarted) return;

            this.ShipTable = KanColleClient.Current.Homeport.Organization.Ships.Values
                .Where(x => !this.IsLocked || x.IsLocked)
                .Where(x => x.Level <= this.LevelMaximum)
                .Where(x => this.LevelMinimum <= x.Level)
                .GroupBy(x => x.Level)
                .Select(x => new ShipTableRow
                {
                    Lv = x.Key,
                    Destroyer = x.Where(s => s.Info.ShipType.Id == 2).ToDisplayStrings(),
                    LightCruiser = x.Where(s => s.Info.ShipType.Id == 3).ToDisplayStrings(),
                    TorpedoCruiser = x.Where(s => s.Info.ShipType.Id == 4).ToDisplayStrings(),
                    HeavyCruiser = x.Where(s => s.Info.ShipType.Id == 5).ToDisplayStrings(),
                    AviationCruiser = x.Where(s => s.Info.ShipType.Id == 6).ToDisplayStrings(),
                    LightAircraftCarrier = x.Where(s => s.Info.ShipType.Id == 7).ToDisplayStrings(),
                    AircraftCarrier = x.Where(s => s.Info.ShipType.Id.Any(11, 18)).ToDisplayStrings(),
                    AviationBattleShip = x.Where(s => s.Info.ShipType.Id == 10).ToDisplayStrings(),
                    BattleCruiser = x.Where(s => s.Info.ShipType.Id == 8).ToDisplayStrings(),
                    BattleShip = x.Where(s => s.Info.ShipType.Id == 9).ToDisplayStrings(),
                    Submarine = x.Where(s => s.Info.ShipType.Id.Any(13, 14)).ToDisplayStrings(),
                    Other = x.Where(s => !s.Info.ShipType.Id.Any(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 18)).ToDisplayStrings(),
                })
                .OrderByDescending(x => x.Lv)
                .ToArray();

            this.ShipTableHeader = new ShipTableHeader
            {
                Destroyer = "駆逐艦\r\n" + this.ShipTable.SelectMany(x => x.Destroyer).ToSubHeader(),
                LightCruiser = "軽巡洋艦\r\n" + this.ShipTable.SelectMany(x => x.LightCruiser).ToSubHeader(),
                TorpedoCruiser = "重雷装巡洋艦\r\n" + this.ShipTable.SelectMany(x => x.TorpedoCruiser).ToSubHeader(),
                HeavyCruiser = "重巡洋艦\r\n" + this.ShipTable.SelectMany(x => x.HeavyCruiser).ToSubHeader(),
                AviationCruiser = "航空巡洋艦\r\n" + this.ShipTable.SelectMany(x => x.AviationCruiser).ToSubHeader(),
                LightAircraftCarrier = "軽空母\r\n" + this.ShipTable.SelectMany(x => x.LightAircraftCarrier).ToSubHeader(),
                AircraftCarrier = "正規・装甲空母\r\n" + this.ShipTable.SelectMany(x => x.AircraftCarrier).ToSubHeader(),
                AviationBattleShip = "航空戦艦\r\n" + this.ShipTable.SelectMany(x => x.AviationBattleShip).ToSubHeader(),
                BattleCruiser = "巡洋戦艦\r\n" + this.ShipTable.SelectMany(x => x.BattleCruiser).ToSubHeader(),
                BattleShip = "戦艦\r\n" + this.ShipTable.SelectMany(x => x.BattleShip).ToSubHeader(),
                Submarine = "潜水艦・潜水空母\r\n" + this.ShipTable.SelectMany(x => x.Submarine).ToSubHeader(),
                Other = "その他\r\n" + this.ShipTable.SelectMany(x => x.Other).ToSubHeader(),
            };
        }
    }

    internal static class TableWindowViewModelExtensions
    {
        public static bool Any(this int src, params int[] values)
        {
            return values.Any(v => v == src);
        }

        public static string ToDisplayString(this Ship ship)
        {
            return ship.Level + " " + ship.Info.Name;
        }

        public static string[] ToDisplayStrings(this IEnumerable<Ship> ships)
        {
            return ships.Select(s => s.ToDisplayString()).ToArray();
        }

        public static string ToSubHeader(this IEnumerable<string> ships)
        {
            return "(" + ships.Count() + " 隻)";
        }
    }

    public class ShipTableRow
    {
        public int Lv { get; set; }
        public string[] Destroyer { get; set; }
        public string[] LightCruiser { get; set; }
        public string[] TorpedoCruiser { get; set; }
        public string[] HeavyCruiser { get; set; }
        public string[] AviationCruiser { get; set; }
        public string[] LightAircraftCarrier { get; set; }
        public string[] AircraftCarrier { get; set; }
        public string[] AviationBattleShip { get; set; }
        public string[] BattleCruiser { get; set; }
        public string[] BattleShip { get; set; }
        public string[] Submarine { get; set; }
        public string[] Other { get; set; }
    }

    public class ShipTableHeader
    {
        public string Destroyer { get; set; }
        public string LightCruiser { get; set; }
        public string TorpedoCruiser { get; set; }
        public string HeavyCruiser { get; set; }
        public string AviationCruiser { get; set; }
        public string LightAircraftCarrier { get; set; }
        public string AircraftCarrier { get; set; }
        public string AviationBattleShip { get; set; }
        public string BattleCruiser { get; set; }
        public string BattleShip { get; set; }
        public string Submarine { get; set; }
        public string Other { get; set; }
    }
}
