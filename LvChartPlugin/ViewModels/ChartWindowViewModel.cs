using System;
using System.Collections.Generic;
using System.Linq;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using LvChartPlugin.Settings;

namespace LvChartPlugin.ViewModels
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


        #region CountMaximumMaxValue変更通知プロパティ
        private int _CountMaximumMaxValue;

        public int CountMaximumMaxValue
        {
            get
            { return this._CountMaximumMaxValue; }
            set
            {
                if (this._CountMaximumMaxValue == value)
                    return;
                this._CountMaximumMaxValue = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region CountMaximumCurrentValue変更通知プロパティ
        private int _CountMaximumCurrentValue;

        public int CountMaximumCurrentValue
        {
            get
            { return this._CountMaximumCurrentValue; }
            set
            { 
                if (this._CountMaximumCurrentValue == value)
                    return;
                this._CountMaximumCurrentValue = value;
                ChartSettings.CountMaximumCurrentValue.Value = value;
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
                ChartSettings.LevelInterval.Value = value;
                this.UpdateCountMaximum();
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
                ChartSettings.IsLocked.Value = value;
                this.UpdateView();
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region IsCheckALL変更通知プロパティ
        private bool isCheckAll;

        public bool IsCheckAll
        {
            get
            { return this.isCheckAll; }
            set
            {
                if (this.isCheckAll == value)
                    return;
                this.isCheckAll = value;
                this.CheckAll();
                this.RaisePropertyChanged();
            }
        }
        #endregion

        public IReadOnlyCollection<ShipTypeViewModel> ShipTypes { get; private set; }

        public ChartWindowViewModel()
        {
            if (KanColleClient.Current.IsStarted)
            {
                var masterShipTypes = KanColleClient.Current.Master.Ships //艦娘マスタで使ってるタイプだけ
                    .Where(x => x.Value.Id < 1500) //敵以外
                    .GroupBy(x => x.Value.ShipType, (key, elements) => key)
                    .OrderBy(x => x.Id);
                if(ChartSettings.SelectedShipTypes.Value == null)
                {
                    ChartSettings.SelectedShipTypes.Value = masterShipTypes.Select(x => x.Id).ToArray();
                }
                this.ShipTypes = masterShipTypes
                    .Select(type => new ShipTypeViewModel(type)
                    {
                        IsSelected = ChartSettings.SelectedShipTypes.Value.Any(id => type.Id == id),
                        SelectionChangedAction = () =>
                        {
                            ChartSettings.SelectedShipTypes.Value = this.ShipTypes.Where(x => x.IsSelected).Select(x => x.Id).ToArray();
                            this.UpdateView();
                        }
                    })
                    .ToArray();
            }
            this.CountMaximumCurrentValue = ChartSettings.CountMaximumCurrentValue;
            this.LevelInterval = ChartSettings.LevelInterval;
            this.IsLocked = ChartSettings.IsLocked;
            this.IsCheckAll = this.ShipTypes.All(x => x.IsSelected);
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

        private void CheckAll()
        {
            if (this.ShipTypes == null) return;

            foreach (var type in this.ShipTypes)
            {
                type.IsSelected = this.IsCheckAll;
            }
        }

        private void UpdateView()
        {
            if (!KanColleClient.Current.IsStarted) return;

            this.Ships = KanColleClient.Current.Homeport.Organization.Ships.Values
                .Where(this.FilterView).ToArray();

            this.UpdateCountMaximum();
        }

        private void UpdateCountMaximum()
        {
            if (this.Ships == null) return;

            var maxValue = this.Ships.Any()
                ? this.Ships.CreateShipData(this.LevelInterval, 0, CommonSettings.LevelLimit).Values.SumValues().CountMaximum()
                : ChartSettings.CountMaximumCurrentValue.Default;
            maxValue = Math.Max(maxValue, ChartSettings.CountMaximumCurrentValue.Default);

            this.CountMaximumCurrentValue = Math.Min(this.CountMaximumCurrentValue, maxValue);
            this.CountMaximumMaxValue = maxValue;
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
