using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace LvChartPlugin
{
    public class ChartWindowViewModel : ViewModel
    {

        #region Ships変更通知プロパティ
        private IEnumerable<Ship> _Ships;

        public IEnumerable<Ship> Ships
        {
            get
            { return this._Ships; }
            set
            { 
                if (Equals(this._Ships, value)) return;
                this._Ships = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region CountMaximum変更通知プロパティ
        private int _CountMaximum;

        public int CountMaximum
        {
            get
            { return this._CountMaximum; }
            set
            {
                if (this._CountMaximum == value)
                    return;
                this._CountMaximum = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region LevelInterval変更通知プロパティ
        private int _LevelInterval;

        public int LevelInterval
        {
            get
            { return this._LevelInterval; }
            set
            { 
                if (this._LevelInterval == value)
                    return;
                this._LevelInterval = value;
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
                this.UpdateView();
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsCheckALL変更通知プロパティ
        private bool _IsCheckALL;

        public bool IsCheckALL
        {
            get
            { return this._IsCheckALL; }
            set
            {
                if (this._IsCheckALL == value)
                    return;
                this._IsCheckALL = value;
                this.CheckALL();
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public IReadOnlyCollection<ShipTypeViewModel> ShipTypes { get; private set; }

        public ChartWindowViewModel()
        {
            if (KanColleClient.Current.IsStarted) this.ShipTypes =
                 KanColleClient.Current.Master.Ships
                 .Where(x => x.Value.Id < 501 || 900 < x.Value.Id)  //敵以外
                 .GroupBy(x => x.Value.ShipType, (key, elements) => key)
                 .OrderBy(x => x.Id)
                 .Select(x => new ShipTypeViewModel(x)
                 {
                     IsSelected = true,
                     SelectionChangedAction = () => this.UpdateView()
                 })
                 .ToArray();
            this.LevelInterval = 10;
            this.IsLocked = true;
            this.IsCheckALL = true;
        }

        public void Initialize()
        {
            this.Ships = KanColleClient.Current.Homeport.Organization.Ships.Values.ToArray();

            this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current.Homeport.Organization)
            {
                {
                    () => KanColleClient.Current.Homeport.Organization.Ships,
                    (_, __) =>
                    {
                        DispatcherHelper.UIDispatcher.Invoke(this.UpdateView);
                    }
                }
            });
        }

        private void CheckALL()
        {
            if (this.ShipTypes == null) return;

            foreach (var type in this.ShipTypes)
            {
                type.IsSelected = this.IsCheckALL;
            }
        }

        private void UpdateView()
        {
            if (!KanColleClient.Current.IsStarted) return;

            this.Ships = KanColleClient.Current.Homeport.Organization.Ships.Values
                .Where(this.FilterView).ToArray();

            if (this.Ships == null) return;

            this.CountMaximum = this.Ships.Any()
                ? this.Ships.CreateShipData(this.LevelInterval).Values.SumValues().CountMaximum()
                : 11;
        }

        private bool FilterView(Ship s)
        {
            if (s == null) return false;
            if (this.ShipTypes == null) return false;

            return (this.IsLocked && s.IsLocked || !this.IsLocked)
                   && this.ShipTypes.Where(t => t.IsSelected).Any(t => s.Info.ShipType.Id == t.Id);
        }
    }
}
