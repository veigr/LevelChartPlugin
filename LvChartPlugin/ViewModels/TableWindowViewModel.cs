using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace LvChartPlugin.ViewModels
{
    public class TableWindowViewModel : ViewModel
    {

        #region ShipTable変更通知プロパティ
        private IEnumerable<ShipTableRow> _ShipTable;

        public IEnumerable<ShipTableRow> ShipTable
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

        public void Initialize()
        {
            this.ShipTable = KanColleClient.Current.Homeport.Organization.Ships.Values
                .GroupBy(x => x.Level)
                .Select(x => new ShipTableRow
                {
                    Lv = x.Key,
                    Destroyer = x.Where(s => s.Info.ShipType.Id == 2),
                    LightCruiser = x.Where(s => s.Info.ShipType.Id == 3),
                    TorpedoCruiser = x.Where(s => s.Info.ShipType.Id == 4),
                    HeavyCruiser = x.Where(s => s.Info.ShipType.Id == 5),
                    AviationCruiser = x.Where(s => s.Info.ShipType.Id == 6),
                    LightAircraftCarrier = x.Where(s => s.Info.ShipType.Id == 7),
                    AircraftCarrier = x.Where(s => s.Info.ShipType.Id.Any(11, 18)),
                    AviationBattleShip = x.Where(s => s.Info.ShipType.Id == 10),
                    BattleCruiser = x.Where(s => s.Info.ShipType.Id == 8),
                    BattleShip = x.Where(s => s.Info.ShipType.Id == 9),
                    Submarine = x.Where(s => s.Info.ShipType.Id.Any(13, 14)),
                    Other = x.Where(s => !s.Info.ShipType.Id.Any(2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 18)),
                })
                .OrderByDescending(x => x.Lv)
                .ToArray();
        }
    }

    internal static class TableWindowViewModelExtensions
    {
        public static bool Any(this int src, params int[] values)
        {
            return values.Any(v => v == src);
        }
    }

    public class ShipTableRow
    {
        public int Lv { get; set; }
        public IEnumerable<Ship> Destroyer { get; set; }
        public IEnumerable<Ship> LightCruiser { get; set; }
        public IEnumerable<Ship> TorpedoCruiser { get; set; }
        public IEnumerable<Ship> HeavyCruiser { get; set; }
        public IEnumerable<Ship> AviationCruiser { get; set; }
        public IEnumerable<Ship> LightAircraftCarrier { get; set; }
        public IEnumerable<Ship> AircraftCarrier { get; set; }
        public IEnumerable<Ship> AviationBattleShip { get; set; }
        public IEnumerable<Ship> BattleCruiser { get; set; }
        public IEnumerable<Ship> BattleShip { get; set; }
        public IEnumerable<Ship> Submarine { get; set; }
        public IEnumerable<Ship> Other { get; set; }
    }
}
