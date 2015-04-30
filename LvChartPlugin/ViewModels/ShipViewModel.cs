using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;


namespace LvChartPlugin.ViewModels
{
    public class ShipViewModel : ViewModel
    {

        #region Level変更通知プロパティ
        private int _Level;

        public int Level
        {
            get
            { return this._Level; }
            set
            { 
                if (this._Level == value)
                    return;
                this._Level = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        #region Name変更通知プロパティ
        private string _Name;

        public string Name
        {
            get
            { return this._Name; }
            set
            { 
                if (this._Name == value)
                    return;
                this._Name = value;
                this.RaisePropertyChanged();
            }
        }
        #endregion


        public ShipViewModel() { }

        public ShipViewModel(Ship ship)
        {
            this.Level = ship.Level;
            this.Name = ship.Info.Name;
        }
    }
}
