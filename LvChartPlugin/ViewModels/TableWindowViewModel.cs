using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System;

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
        private ShipTableSubHeader _ShipTableSubHeader;

        public ShipTableSubHeader ShipTableSubHeader
        {
            get
            { return this._ShipTableSubHeader; }
            set
            {
                if (this._ShipTableSubHeader == value)
                    return;
                this._ShipTableSubHeader = value;
                this.RaisePropertyChanged();
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

            var groupedShips = KanColleClient.Current.Homeport.Organization.Ships.Values
                .Where(x => !this.IsLocked || x.IsLocked)
                .Where(x => x.Level <= this.LevelMaximum)
                .Where(x => this.LevelMinimum <= x.Level)
                .GroupBy(x => x.Level)
                .Select(x => new ShipTableRow
                {
                    Lv = x.Key,
                    Destroyer = x.Where(s => s.Info.ShipType.Id == 2).ToDisplayValue(),
                    LightCruiser = x.Where(s => s.Info.ShipType.Id == 3).ToDisplayValue(),
                    TorpedoCruiser = x.Where(s => s.Info.ShipType.Id == 4).ToDisplayValue(),
                    HeavyCruiser = x.Where(s => s.Info.ShipType.Id == 5).ToDisplayValue(),
                    AviationCruiser = x.Where(s => s.Info.ShipType.Id == 6).ToDisplayValue(),
                    LightAircraftCarrier = x.Where(s => s.Info.ShipType.Id == 7).ToDisplayValue(),
                    AircraftCarrier = x.Where(s => s.Info.ShipType.Id.Any(11, 18)).ToDisplayValue(),
                    AviationBattleShip = x.Where(s => s.Info.ShipType.Id == 10).ToDisplayValue(),
                    BattleCruiser = x.Where(s => s.Info.ShipType.Id == 8).ToDisplayValue(),
                    BattleShip = x.Where(s => s.Info.ShipType.Id == 9).ToDisplayValue(),
                    Submarine = x.Where(s => s.Info.ShipType.Id.Any(13, 14)).ToDisplayValue(),
                    Other = x.Where(s => !s.Info.ShipType.Id.Any(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 18)).ToDisplayValue(),
                })
                .ToArray();

            var left = Enumerable.Range(1, 99)
                .GroupJoin(groupedShips, x => x, x => x.Lv, (lv, row) => row.SingleOrDefault() ?? new ShipTableRow { Lv = lv });
            var right = groupedShips
                .GroupJoin(Enumerable.Range(1, 99), x => x.Lv, x => x, (x, y) => x);
            this.ShipTable = left.Union(right)
                .Where(x => x.Lv <= this.LevelMaximum)
                .Where(x => this.LevelMinimum <= x.Lv)
                .OrderByDescending(x => x.Lv)
                .ToArray();

            this.UpdateSubHeader(groupedShips);
        }

        private void UpdateSubHeader(ShipTableRow[] ships)
        {
            this.ShipTableSubHeader = new ShipTableSubHeader
            {
                Destroyer = ships.SelectMany(x => x.Destroyer).ToSubHeader(),
                LightCruiser = ships.SelectMany(x => x.LightCruiser).ToSubHeader(),
                TorpedoCruiser = ships.SelectMany(x => x.TorpedoCruiser).ToSubHeader(),
                HeavyCruiser = ships.SelectMany(x => x.HeavyCruiser).ToSubHeader(),
                AviationCruiser = ships.SelectMany(x => x.AviationCruiser).ToSubHeader(),
                LightAircraftCarrier = ships.SelectMany(x => x.LightAircraftCarrier).ToSubHeader(),
                AircraftCarrier = ships.SelectMany(x => x.AircraftCarrier).ToSubHeader(),
                AviationBattleShip = ships.SelectMany(x => x.AviationBattleShip).ToSubHeader(),
                BattleCruiser = ships.SelectMany(x => x.BattleCruiser).ToSubHeader(),
                BattleShip = ships.SelectMany(x => x.BattleShip).ToSubHeader(),
                Submarine = ships.SelectMany(x => x.Submarine).ToSubHeader(),
                Other = ships.SelectMany(x => x.Other).ToSubHeader(),
            };
        }
    }

    internal static class TableWindowViewModelExtensions
    {
        public static bool Any(this int src, params int[] values)
            => values.Any(v => v == src);

        public static string ToDisplayString(this Ship ship)
            => $"{ship.Level} {ship.Info.Name}";

        public static ShipViewModel[] ToDisplayValue(this IEnumerable<Ship> ships)
            => ships
                .OrderBy(s => s.SortNumber)
                .Select(s => new ShipViewModel(s))
                .ToArray();

        public static string ToSubHeader(this IEnumerable<ShipViewModel> ships)
            => $"({ships.Count()} 隻)";
    }

    public class ShipTableRow
    {
        public int Lv { get; set; }
        public ShipViewModel[] Destroyer { get; set; }
        public ShipViewModel[] LightCruiser { get; set; }
        public ShipViewModel[] TorpedoCruiser { get; set; }
        public ShipViewModel[] HeavyCruiser { get; set; }
        public ShipViewModel[] AviationCruiser { get; set; }
        public ShipViewModel[] LightAircraftCarrier { get; set; }
        public ShipViewModel[] AircraftCarrier { get; set; }
        public ShipViewModel[] AviationBattleShip { get; set; }
        public ShipViewModel[] BattleCruiser { get; set; }
        public ShipViewModel[] BattleShip { get; set; }
        public ShipViewModel[] Submarine { get; set; }
        public ShipViewModel[] Other { get; set; }
    }

    public class ShipTableSubHeader
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
